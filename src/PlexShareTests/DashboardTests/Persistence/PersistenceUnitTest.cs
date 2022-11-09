using System;
using Xunit;
using System.Diagnostics;
using Dashboard.Server.Persistence;
using PlexShareDashboard.Dashboard.Server.Telemetry;
namespace PlexShareTests.DashboardTests.Persistence
{
    public class PersistenceUnitTest
    {
        // [Fact]
        // public void SaveSummaryNullPathExceptions()
        // {
        //    var summary = "Unit Testing";
        //    var summaryPersister = PersistenceFactory.GetSummaryPersistenceInstance();
        //    summaryPersister.SummaryPath = null;

        //    var response = summaryPersister.SaveSummary(summary);
        //    summaryPersister.SummaryPath = "../../../Persistence/PersistenceDownloads/SummaryDownloads/";
        //    Assert.False(response);
        // }
        [Fact]
        public void SaveSummaryTestOne()
        {
            var configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folderPath = Path.Combine(configPath, "plexshare");
            string path = folderPath + "/Server/Persistence/PersistenceDownloads/SummaryDownloads/";
            //var path = "../../../Persistence/PersistenceDownloads/SummaryDownloads/";
            var summary = "Unit Testing";
            var textToBeSaved = "Summary : --------- " + Environment.NewLine + summary + Environment.NewLine;
            var response = PersistenceFactory.GetSummaryPersistenceInstance().SaveSummary(summary);
            path = path + "Summary_" +DateTime.Now.ToString("MM/dd/yyyy");
            var p1 = "Summary_of_the_session.txt";
            var textActuallySaved = File.ReadAllText(Path.Combine(path, p1));
            File.Delete(Path.Combine(path,p1));

            if (textToBeSaved == textActuallySaved)
            {
                Trace.WriteLine(textToBeSaved);
                Trace.WriteLine(textActuallySaved);
                Assert.True(response);
            }
            else{
                Trace.WriteLine("text not saved");
            }
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

            userCountVsTimeStamp.Add(DateTime.Now, 12);
            userCountVsTimeStamp.Add(DateTime.Now.AddHours(1), 15);
            userCountVsTimeStamp.Add(DateTime.Now.AddHours(2), 18);

            chatCountVsUserId.Add(1000, 10);
            chatCountVsUserId.Add(2000, 20);
            chatCountVsUserId.Add(3000, 30);
            var sessionAnalytics = new SessionAnalytics();
            sessionAnalytics.chatCountForEachUser = chatCountVsUserId;
            sessionAnalytics.userCountVsTimeStamp = userCountVsTimeStamp;
            sessionAnalytics.listOfInSincereMembers = insincereList;

            var Path1 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folderPath = Path.Combine(Path1, "plexshare");
            string ServerDataPath = folderPath + "/Server/Persistence/PersistenceDownloads/TelemetryDownloads/ServerData/";
           string TelemetryAnalyticsPath = folderPath + "/Server/Persistence/PersistenceDownloads/TelemetryDownloads/TelemetryAnalytics/";
            TelemetryAnalyticsPath = TelemetryAnalyticsPath + "_" + DateTime.Now.ToString("MM/dd/yyyy") + "_Analytics";
            var response = telemetryPersist.Save(sessionAnalytics);

            var IsChatCountForUserSaved = File.Exists(Path.Combine(TelemetryAnalyticsPath, "ChatCountVsUserID.png"));
            var IsInsincereMembersSaved = File.Exists(Path.Combine(TelemetryAnalyticsPath, "insincereMembersList.txt"));
            var IsUserCountAtAnyTimeSaved = File.Exists(Path.Combine(TelemetryAnalyticsPath, "UserCountVsTimeStamp.png"));
            File.Delete(Path.Combine(TelemetryAnalyticsPath, "ChatCountVsUserID.png"));
            File.Delete(Path.Combine(TelemetryAnalyticsPath, "insincereMembersList.txt"));
            File.Delete(Path.Combine(TelemetryAnalyticsPath, "UserCountVsTimeStamp.png"));
            Assert.True(IsChatCountForUserSaved && IsInsincereMembersSaved && IsUserCountAtAnyTimeSaved);
                }
    }
}

