//<author>Hrishi Raaj Singh Chauhan</author>
using Dashboard.Server.Persistence;
using PlexShareDashboard.Dashboard.Server.Telemetry;
using System.Diagnostics;
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
        /// <summary>
        /// Function to check if Summary is getting saved properly.
        /// </summary>
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
            path = path + "Summary_" + DateTime.Now.ToString("MM/dd/yyyy");
            var p1 = "Summary_of_the_session.txt";
            var textActuallySaved = File.ReadAllText(Path.Combine(path, p1));
            File.Delete(Path.Combine(path, p1));

            if (textToBeSaved == textActuallySaved)
            {
                Trace.WriteLine(textToBeSaved);
                Trace.WriteLine(textActuallySaved);
                Assert.True(response);
            }
            else
            {
                Trace.WriteLine("text not saved");
            }
        }
        /// <summary>
        /// Function to check if SessionAnalytics is getting saved properly.
        /// </summary>

        [Fact]
        public void SaveTelemetryAnalysisTestOne()
        {

            var telemetryPersist = new TelemetryPersistence();
            var userCountVsTimeStamp = new Dictionary<DateTime, int>();
            var userNameVsChatCount = new Dictionary<string, int>();

            userCountVsTimeStamp.Add(DateTime.Now, 12);
            userCountVsTimeStamp.Add(DateTime.Now.AddHours(1), 15);
            userCountVsTimeStamp.Add(DateTime.Now.AddHours(2), 18);

            userNameVsChatCount.Add("Hrishi", 10);
            userNameVsChatCount.Add("Rupesh", 20);
            userNameVsChatCount.Add("Saurabh", 30);
            var sessionAnalytics = new SessionAnalytics();
            sessionAnalytics.userNameVsChatCount = userNameVsChatCount;
            sessionAnalytics.userCountVsTimeStamp = userCountVsTimeStamp;

            var Path1 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folderPath = Path.Combine(Path1, "plexshare");
            string ServerDataPath = folderPath + "/Server/Persistence/PersistenceDownloads/TelemetryDownloads/ServerData/";
            string TelemetryAnalyticsPath = folderPath + "/Server/Persistence/PersistenceDownloads/TelemetryDownloads/TelemetryAnalytics/";
            TelemetryAnalyticsPath = TelemetryAnalyticsPath + "_" + DateTime.Now.ToString("MM/dd/yyyy") + "_Analytics";
            var response = telemetryPersist.Save(sessionAnalytics);

            var IsChatCountForUserSaved = File.Exists(Path.Combine(TelemetryAnalyticsPath, "ChatCountVsUserID.png"));
            var IsInsincereMembersSaved = File.Exists(Path.Combine(TelemetryAnalyticsPath, "serverData.xml"));
            var IsUserCountAtAnyTimeSaved = File.Exists(Path.Combine(TelemetryAnalyticsPath, "UserCountVsTimeStamp.png"));
            File.Delete(Path.Combine(TelemetryAnalyticsPath, "ChatCountVsUserID.png"));
            File.Delete(Path.Combine(TelemetryAnalyticsPath, "serverData.xml"));
            File.Delete(Path.Combine(TelemetryAnalyticsPath, "UserCountVsTimeStamp.png"));
            Assert.True(IsChatCountForUserSaved && IsInsincereMembersSaved && IsUserCountAtAnyTimeSaved);
        }
    }
}
