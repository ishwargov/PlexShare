/// <author>Jayanth Kumar</author>
/// <created>10/11/2021</created>
/// <summary>
///		This is the unit testing file
///		for the porter stemmmer logic module and
///		tests the stemming functions and steps.
/// </summary>
using PlexShareDashboard.Dashboard.Server.Summary;
using System.Reflection;
using System.Diagnostics;

namespace PlexShareTests.DashboardTests.Summary
{
    public class PorterStemmerTests
    {
        public static IEnumerable<object[]> GetData()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            using (var stream = asm
                .GetManifestResourceStream("PlexShareTests.DashboardTests.Summary.Resources.data.csv"))
            {
                using (var reader = new StreamReader(stream))
                {
                    var line = reader.ReadLine();
                    Debug.WriteLine(line);

                    while (reader.ReadLine() != null)
                    {
                        var splittedLine = line.Split(',');
                        yield return new object[]
                        {
                            splittedLine.First(),
                            splittedLine.Skip(1).First()
                        };
                        line = reader.ReadLine();
                    }
                }
            }
        }

        // Batch test
        [Theory]
        [MemberData(nameof(GetData))]
        public void Stem_WithBatchData_StemsAllWordsCorrectly(string unstemmed, string expected)
        {
            var stemmer = new EnglishPorter2Stemmer();

            var stemmed = stemmer.Stem(unstemmed);

            Assert.Equal(expected, stemmed);
        }


        // LyEndingsTests
        [Fact]
        public void RemoveLySuffixes_EndingInEedlyAndInR1_ReplacesSuffixWithEe()
        {
            const string word = "inbreedly";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1BRemoveLySuffixes(word, stemmer.GetRegion1(word));

            // Assert
            Assert.Equal("inbree", actual);
        }

        [Fact]
        public void RemoveLySuffixes_EndingInEedAndInR1_ReplacesSuffixWithEe()
        {
            const string word = "inbreed";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1BRemoveLySuffixes(word, stemmer.GetRegion1(word));

            // Assert
            Assert.Equal("inbree", actual);
        }

        [Fact]
        public void RemoveLySuffixes_EndingInEdAndDoesNotContainVowel_LeavesWord()
        {
            const string word = "fred";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1BRemoveLySuffixes(word, stemmer.GetRegion1(word));

            // Assert
            Assert.Equal("fred", actual);
        }

        [Fact]
        public void RemoveLySuffixes_EndingInEdAndAtProceedsThat_ReplacesSuffixWithE()
        {
            const string word = "luxuriated";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1BRemoveLySuffixes(word, stemmer.GetRegion1(word));

            // Assert
            Assert.Equal("luxuriate", actual);
        }

        [Fact]
        public void RemoveLySuffixes_EndingInEdlyAndAtProceedsThat_ReplacesSuffixWithE()
        {
            const string word = "luxuriatedly";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1BRemoveLySuffixes(word, stemmer.GetRegion1(word));

            // Assert
            Assert.Equal("luxuriate", actual);
        }

        [Fact]
        public void RemoveLySuffixes_EndingInIngAndAtProceedsThat_ReplacesSuffixWithE()
        {
            const string word = "luxuriating";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1BRemoveLySuffixes(word, stemmer.GetRegion1(word));

            // Assert
            Assert.Equal("luxuriate", actual);
        }

        [Fact]
        public void RemoveLySuffixes_EndingInInglyAndAtProceedsThat_ReplacesSuffixWithE()
        {
            const string word = "luxuriated";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1BRemoveLySuffixes(word, stemmer.GetRegion1(word));

            // Assert
            Assert.Equal("luxuriate", actual);
        }

        [Fact]
        public void RemoveLySuffixes_EndingInIngAndDoubledLetterProceedsThat_RemovesDoubledLetter()
        {
            const string word = "hopping";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1BRemoveLySuffixes(word, stemmer.GetRegion1(word));

            // Assert
            Assert.Equal("hop", actual);
        }

        [Fact]
        public void RemoveLySuffixes_EndingInIngAndIsShortWord_ReplacesSuffixWithE()
        {
            const string word = "hoping";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1BRemoveLySuffixes(word, stemmer.GetRegion1(word));

            // Assert
            Assert.Equal("hope", actual);
        }

        // MarkingVowelsAsConsonants
        [Fact]
        public void MarkVowelsAsConsonants_WithInitialY_MarksYAsConsonant()
        {
            const string word = "youth";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.MarkYsAsConsonants(word);

            // Assert
            Assert.Equal("Youth", actual);
        }

        [Fact]
        public void MarkVowelsAsConsonants_WithYAfterVowel_MarksYAsConsonant()
        {
            const string word = "boy";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.MarkYsAsConsonants(word);

            // Assert
            Assert.Equal("boY", actual);
        }

        [Fact]
        public void MarkVowelsAsConsonants_WithYBetweenTwoVowels_MarksYAsConsonant()
        {
            const string word = "boyish";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.MarkYsAsConsonants(word);

            // Assert
            Assert.Equal("boYish", actual);
        }

        [Fact]
        public void MarkVowelsAsConsonants_WithYAfterConsonant_DoesNotMarkYAsConsonant()
        {
            const string word = "fly";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.MarkYsAsConsonants(word);

            // Assert
            Assert.Equal("fly", actual);
        }

        [Fact]
        public void MarkVowelsAsConsonants_WithVowelOnlyFollowingY_DoesNotMarkYAsConsonant()
        {
            const string word = "flying";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.MarkYsAsConsonants(word);

            // Assert
            Assert.Equal("flying", actual);
        }

        [Fact]
        public void MarkVowelsAsConsonants_WithNoVowelsButY_DoesNotMarkAnyYAsConsonant()
        {
            const string word = "syzygy";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.MarkYsAsConsonants(word);

            // Assert
            Assert.Equal("syzygy", actual);
        }

        [Fact]
        public void MarkVowelsAsConsonants_WithDoubledY_MarksFirstButNotSecondYAsConsonant()
        {
            const string word = "sayyid";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.MarkYsAsConsonants(word);

            // Assert
            Assert.Equal("saYyid", actual);
        }

        // RegionTests

        [Fact]
        public void GetRegion1_WithWordContainingRegion1AndRegion2_ProvidesCorrectRangeForRegion1()
        {
            // Arrange
            const string word = "beautiful";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.GetRegion1(word);

            // Assert
            Assert.Equal(5, actual);
        }

        [Fact]
        public void GetRegion2_WithWordContainingRegion1AndRegion2_ProvidesCorrectRangeForRegion2()
        {
            // Arrange
            const string word = "beautiful";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.GetRegion2(word);

            // Assert
            Assert.Equal(7, actual);
        }

        [Fact]
        public void GetRegion1_WithWordContainingOnlyRegion1_ProvidesCorrectRangeForRegion1()
        {
            // Arrange
            const string word = "beauty";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.GetRegion1(word);

            // Assert
            Assert.Equal(5, actual);
        }

        [Fact]
        public void GetRegion2_WithWordContainingOnlyRegion1_ProvidesRangeWithLength0()
        {
            // Arrange
            const string word = "beauty";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.GetRegion2(word);

            // Assert
            Assert.Equal(0, actual - word.Length);
        }

        [Fact]
        public void GetRegion1_WithWordContainingNeitherRegion_ProvidesRangeWithLength0()
        {
            // Arrange
            const string word = "beau";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.GetRegion1(word);

            // Assert
            Assert.Equal(0, actual - word.Length);
        }

        [Fact]
        public void GetRegion2_WithWordContainingNeitherRegion_ProvidesRangeWithLength0()
        {
            // Arrange
            const string word = "beau";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.GetRegion2(word);

            // Assert
            Assert.Equal(0, actual - word.Length);
        }

        // RemoveSPluralTests
        
        [Fact]
        public void RemoveSPluralSuffix_WithWordEndingInApostrophe_RemovesSuffix()
        {
            const string word = "holy'";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step0RemoveSPluralSuffix(word);

            // Assert
            Assert.Equal("holy", actual);
        }

        [Fact]
        public void RemoveSPluralSuffix_WithWordEndingInApostropheS_RemovesSuffix()
        {
            const string word = "holy's";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step0RemoveSPluralSuffix(word);

            // Assert
            Assert.Equal("holy", actual);
        }

        [Fact]
        public void RemoveSPluralSuffix_WithWordEndingInApostropheSApostrophe_RemovesSuffix()
        {
            const string word = "holy's'";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step0RemoveSPluralSuffix(word);

            // Assert
            Assert.Equal("holy", actual);
        }
        // ReplaceYSuffixWithITests
        [Fact]
        public void ReplaceYSuffix_PreceededByConsonant_ReplacesSuffixWithI()
        {
            const string word = "cry";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1CReplaceSuffixYWithIIfPreceededWithConsonant(word);

            // Assert
            Assert.Equal("cri", actual);
        }

        [Fact]
        public void ReplaceYSuffix_PreceededByConsonantAsFirstLetterOfWord_DoesNotReplaceSuffix()
        {
            const string word = "by";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1CReplaceSuffixYWithIIfPreceededWithConsonant(word);

            // Assert
            Assert.Equal("by", actual);
        }

        [Fact]
        public void ReplaceYSuffix_NotPreceededyConsonant_DoesNotReplaceSuffix()
        {
            const string word = "say";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1CReplaceSuffixYWithIIfPreceededWithConsonant(word);

            // Assert
            Assert.Equal("say", actual);
        }
        // ShortSyllableTests
        [Fact]
        public void EndInShortSyllable_TestingRap_IsCountedAsShort()
        {
            // Arrange
            const string word = "rap";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.EndsInShortSyllable(word);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void EndInShortSyllable_TestingTrap_IsCountedAsShort()
        {
            // Arrange
            const string word = "trap";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.EndsInShortSyllable(word);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void EndInShortSyllable_TestingEntrap_IsCountedAsShort()
        {
            // Arrange
            const string word = "entrap";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.EndsInShortSyllable(word);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void EndInShortSyllable_TestingOw_IsCountedAsShort()
        {
            // Arrange
            const string word = "ow";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.EndsInShortSyllable(word);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void EndInShortSyllable_TestingOn_IsCountedAsShort()
        {
            // Arrange
            const string word = "on";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.EndsInShortSyllable(word);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void EndInShortSyllable_TestingAt_IsCountedAsShort()
        {
            // Arrange
            const string word = "at";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.EndsInShortSyllable(word);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void EndInShortSyllable_TestingUproot_IsNotCountedAsShort()
        {
            // Arrange
            const string word = "uproot";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.EndsInShortSyllable(word);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void EndInShortSyllable_TestingBestow_IsCountedAsShort()
        {
            // Arrange
            const string word = "bestow";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.EndsInShortSyllable(word);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void EndInShortSyllable_TestingDisturb_IsCountedAsShort()
        {
            // Arrange
            const string word = "disturb";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.EndsInShortSyllable(word);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsShortWord_TestingBed_IsCountedAsShort()
        {
            // Arrange
            const string word = "bed";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.IsShortWord(word);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsShortWord_TestingShed_IsCountedAsShort()
        {
            // Arrange
            const string word = "shed";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.IsShortWord(word);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsShortWord_TestingShred_IsCountedAsShort()
        {
            // Arrange
            const string word = "shred";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.IsShortWord(word);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsShortWord_TestingBead_IsNotCountedAsShort()
        {
            // Arrange
            const string word = "bead";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.IsShortWord(word);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsShortWord_TestingEmbed_IsNotCountedAsShort()
        {
            // Arrange
            const string word = "embed";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.IsShortWord(word);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsShortWord_TestingBeds_IsNotCountedAsShort()
        {
            // Arrange
            const string word = "beds";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.IsShortWord(word);

            // Assert
            Assert.False(actual);
        }
        // sses_ied_ies_s_Tests
        [Fact]
        public void RemoveOtherSPluralSuffix_WithWordEndingInSses_ReplaceWithSs()
        {
            const string word = "assesses";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.Equal("assess", actual);
        }

        [Fact]
        public void RemoveOtherSPluralSuffix_WithLongWordEndingInIes_ReplaceWithI()
        {
            const string word = "cries";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.Equal("cri", actual);
        }

        [Fact]
        public void RemoveOtherSPluralSuffix_WithShortWordEndingInIes_ReplaceWithIe()
        {
            const string word = "ties";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.Equal("tie", actual);
        }

        [Fact]
        public void RemoveOtherSPluralSuffix_WithLongWordEndingInIed_ReplaceWithI()
        {
            const string word = "cried";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.Equal("cri", actual);
        }

        [Fact]
        public void RemoveOtherSPluralSuffix_WithShortWordEndingInIed_ReplaceWithIe()
        {
            const string word = "tied";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.Equal("tie", actual);
        }

        [Fact]
        public void RemoveOtherSPluralSuffix_EndingInSAndContainingAVowelRightBefore_LeavesTheS()
        {
            const string word = "gas";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.Equal("gas", actual);
        }

        [Fact]
        public void RemoveOtherSPluralSuffix_EndingInSAndContainingAVowelEarlierInWord_DeletesTheS()
        {
            const string word = "gaps";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.Equal("gap", actual);
        }

        [Fact]
        public void RemoveOtherSPluralSuffix_EndingInSAndContainingAVowelRightBeforeAndEarlierInWord_DeletesTheS()
        {
            const string word = "kiwis";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.Equal("kiwi", actual);
        }

        [Fact]
        public void RemoveOtherSPluralSuffix_EndingInSs_LeavesWordAlone()
        {
            const string word = "assess";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.Equal("assess", actual);
        }

        [Fact]
        public void RemoveOtherSPluralSuffix_EndingInUs_LeavesWordAlone()
        {
            const string word = "consensus";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.Equal("consensus", actual);
        }

    }
}
