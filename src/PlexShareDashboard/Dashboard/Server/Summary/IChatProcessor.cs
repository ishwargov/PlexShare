/// <author>Morem Jayanth Kumar</author>
/// <created>3/11/2022</created>
/// <summary>
///		This file is the main processor interface for summary generation
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareDashboard.Dashboard.Server.Summary
{
    public interface IChatProcessor
    {

        /// <summary>
        /// Loads stop words (e.g. a/an/the...) from enmbedded resource file for exclusion from analysis
        /// </summary>
        void LoadStopWords();


        /// <summary>
        ///     Removes punctuation and converts text to lower-case
        /// </summary>
        /// <param name="discussionChat">
        ///     List of chat messages
        /// </param>
        /// <returns>
        ///     List of strings representing clean chat messages
        /// </returns>
        List<(string, bool)> CleanChat(List<string> discussionChat);

        /// <summary>
        ///     Returns the words from the sentence
        /// </summary>
        /// <param name="chatMessage">
        ///     Takes in a single chat message
        /// </param>
        /// <returns>
        ///     List of strings containing the words of the sentence
        /// </returns>
        List<string> GetWordsFromSentence(string chatMessage);

        /// <summary>
        ///     Find the number of occurances of each non-stop word
        /// </summary>
        /// <param name="discussionChat">
        ///     Takes in the List of chat messages
        /// </param>
        /// <returns>
        ///     Map from words to their frequency of occurence
        /// </returns>
        Dictionary<string, int> CountWords(List<string> discussionChat);

        /// <summary>
        ///     Sorts all words that occur more than once by count descending
        /// </summary>
        /// <param name="wordCounts">
        ///     Takes in the map from words to their frequency of occurence
        /// </param>
        /// <returns>
        ///     List of words orderd in descending order of their frequencies
        /// </returns>
        string[] SortWords(Dictionary<string, int> wordCounts);

        /// <summary>
        ///     Assigns a score for each word based on what percentile it falls in when ordered by count descending
        /// </summary>
        /// <param name="sortedWords">
        ///     List of words orderd in descending order of their frequencies
        /// </param>
        /// <returns>
        ///     Map from words to their score
        /// </returns>
        Dictionary<string, int> ScoreWords(string[] sortedWords);

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
        Dictionary<string, int> ScoreSentences(List<string> cleanedDiscussionChat, Dictionary<string, int> wordScores);

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
        string[] SortSentences(Dictionary<string, int> sentenceScores, double percentToKeep);

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
        string BuildSummary(List<string> cleanedDiscussionChat, string[] finalSentences, List<string> discussionChat);


        /// <summary>
        ///     Call the ML Model for the sentiment score of the chat discussion
        /// </summary>
        /// <param name="cleanedDiscussionChat">
        ///     Cleaned Chat Messages
        /// </param>
        /// <returns>
        ///     Returns a string representing the sentiment score of the chat discussion
        /// </returns>
        string BuildSentimentScore(List<string> cleanedDiscussionChat);

        /// <summary>
        ///     Gemerate summary from chats
        /// </summary>
        /// <param name="discussionChat">
        ///     Original Chat Messages
        /// </param>
        /// <returns>
        ///     Summary of chat messages
        /// </returns>
        string Summarize(List<string> discussionChat);

    }
}
