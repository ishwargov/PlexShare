using System;
using Xunit;
using System.Diagnostics;
using Dashboard.Server.Persistence;
using PlexShareDashboard.Dashboard.Server.Telemetry;
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
            var response = PersistenceFactory.GetSummaryPersistenceInstance().SaveSummary(summary);
            var p1 = "Summary" + "_"+ DateTime.Now.ToString("MM/dd/yyyy");
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
        public void SaveSummaryNullPathExceptions()
        {
            var summary = "Unit Testing";
            var summaryPersister = PersistenceFactory.GetSummaryPersistenceInstance();
            summaryPersister.summaryPath = null;

            var response = summaryPersister.SaveSummary(summary);
            summaryPersister.summaryPath = "../../../Persistence/PersistenceDownloads/SummaryDownloads/";
            Assert.False(response);
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


            var response = telemetryPersist.Save(sessionAnalytics);

            var p1 = "../../../Persistence/PersistenceDownloads/TelemetryDownloads/TelemetryAnalytics/" + "_"+ DateTime.Now.ToString("MM/dd/yyyy")+"_";

            var IsChatCountForUserSaved = File.Exists(Path.Combine(p1, "ChatCountVsUserID.png"));
            var IsInsincereMembersSaved = File.Exists(Path.Combine(p1, "insincereMembersList.txt"));
            var IsUserCountAtAnyTimeSaved = File.Exists(Path.Combine(p1, "UserCountVsTimeStamp.png"));
            File.Delete(Path.Combine(p1, "ChatCountVsUserID.png"));
            File.Delete(Path.Combine(p1, "insincereMembersList.txt"));
            File.Delete(Path.Combine(p1, "UserCountVsTimeStamp.png"));
            Assert.True(IsChatCountForUserSaved && IsInsincereMembersSaved && IsUserCountAtAnyTimeSaved);
                }
    }
}

