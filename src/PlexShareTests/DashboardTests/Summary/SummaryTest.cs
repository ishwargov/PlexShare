using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using PlexShareContent.DataModels;

using Xunit;
using PlexShareDashboard.Dashboard.Server.Summary;

using PlexShareTests.DashboardTests.Summary ;

namespace PlexShareTests.DashboardTests.Summary
{
    public class SummaryFact
    {
        private ISummarizer _summarizer;

        /// <summary>
        ///     Setup the summarizer from the Summarizer factory
        /// </summary>
        public SummaryFact()
        {
            _summarizer = SummarizerFactory.GetSummarizer();
        }

        [Fact]
        public void LoadStopWord()
        {
            ChatProcessor.LoadStopWords();
        }

        /// <summary>
        ///     Null chat should give empty string
        /// </summary>
        [Fact]
        public void GetSummary_NullChatContext_EmptyString()
        {
            var chats = Utils.GetChatThread("Null Context");
            Assert.Equal(_summarizer.GetSummary(chats), "");
        }

        /// <summary>
        ///     Empty chat context should give empty string
        /// </summary>
        [Fact]
        public void GetSummary_EmptyChatContext_EmptyString()
        {
            var chats = Utils.GetChatThread("Empty chat context");
            Assert.Equal(_summarizer.GetSummary(chats), "");
        }

        /// <summary>
        ///     Empty chat context should give empty string
        /// </summary>
        [Fact]
        public void GetSummary_EmptyChats_EmptyString()
        {
            var chats = Utils.GetChatThread("Empty chats");
            Assert.Equal(_summarizer.GetSummary(chats), "");
        }

        /// <summary>
        ///     We only check on constant string since it is a probabilistic algorithm to give a string of constants.
        /// </summary>
        [Fact]
        public void GetSummary_ValidChatsFixedLength_ConstString()
        {
            var chats = Utils.GetChatThread("Fixed chat");
            var output = "CONST. \n\n";
            // removing the sentiment score from the summary
            var summary = _summarizer.GetSummary(chats);
            summary = summary.Split("The Sentiment Score for the current Chat").First();
            Debug.WriteLine(output);
            Assert.Equal(summary, output);
        }

        /// <summary>
        ///     On the general string we can only check fi the output summary is not null
        /// </summary>
        [Fact]
        public void GetSummary_ValidChatsSmall_NonEmptyString()
        {
            var chats = Utils.GetChatThread("Variable chat");
            var summary = _summarizer.GetSummary(chats);
            Assert.True(summary.Length > 0);
        }

        /// <summary>
        ///     On the general string we can only check fi the output summary is not null
        /// </summary>
        [Fact]
        public void GetSummary_ValidChatsGeneral_NonEmptyString()
        {
            var chats = Utils.GetChatThread("General chat");
            Assert.True(_summarizer.GetSummary(chats).Length > 0);
        }

        /// <summary>
        ///     Fact the save summary function on a general chat
        /// </summary>
        //[Fact]
        //public void SaveSummary_ValidChatsGeneral_ReturnsTrue()
        //{
        //    var chats = Utils.GetChatThread("General chat");
        //    Directory.Delete("../../../Persistence", true);
        //}

        [Theory]
        [InlineData("TEST", "test")]
        [InlineData("test", "test")]
        [InlineData("Test", "test")]
        public void CleanText_TextWithUpperCase_ReturnsLowerCaseText(string input, string expectedResult)
        {
            var result = ChatProcessor.CleanChat(new List<string>{input});

            Assert.Equal(result.First(),expectedResult);
        }

        [Theory]
        [InlineData("te-st", "test")]
        [InlineData("te—st", "test")]
        [InlineData("te.st", "test")]
        [InlineData("te...st", "test")]
        [InlineData("test's", "tests")]
        [InlineData("test,", "test")]
        [InlineData("\"test\"", "test")]
        public void CleanText_TextWithPuncuation_ReturnsTextWithoutPunctuation(string input, string expectedResult)
        {
            var result = ChatProcessor.CleanChat(new List<string>{input});

            Assert.Equal(result.First(),expectedResult);
        }


        [Fact]
        public void CountWords_ListOfNonStopWords_ReturnsProperCount()
        {
            ChatProcessor.LoadStopWords();

            var words = new List<string> {"test","test","trial"};
            var expectedWordCounts = new Dictionary<string, int>
            {
                { "test", 2 },
                { "trial", 1 }
            };

            var wordCounts = ChatProcessor.CountWords(words);

            Assert.Equal(wordCounts, expectedWordCounts);
        }


        [Fact]
        public void CountWords_ListContainsStopWords_ReturnsProperCountExcludingStopWords()
        {
            ChatProcessor.LoadStopWords();

            var words = new List<string>{"test","test","trial","a","an","the"};
            var expectedWordCounts = new Dictionary<string, int>
            {
                { "test", 2 },
                { "trial", 1 }
            };

            var result = ChatProcessor.CountWords(words);

            Assert.Equal(result, expectedWordCounts);
        }


        [Fact]
        public void SortWords_DictionaryOfWordCounts_ReturnsProperCount()
        {
            var wordCounts = new Dictionary<string, int>
            {
                { "test", 2 },
                { "challenge", 5},
                { "trial", 1 }
            };

            string[] expectedOrder = { "challenge", "test" };

            var result = ChatProcessor.SortWords(wordCounts);

            Assert.Equal(result, expectedOrder);
        }

        [Theory]
        [InlineData("a", 5)]
        [InlineData("b", 4)]
        [InlineData("e", 3)]
        [InlineData("i", 2)]
        [InlineData("m", 1)]
        [InlineData("q", 0)]
        [InlineData("r", 0)]
        [InlineData("s", 0)]
        [InlineData("t", 0)]
        public void ScoreWords_ArrayOfWords_WordsHaveProperScores(string input, int expectedResult)
        {
            string[] sortedWords = { "a", "b", "c", "d", "e",
                                     "f", "g", "h", "i", "j",
                                     "k", "l", "m", "n", "o",
                                     "p", "q", "r", "s", "t" };

            var wordScores = ChatProcessor.ScoreWords(sortedWords);

            int result;
            if (wordScores.ContainsKey(input))
                result = wordScores[input];
            else
                result = 0;

            Assert.Equal(result, expectedResult);
        }

        [Theory]
        [InlineData("This is a test.", 5)]
        [InlineData("The scores must match.", 7)]
        [InlineData("Is a this that the.", 0)]
        public void ScoreSentences_WordScores_SentenceScoreMatchesSumOfWordScores(string input, int expectedResult)
        {
            var sentences = new List<string> {"This is a test.","The scores must match.","Is a this that the."};

            var wordScores = new Dictionary<string, int>
            {
                {"test", 5 },
                {"scores", 4 },
                {"match", 3 }
            };

            var sentenceScores = ChatProcessor.ScoreSentences(sentences, wordScores);

            Assert.Equal(sentenceScores[input], expectedResult);
        }

        [Theory]
        [InlineData(0.2, new string[] { "correct" })]
        [InlineData(0.4, new string[] { "correct", "results" })]
        public void SortSentences_ScoredSentences_ResultsAreSortedAndContainProperAmountOfSentences(double input, string[] expectedResult)
        {
            var sentenceScores = new Dictionary<string, int>
            {
                {"test", 1 },
                {"scores", 2 },
                {"match", 3 },
                {"results", 4 },
                {"correct", 5 }
            };

            var result = ChatProcessor.SortSentences(sentenceScores, input);

            Assert.Equal(result, expectedResult);
        }


        [Fact]
        public void BuildSummary_SentenceList_SummaryContainsOnlyFinalSentences()
        {
            var rawSentences = new List<string> {"test","trial"};
            string[] finalSentences = { "test" };

            var result = ChatProcessor.BuildSummary(rawSentences, finalSentences, rawSentences);

            Assert.Equal(result, finalSentences[0] + ". \n\n");
        }
    }
}
