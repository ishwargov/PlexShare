using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using OxyPlot.Wpf;
using PlexShareDashboard.Dashboard.Server.Telemetry;
using ScottPlot;
using SharpDX.Text;
using System.Xml;
using System.Xml.Linq;

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
            TelemetryAnalyticsPath = TelemetryAnalyticsPath + "_" + DateTime.Now.ToString("MM/dd/yyyy") + "_";

        }

        public string ServerDataPath { get; set; }

        public string TelemetryAnalyticsPath { get; set; }




        public bool Save(SessionAnalytics sessionAnalyticsData)
        {
            var sessionId = "Analytics";
            bool t1 = UserCountVsTimeStamp_PlotUtil(sessionAnalyticsData.userCountVsTimeStamp, sessionId);

            bool t2 = ChatCountVsUserName_PlotUtil(sessionAnalyticsData.userNameVsChatCount, sessionId);
            bool t3 = XML_save(sessionAnalyticsData,sessionId);
            bool isSaved = t1 & t2 & t3;
            return isSaved;
        }
        private bool XML_save(SessionAnalytics sessionanalytics,string sessionId)
        {
            var score = "0";
            if ( sessionanalytics.sessionSummary != null && sessionanalytics.sessionSummary.score != null) 
                score = sessionanalytics.sessionSummary.score.ToString();
            var path = TelemetryAnalyticsPath + sessionId;
            var mostengaged = "None";
            var temp = 0;
            var maxcount = 0;
            var val = sessionanalytics.userCountVsTimeStamp.Values.ToArray();
            if (val != null)
            {
                for (var i = 0; i < val.Length; i++)
                {
                    if (maxcount < val[i]) maxcount = val[i];
                }
            }
            var val1 = sessionanalytics.userNameVsChatCount;
            if (val1 != null)
            {
                foreach (var i in val1)
                {
                    if (i.Value < temp)
                    {
                        temp = i.Value;
                        mostengaged = i.Key;
                    }
                }
            }
            try
            {

                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    XmlTextWriter xwwrite = new XmlTextWriter(Path.Combine(path,"serverData.xml"), Encoding.UTF8);
                    xwwrite.Formatting = Formatting.Indented;
                    xwwrite.WriteStartElement("SessionTime");
                    xwwrite.WriteString(DateTime.Now.ToString());
                    xwwrite.WriteStartElement("SessionScore");
                    xwwrite.WriteString(score);
                    xwwrite.WriteEndElement();
                    xwwrite.WriteStartElement("MaximumUserCount");
                    xwwrite.WriteString(maxcount.ToString());
                    xwwrite.WriteEndElement();
                    xwwrite.WriteStartElement("MostEngagedUser");
                    xwwrite.WriteString(mostengaged);
                    xwwrite.WriteEndElement();
                    xwwrite.WriteEndElement();
                    xwwrite.Close();

                    return true;
                

            }
            catch(Exception except) {
                Trace.WriteLine(except.Message);
                return false;
            }

        }
       private bool ChatCountVsUserName_PlotUtil(Dictionary<string, int> ChatCountForEachUser, string sessionId)
        {
            var p1 = TelemetryAnalyticsPath + sessionId;
            if(ChatCountForEachUser == null)
            {
                Trace.WriteLine("null exception at chat count");
                return false;
            }
            var val1 = ChatCountForEachUser.Values.ToArray();
            var values1 = new double[val1.Length];
            int ik = 0;
            foreach (var i in ChatCountForEachUser) {
                values1[ik] = i.Value;
                ik++;
            }
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

            plt1.XLabel("UserName");
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