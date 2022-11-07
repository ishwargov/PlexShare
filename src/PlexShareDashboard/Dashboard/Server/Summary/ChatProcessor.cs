using ScottPlot.Palettes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

using PlexShareContent;
using System.IO;
using System.Reflection;

using PlexShareDashboard.Dashboard.Server.Summary;
using PlexShareDashboard.Dashboard.Server.Telemetry;


namespace PlexShareDashboard.Dashboard.Server.Summary
{
    public class EmptyStringException : Exception
    {
    }
    public class ChatProcessor
    {
        public static int SummariesGenerated { get; set; }
        public static List<string> StopWords;
        private static IPorterStemmer _stemmer;

        public ChatProcessor()
        {
            _stemmer = new EnglishPorter2Stemmer();
        }


        /// <summary>
        /// Loads stop words (e.g. a/an/the...) from resource file for exclusion from analysis
        /// </summary>
        public static void LoadStopWords()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            StreamReader reader = new StreamReader(asm.GetManifestResourceStream("PlexShareDashboard.Dashboard.Server.Summary.Resources.stop_words.txt"));
            string stopwords = reader.ReadToEnd();
            StopWords = new List<string>(stopwords.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
        }


        /// <summary>
        /// Removes punctuation and case
        /// </summary>
        public static List<(string, bool)> CleanChat(List<(string, bool)> discussionChat)
        {
            List<(string, bool)> cleanedDiscussionChat = new List<(string, bool)>();
            foreach (var discussionChatItem in discussionChat)
            {
                string textWithoutPunctuation = Regex.Replace(discussionChatItem.Item1, @"(\p{P})", "").ToLower();
                if (textWithoutPunctuation.Length == 0) continue;
                cleanedDiscussionChat.Add((textWithoutPunctuation, discussionChatItem.Item2));
            }
            return cleanedDiscussionChat;
        }


        public static List<string> GetWordsFromSentence(string chatMessage)
        {
            List<string> words = new List<string>(chatMessage.Split());
            words.RemoveAll(word => word == "");
            for (int i = 0; i < words.Count(); i++)
                words[i] = words[i].Replace(".", "");
            return words;
        }

        /// <summary>
        /// Find the number of occurances of each non-stop word
        /// </summary>
        public static Dictionary<string, int> CountWords(List<(string, bool)> discussionChat)
        {
            var wordCounts = new Dictionary<string, int>();

            foreach (var discussionChatItem in discussionChat)
            {
                List<string> words = GetWordsFromSentence(discussionChatItem.Item1);
                foreach (var word in words)
                {
                    string stemOfWord = _stemmer.Stem(word);
                    if (!wordCounts.ContainsKey(stemOfWord))
                    {
                        // Check to see if current word is a stop word or not
                        if (!StopWords.Contains(word) && !StopWords.Contains(stemOfWord))
                            wordCounts[stemOfWord] = 1;
                    }
                    else
                    {
                        wordCounts[stemOfWord] += 1;
                    }
                }
            }

            return wordCounts;
        }


        /// <summary>
        /// Sorts all words that occur more than once by count descending
        /// </summary>
        public static string[] SortWords(Dictionary<string, int> wordCounts)
        {
            var sortedWords = from word in wordCounts
                              where word.Value > 1
                              orderby word.Value descending
                              select word.Key;

            return sortedWords.ToArray();
        }


        /// <summary>
        /// Assigns a score for each word based on what percentile
        /// it falls in when ordered by count descending
        /// </summary>
        public static Dictionary<string, int> ScoreWords(string[] sortedWords)
        {
            var wordScores = new Dictionary<string, int>();
            int count = sortedWords.Length;
            int score;

            for (int i = 0; i < count; i++)
            {
                if (i < 0.05 * count)
                    score = 5;
                else if (i < 0.2 * count)
                    score = 4;
                else if (i < 0.4 * count)
                    score = 3;
                else if (i < 0.6 * count)
                    score = 2;
                else if (i < 0.8 * count)
                    score = 1;
                else
                    break;

                wordScores.Add(sortedWords[i], score);
            }

            return wordScores;
        }


        /// <summary>
        /// Scores sentences
        /// </summary>
        public static Dictionary<string, int> ScoreSentences(List<(string, bool)> cleanedDiscussionChat, Dictionary<string, int> wordScores)
        {
            var sentenceScores = new Dictionary<string, int>();

            foreach (var chatMessage in cleanedDiscussionChat)
            {
                // Separate words
                List<string> cleanSentenceWords = GetWordsFromSentence(chatMessage.Item1);

                int score = 0;

                // Tally the score of sentence by summing scores of each word in sentence
                foreach (string word in cleanSentenceWords)
                {
                    Debug.WriteLine("word ==== " + word);
                    string stemOfWord = _stemmer.Stem(word);
                    if(wordScores.ContainsKey(word))
                        score += wordScores[word];
                    else if (wordScores.ContainsKey(stemOfWord))
                        score += wordScores[stemOfWord];
                }
                // Make sure no duplicate additions are attempted, e.g. two emtpy strings
                if (!sentenceScores.ContainsKey(chatMessage.Item1))
                    sentenceScores.Add(chatMessage.Item1, score);
            }

            foreach (KeyValuePair<string, int> entry in sentenceScores)
            {
                Debug.WriteLine(entry.Key+"  "+entry.Value + "\n");
            }
            return sentenceScores;
        }


        /// <summary>
        /// Returns only the top XX% of sentences specified by percentToKeep
        /// </summary>
        public static string[] SortSentences(Dictionary<string, int> sentenceScores, double percentToKeep = 0.4)
        {
            // Find the top XX% of sentences when ranked by score descending
            var topSentences = (from sentence in sentenceScores
                                orderby sentence.Value descending
                                select sentence.Key).Take(Convert.ToInt16(percentToKeep * sentenceScores.Count));
            
            // Considering a case of chat getting spammed by one single message
            if (topSentences.ToArray().Length==0)
            {
                topSentences = (from sentence in sentenceScores
                                orderby sentence.Value descending
                                select sentence.Key).Take(Convert.ToInt16(1));
            }
            
            return topSentences.ToArray();
        }


        /// <summary>
        /// Builds an ordered summary from the final sentences
        /// </summary>
        public static string BuildSummary(List<(string, bool)> cleanedDiscussionChat, string[] finalSentences, List<(string, bool)> discussionChat)
        {
            StringBuilder summary = new StringBuilder();
            Dictionary<string, bool> visitedFinalSentence = new();
            for(int i= 0;i < cleanedDiscussionChat.Count;i++)
            {
                if (finalSentences.Contains(cleanedDiscussionChat[i].Item1) && !visitedFinalSentence.ContainsKey(discussionChat[i].Item1))
                {
                    summary.Append(discussionChat[i].Item1);
                    summary.Append(". \n\n");

                    visitedFinalSentence[discussionChat[i].Item1] = true;
                }
            }

            return summary.ToString();
        }


        /// <summary>
        /// Gemerate summary from chats
        /// </summary>
        public string Summarize(List<(string, bool)> discussionChat)
        {
            // Load stop words if a summary has not been generated yet
            if (SummariesGenerated == 0)
            {
                LoadStopWords();
            }
            //List<(string, bool)> discussionChat = new List<(string, bool)>() {("Usain Bolt rounded off the world championships Sunday by claiming his third gold in Moscow as he anchored Jamaica to victory in the men’s 4x100m relay.\r\nThe fastest man in the world charged clear of United States rival Justin Gatlin as the Jamaican quartet of Nesta Carter, Kemar Bailey-Cole, Nickel Ashmeade and Bolt won in 37.36 seconds.", false),
            //                  ("The U.S finished second in 37.56 seconds with Canada taking the bronze after Britain were disqualified for a faulty handover.\r\nThe 26-year-old Bolt has now collected eight gold medals at world championships, equaling the record held by American trio Carl Lewis, Michael Johnson and Allyson Felix, not to mention the small matter of six Olympic titles.", true),
            //                  ("Usain Bolt: I try to clear my mind\r\nThe relay triumph followed individual successes in the 100 and 200 meters in the Russian capital.\r\n“I’m proud of myself and I’ll continue to work to dominate for as long as possible,” Bolt said, having previously expressed his intention to carry on until the 2016 Rio Olympics.", false),
            //                   ("Victory was never seriously in doubt once he got the baton safely in hand from Ashmeade, while Gatlin and the United States third leg runner Rakieem Salaam had problems.\r\nGatlin strayed out of his lane as he struggled to get full control of their baton and was never able to get on terms with Bolt.\r\nEarlier, Jamaica’s women underlined their dominance in the sprint events by winning the 4x100m relay gold, anchored by Shelly-Ann Fraser-Pryce, who like Bolt was completing a triple.",false),
            //("Their quartet recorded a championship record of 41.29 seconds, well clear of France, who crossed the line in second place in 42.73 seconds.\r\nDefending champions, the United States, were initially back in the bronze medal position after losing time on the second handover between Alexandria Anderson and English Gardner, but promoted to silver when France were subsequently disqualified for an illegal handover.\r\nThe British quartet, who were initially fourth, were promoted to the bronze which eluded their men’s team.",true),
            //("Fraser-Pryce, like Bolt aged 26, became the first woman to achieve three golds in the 100-200 and the relay.\r\nIn other final action on the last day of the championships, France’s Teddy Tamgho became the third man to leap over 18m in the triple jump, exceeding the mark by four centimeters to take gold.",false),
            //("Germany’s Christina Obergfoll finally took gold at global level in the women’s javelin after five previous silvers, while Kenya’s Asbel Kiprop easily won a tactical men’s 1500m final.",false)};
            // Grab the inner text from the HTML tags and combine into one string
            //string text = "\r\nCNN\r\n — \r\nUsain Bolt rounded off the world championships Sunday by claiming his third gold in Moscow as he anchored Jamaica to victory in the men’s 4x100m relay.\r\n\r\nThe fastest man in the world charged clear of United States rival Justin Gatlin as the Jamaican quartet of Nesta Carter, Kemar Bailey-Cole, Nickel Ashmeade and Bolt won in 37.36 seconds.\r\n\r\nThe U.S finished second in 37.56 seconds with Canada taking the bronze after Britain were disqualified for a faulty handover.\r\n\r\nThe 26-year-old Bolt has now collected eight gold medals at world championships, equaling the record held by American trio Carl Lewis, Michael Johnson and Allyson Felix, not to mention the small matter of six Olympic titles.\r\n\r\nUsain Bolt: I try to clear my mind\r\nThe relay triumph followed individual successes in the 100 and 200 meters in the Russian capital.\r\n\r\n“I’m proud of myself and I’ll continue to work to dominate for as long as possible,” Bolt said, having previously expressed his intention to carry on until the 2016 Rio Olympics.\r\n\r\nVictory was never seriously in doubt once he got the baton safely in hand from Ashmeade, while Gatlin and the United States third leg runner Rakieem Salaam had problems.\r\n\r\nGatlin strayed out of his lane as he struggled to get full control of their baton and was never able to get on terms with Bolt.\r\n\r\nEarlier, Jamaica’s women underlined their dominance in the sprint events by winning the 4x100m relay gold, anchored by Shelly-Ann Fraser-Pryce, who like Bolt was completing a triple.\r\n\r\nTheir quartet recorded a championship record of 41.29 seconds, well clear of France, who crossed the line in second place in 42.73 seconds.\r\n\r\nDefending champions, the United States, were initially back in the bronze medal position after losing time on the second handover between Alexandria Anderson and English Gardner, but promoted to silver when France were subsequently disqualified for an illegal handover.\r\n\r\nThe British quartet, who were initially fourth, were promoted to the bronze which eluded their men’s team.\r\n\r\nFraser-Pryce, like Bolt aged 26, became the first woman to achieve three golds in the 100-200 and the relay.\r\n\r\nIn other final action on the last day of the championships, France’s Teddy Tamgho became the third man to leap over 18m in the triple jump, exceeding the mark by four centimeters to take gold.\r\n\r\nGermany’s Christina Obergfoll finally took gold at global level in the women’s javelin after five previous silvers, while Kenya’s Asbel Kiprop easily won a tactical men’s 1500m final.\r\n\r\n";

            // Remove punctuation and case
            List<(string, bool)> cleanedDiscussionChat = CleanChat(discussionChat);

            // Find each individual word from the text and remove blanks

            // Tally up word counts for non-stop words
            Dictionary<string, int> wordCounts = CountWords(cleanedDiscussionChat);

            // Sort words that occur more than once by word count, descending
            string[] sortedWords = SortWords(wordCounts);

            // Assign a point value to each word based on its percentile
            Dictionary<string, int> wordScores = ScoreWords(sortedWords);

            // Score each individual sentence
            Dictionary<string, int> sentenceScores = ScoreSentences(cleanedDiscussionChat, wordScores);


            //foreach (KeyValuePair<string, int> entry in sentenceScores)
            //{
            //    Debug.WriteLine(entry.Value + "\n");
            //}

            // Score and sort sentences by highest score, only keep top XX%
            string[] finalSentences = SortSentences(sentenceScores);


            Debug.WriteLine("<<<<<<<<<<Total Number of Sentences in the Summary : " + finalSentences.Length  + " >>>>>>>>>>>>>>");

            // Traverse the sentences array. If the sentence falls in
            // the top XX% (finalSentences), then add it to the summary
            string summary = BuildSummary(cleanedDiscussionChat, finalSentences, discussionChat);

            Debug.WriteLine("<<<<<<<<<<SUMMARY START>>>>>>>>>>>>>>");
            Debug.WriteLine(summary);
            Debug.WriteLine("<<<<<<<<<<SUMMARY END>>>>>>>>>>>>>>");

            SummariesGenerated++;

            return summary;
        }
    }
}
