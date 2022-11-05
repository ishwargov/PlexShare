using System;
using Xunit;
using Dashboard.Server.Persistence;
namespace PlexShareTests.DashboardTests.Persistence
{
    public class PersistenceUnitTest
    {
        [Fact]
        public void SaveSummaryTestOne()
        {
            var path = "../../../Persistence/PersistenceDownloads/SummaryDownloads/";
            var summary = "Unit Testing";
            var textToBeSaved = "Summary : --------- " + Environment.NewLine + summary + Environment.NewLine;
            var response = PersistenceFactory.GetSummaryPersistenceInstance().SaveSummary(summary, true);

            var textActuallySaved = File.ReadAllText(Path.Combine(path, response.FileName));
            File.Delete(Path.Combine(path, response.FileName));

            if (textToBeSaved == textActuallySaved)
            {
                Trace.WriteLine(textToBeSaved);
                Trace.WriteLine(textActuallySaved);
                Assert.IsTrue(response.IsSaved);
            }
            else{
                Trace.WriteLine("text not saved");
            }
        }
        [Fact]
        public void SaveSummaryNullPathExceptions()
        {
            var summary = "Unit Testing";
            var summaryPersister = PersistenceFactory.GetSummaryPersistenceInstance();
            summaryPersister.summaryPath = null;

            var response = summaryPersister.SaveSummary(summary, true);
            summaryPersister.summaryPath = "../../../Persistence/PersistenceDownloads/SummaryDownloads/";
            Assert.IsFalse(response.IsSaved);
        }
        [Fact]
        public void SaveTelemetryAnalysisTestOne()
        {
            var telemetryPersist = new TelemetryPersistence();
            var userCountVsTimeStamp = new Dictionary<DateTime, int>();
            var chatCountVsUserId = new Dictionary<int, int>();
            var insincereList = new List<int>();
            insincereList.Add(11);
            insincereList.Add(21);
            insincereList.Add(39);
            insincereList.Add(42);

            UserCountVsTimeStamp.Add(DateTime.Now, 12);
            UserCountVsTimeStamp.Add(DateTime.Now.AddHours(1), 15);
            UserCountVsTimeStamp.Add(DateTime.Now.AddHours(2), 18);

            chatCountVsUserId.Add(1000, 10);
            chatCountVsUserId.Add(2000, 20);
            chatCountVsUserId.Add(3000, 30);
            var sessionAnalytics = new SessionAnalytics();
            sessionAnalytics.chatCountForEachUser = chatCountVsUserId;
            sessionAnalytics.userCountAtAnyTime = UserCountVsTimeStamp;
            sessionAnalytics.insincereMembers = insincereList;

            // Actually Saving it
            var response = tp.Save(sessionAnalytics);

            var p1 = "../../../Persistence/PersistenceDownloads/TelemetryDownloads/TelemetryAnalytics/" +
                     response.FileName;

            // Check if such .png and .txt files are present or not
            var IsChatCountForUserSaved = File.Exists(Path.Combine(p1, "ChatCountVsUserID.png"));
            var IsInsincereMembersSaved = File.Exists(Path.Combine(p1, "insincereMembersList.txt"));
            var IsUserCountAtAnyTimeSaved = File.Exists(Path.Combine(p1, "UserCountVsTimeStamp.png"));
            File.Delete(Path.Combine(p1, "ChatCountVsUserID.png"));
            File.Delete(Path.Combine(p1, "insincereMembersList.txt"));
            File.Delete(Path.Combine(p1, "UserCountVsTimeStamp.png"));
            Assert.IsTrue(IsChatCountForUserSaved && IsInsincereMembersSaved && IsUserCountAtAnyTimeSaved);
                }
    }
}

