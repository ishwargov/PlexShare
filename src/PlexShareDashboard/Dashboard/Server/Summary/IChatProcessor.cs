using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareDashboard.Dashboard.Server.Summary
{
    public interface IChatProcessor
    {
        void LoadStopWords();

        List<(string, bool)> CleanChat(List<(string, bool)> discussionChat);

        List<string> GetWordsFromSentence(string chatMessage);

        Dictionary<string, int> CountWords(List<(string, bool)> discussionChat);

        string[] SortWords(Dictionary<string, int> wordCounts);

        Dictionary<string, int> ScoreWords(string[] sortedWords);

        Dictionary<string, int> ScoreSentences(List<(string, bool)> cleanedDiscussionChat, Dictionary<string, int> wordScores);

        string[] SortSentences(Dictionary<string, int> sentenceScores, double percentToKeep);

        string BuildSummary(List<(string, bool)> cleanedDiscussionChat, string[] finalSentences, List<(string, bool)> discussionChat);

        string Summarize(List<(string, bool)> discussionChat);

    }
}
