/// <author>Hrishi Raaj Singh Chauhan</author>
/// <created>1/11/2022</created>
/// <summary>
///     It contains the TelemetryPersistence class
///     It implements the ITelemetryPersistence interface functions.
/// </summary> 
/// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using OxyPlot.Wpf;
using PlexShareDashboard.Dashboard.Server.Telemetry;
//using PlexShareDashboard.Dashboard.Server.Telemetry;
using ScottPlot;

namespace Dashboard.Server.Persistence
{
    public class TelemetryPersistence : ITelemetryPersistence
    {
        public TelemetryPersistence()
        {
            var Path1 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folderPath = Path.Combine(Path1, "plexshare");
            ServerDataPath = folderPath + "/Server/Persistence/PersistenceDownloads/TelemetryDownloads/ServerData/";
            TelemetryAnalyticsPath = folderPath + "/Server/Persistence/PersistenceDownloads/TelemetryDownloads/TelemetryAnalytics/";
            TelemetryAnalyticsPath = TelemetryAnalyticsPath + DateTime.Now.ToString("MM/dd/yyyy");
        }

        public string ServerDataPath { get; set; }

        public string TelemetryAnalyticsPath { get; set; }




        public bool Save(SessionAnalytics sessionAnalyticsData)
        {
            var sessionId = string.Format("Analytics_{0:yyyy - MM - dd_hh - mm - ss - tt}", DateTime.Now);

            var t1 = UserCountVsTimeStamp_PlotUtil(sessionAnalyticsData.userCountVsTimeStamp, sessionId);

            var t2 = ChatCountVsUserID_PlotUtil(sessionAnalyticsData.chatCountForEachUser, sessionId);


            var t3 = InsincereMembers_SaveUtil(sessionAnalyticsData.listOfInSincereMembers, sessionId);
            bool isSaved = t1.IsSaved & t2.IsSaved & t3.IsSaved;
            return isSaved;
        }
        private bool InsincereMembers_SaveUtil(List<int> InsincereMembers, string sessionId)
        {
            var p1 = TelemetryAnalyticsPath + sessionId;
            var TextToSave = "Followings are UserIDs of InsincereMembers : " + Environment.NewLine;
            foreach (var w in InsincereMembers) TextToSave = TextToSave + w + Environment.NewLine;

            try
            {
                if (!Directory.Exists(p1)) Directory.CreateDirectory(p1);

                File.WriteAllText(Path.Combine(p1, "insincereMembersList.txt"), TextToSave);
                Trace.WriteLine("insincereMembersList.txt saved Successfully!!");
                bool isSaved = true;
                return isSaved;
            }
            catch (Exception except)
            {
                Trace.WriteLine(except.Message);
                bool isSaved = false;
                return isSaved;
            }
        }
       private bool ChatCountVsUserID_PlotUtil(Dictionary<int, int> ChatCountForEachUser, string sessionId)
        {
            var p1 = TelemetryAnalyticsPath + sessionId;
            var val1 = ChatCountForEachUser.Values.ToArray();
            var values1 = new double[val1.Length];
            for (var i = 0; i < val1.Length; i++) values1[i] = val1[i];
            var pos1 = new List<double>();
            var lb1 = new List<string>();

            var x1 = 0;
            foreach (var k1 in ChatCountForEachUser.Keys)
            {
                pos1.Add(x1);
                lb1.Add(k1.ToString());
                x1++;
            }

            var labels1 = lb1.ToArray();

            var positions1 = pos1.ToArray();

            var plt1 = new Plot(600, 400);

            plt1.AddBar(values1, positions1);

            plt1.XTicks(positions1, labels1);
            plt1.SetAxisLimits(yMin: 0);

            plt1.YAxis.ManualTickSpacing(1);

            plt1.XLabel("UserID");
            plt1.YLabel("ChatCount for any User");

            try
            {
                if (!Directory.Exists(p1)) Directory.CreateDirectory(p1);
                plt1.SaveFig(Path.Combine(p1, "ChatCountVsUserID.png"));
                bool isSaved = true;
                Trace.WriteLine("ChatCountVsUserID.png saved Successfully!!");
                return isSaved;
            }
            catch (Exception except)
            {
                Trace.WriteLine(except.Message);
                bool isSaved = false;
                return isSaved;
            }
        }
        private bool UserCountVsTimeStamp_PlotUtil(Dictionary<DateTime, int> UserCountAtAnyTime,
            string sessionId)
        {
            var val = UserCountAtAnyTime.Values.ToArray();
            var values = new double[val.Length];
            for (var i = 0; i < val.Length; i++) values[i] = val[i];
            var pos = new List<double>();
            var lb = new List<string>();
            var x = 0;
            foreach (var k in UserCountAtAnyTime.Keys)
            {
                pos.Add(x);
                lb.Add(k.ToString());
                x++;
            }

            var labels = lb.ToArray();

            var positions = pos.ToArray();

            var plt = new Plot(600, 400);

            var temp = plt.AddBar(values, positions);

            plt.XTicks(positions, labels);

            plt.YAxis.ManualTickSpacing(1);
            plt.SetAxisLimits(yMin: 0);

            temp.FillColor = Color.Green;

            plt.XLabel("TimeStamp");
            plt.YLabel("UserCount At Any Instant");
            var p1 = TelemetryAnalyticsPath + sessionId;
            try
            {
                if (!Directory.Exists(p1)) Directory.CreateDirectory(p1);
                plt.SaveFig(Path.Combine(p1, "UserCountVsTimeStamp.png"));
                Trace.WriteLine("UserCountVsTimeStamp.png saved Successfully!!");
                bool isSaved = true;
                return isSaved;
            }
            catch (Exception except)
            {
                Trace.WriteLine(except.Message);
                bool isSaved = false;
                return isSaved;
            }
        }
    }
}