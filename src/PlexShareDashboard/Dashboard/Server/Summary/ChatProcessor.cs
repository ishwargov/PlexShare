/// <author>Morem Jayanth Kumar</author>
/// <created>3/11/2022</created>
/// <summary>
///		This file is the main processor for summary generation and sentiment analysing
/// </summary>
using ScottPlot.Palettes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;
using System.Reflection;

using PlexShareContent;
using PlexShareDashboard.Dashboard.Server.Summary;
using PlexShareDashboard.Dashboard.Server.Telemetry;

using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

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
        private static MLContext mlContext;
        private static SentimentAnalyzer sentimentAnalyzer;
        ITransformer mlModel;
        private static PredictionEngine<ModelInput,ModelOutput> predEngine;
        private const string MODEL_FILEPATH = @"../../../../PlexShareDashboard/Dashboard/Server/Summary/Resources/MLModel.zip";

        public ChatProcessor()
        {
            mlContext = new MLContext();
            _stemmer = new EnglishPorter2Stemmer();
            sentimentAnalyzer = new SentimentAnalyzer();
            mlModel = mlContext.Model.Load(SentimentAnalyzer.GetAbsolutePath(MODEL_FILEPATH), out DataViewSchema inputSchema);
            predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        }


        /// <summary>
        /// Loads stop words (e.g. a/an/the...) from enmbedded resource file for exclusion from analysis
        /// </summary>
        public static void LoadStopWords()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            StreamReader reader = new StreamReader(asm.GetManifestResourceStream("PlexShareDashboard.Dashboard.Server.Summary.Resources.stop_words.txt"));
            string stopwords = reader.ReadToEnd();
            StopWords = new List<string>(stopwords.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
        }

        /// <summary>
        ///     Removes punctuation and converts text to lower-case
        /// </summary>
        /// <param name="discussionChat">
        ///     List of chat messages
        /// </param>
        /// <returns>
        ///     List of strings representing clean chat messages
        /// </returns>
        public static List<string> CleanChat(List<string> discussionChat)
        {
            List<string> cleanedDiscussionChat = new List<string>();
            foreach (var discussionChatItem in discussionChat)
            {
                string textWithoutPunctuation = Regex.Replace(discussionChatItem, @"(\p{P})", "").ToLower();
                if (textWithoutPunctuation.Length == 0) continue;
                cleanedDiscussionChat.Add((textWithoutPunctuation));
            }
            return cleanedDiscussionChat;
        }

        /// <summary>
        ///     Returns the words from the sentence
        /// </summary>
        /// <param name="chatMessage">
        ///     Takes in a single chat message
        /// </param>
        /// <returns>
        ///     List of strings containing the words of the sentence
        /// </returns>
        public static List<string> GetWordsFromSentence(string chatMessage)
        {
            List<string> words = new List<string>(chatMessage.Split());
            words.RemoveAll(word => word == "");
            for (int i = 0; i < words.Count(); i++)
                words[i] = words[i].Replace(".", "");
            return words;
        }
        /// <summary>
        ///     Find the number of occurances of each non-stop word
        /// </summary>
        /// <param name="discussionChat">
        ///     Takes in the List of chat messages
        /// </param>
        /// <returns>
        ///     Map from words to their frequency of occurence
        /// </returns>
        public static Dictionary<string, int> CountWords(List<string> discussionChat)
        {
            var wordCounts = new Dictionary<string, int>();

            foreach (var discussionChatItem in discussionChat)
            {
                List<string> words = GetWordsFromSentence(discussionChatItem);
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
        ///     Sorts all words that occur more than once by count descending
        /// </summary>
        /// <param name="wordCounts">
        ///     Takes in the map from words to their frequency of occurence
        /// </param>
        /// <returns>
        ///     List of words orderd in descending order of their frequencies
        /// </returns>
        public static string[] SortWords(Dictionary<string, int> wordCounts)
        {
            var sortedWords = from word in wordCounts
                              where word.Value > 1
                              orderby word.Value descending
                              select word.Key;

            return sortedWords.ToArray();
        }

        /// <summary>
        ///     Assigns a score for each word based on what percentile it falls in when ordered by count descending
        /// </summary>
        /// <param name="sortedWords">
        ///     List of words orderd in descending order of their frequencies
        /// </param>
        /// <returns>
        ///     Map from words to their score
        /// </returns>
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
        ///     Scores sentences
        /// </summary>
        /// <param name="cleanedDiscussionChat">
        ///     Cleaned Chat Messages
        /// </param>
        /// <param name="wordScores">
        ///     Map from words to thier scores
        /// </param>
        /// <returns>
        ///     Map from Chat Sentences to their scores
        /// </returns>
        public static Dictionary<string, int> ScoreSentences(List<string> cleanedDiscussionChat, Dictionary<string, int> wordScores)
        {
            var sentenceScores = new Dictionary<string, int>();

            foreach (var chatMessage in cleanedDiscussionChat)
            {
                // Separate words
                List<string> cleanSentenceWords = GetWordsFromSentence(chatMessage);

                int score = 0;

                // Tally the score of sentence by summing scores of each word in sentence
                foreach (string word in cleanSentenceWords)
                {
                    string stemOfWord = _stemmer.Stem(word);
                    if(wordScores.ContainsKey(word))
                        score += wordScores[word];
                    else if (wordScores.ContainsKey(stemOfWord))
                        score += wordScores[stemOfWord];
                }
                // Make sure no duplicate additions are attempted, e.g. two emtpy strings
                if (!sentenceScores.ContainsKey(chatMessage))
                    sentenceScores.Add(chatMessage, score);
            }
            return sentenceScores;
        }

        /// <summary>
        ///     Returns only the top XX% of sentences specified by percentToKeep
        /// </summary>
        /// <param name="sentenceScores">
        ///     Map from Chat Sentences to their scores
        /// </param>
        /// <param name="percentToKeep">
        ///     Percentage of Messages to be filtered
        /// </param>
        /// <returns>
        ///     List of sentences with score greater than threshold
        /// </returns>
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
        ///     Builds an ordered summary from the final sentences
        /// </summary>
        /// <param name="cleanedDiscussionChat">
        ///     Cleaned Chat Messages
        /// </param>
        /// <param name="finalSentences">
        ///     sentences after processing
        /// </param>
        /// <param name="discussionChat">
        ///     Orginal Chat Messages
        /// </param>
        /// <returns>
        ///     Returns the summary of original messages
        /// </returns>
        public static string BuildSummary(List<string> cleanedDiscussionChat, string[] finalSentences, List<string> discussionChat)
        {
            StringBuilder summary = new StringBuilder();
            Dictionary<string, bool> visitedFinalSentence = new();
            for(int i= 0;i < cleanedDiscussionChat.Count;i++)
            {
                if (finalSentences.Contains(cleanedDiscussionChat[i]) && !visitedFinalSentence.ContainsKey(discussionChat[i]))
                {
                    summary.Append(discussionChat[i]);
                    summary.Append(". \n\n");

                    visitedFinalSentence[discussionChat[i]] = true;
                }
            }

            return summary.ToString();
        }
        /// <summary>
        ///     Call the ML Model for the sentiment score of the chat discussion
        /// </summary>
        /// <param name="cleanedDiscussionChat">
        ///     Cleaned Chat Messages
        /// </param>
        /// <returns>
        ///     Returns a string representing the sentiment score of the chat discussion
        /// </returns>
        public static string BuildSentimentScore(List<string> cleanedDiscussionChat)
        {
            string singleStringChatMessages = String.Join(". ", cleanedDiscussionChat.ToArray());
            if (singleStringChatMessages != "")
            {
                ModelInput singleStringChatMessagesModelInput = SentimentAnalyzer.CreateSingleDataSample(singleStringChatMessages);
                ModelOutput predictionResult = predEngine.Predict(singleStringChatMessagesModelInput);
                Debug.WriteLine($"Single Prediction --> Prediction for '{singleStringChatMessagesModelInput}' was PositiveSentiment = {predictionResult.Prediction}");
                Debug.WriteLine("=============== End of process, hit any key to finish ===============");
                return $"The Sentiment Score for the current Chat is '{predictionResult.Score * 10}'";
            }
            return "";
        }

        /// <summary>
        ///     Gemerate summary from chats
        /// </summary>
        /// <param name="discussionChat">
        ///     Original Chat Messages
        /// </param>
        /// <returns>
        ///     Summary of chat messages
        /// </returns>
        public string Summarize(List<string> discussionChat)
        {
            // Load stop words if a summary has not been generated yet
            if (SummariesGenerated == 0)
            {
                LoadStopWords();
            }
            // Remove punctuation and case
            List<string> cleanedDiscussionChat = CleanChat(discussionChat);

            // Tally up word counts for non-stop words
            Dictionary<string, int> wordCounts = CountWords(cleanedDiscussionChat);

            // Sort words that occur more than once by word count, descending
            string[] sortedWords = SortWords(wordCounts);

            // Assign a point value to each word based on its percentile
            Dictionary<string, int> wordScores = ScoreWords(sortedWords);

            // Score each individual sentence
            Dictionary<string, int> sentenceScores = ScoreSentences(cleanedDiscussionChat, wordScores);

            // Score and sort sentences by highest score, only keep top XX%
            string[] finalSentences = SortSentences(sentenceScores);

            // Traverse the sentences array. If the sentence falls in
            // the top XX% (finalSentences), then add it to the summary
            string summary = BuildSummary(cleanedDiscussionChat, finalSentences, discussionChat);

            // Get the sentiment score
            summary += BuildSentimentScore(cleanedDiscussionChat);

            SummariesGenerated++;

            return summary;
        }
    }
}
