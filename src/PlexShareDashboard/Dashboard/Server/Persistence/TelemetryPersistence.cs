/// <author>Hrishi Raaj Singh</author>
///<summary>
///     It contains the TelemetryPersistence class which implements the ITelemetryPersistence
///</summary>
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



        /// <summary>
        /// Save the plot of UserNameVsChatCount UserCountVSTimeStamp and also save the XML file containing the info of the session like most engaged person etc.
        /// </summary>
        /// <param name="sessionAnalyticsData">Takes sessionAnaytics as input from the Telemetry</param>
        /// <returns>True if saved successfully</returns>
        public bool Save(SessionAnalytics sessionAnalyticsData)
        {
            var sessionId = "Analytics";
            // To plot the UserCountVSTimeStamp
            bool t1 = UserCountVsTimeStamp(sessionAnalyticsData.userCountVsTimeStamp, sessionId);
            // To plot the UserNameVSChatCount
            bool t2 = ChatCountVsUserName(sessionAnalyticsData.userNameVsChatCount, sessionId);
            // TO save the xml file containing information of the session
            bool t3 = XML_save(sessionAnalyticsData,sessionId);
            bool isSaved = t1 & t2 & t3;
            return isSaved;
        }
        /// <summary>
        /// Saves the XML file of the session containing the session information
        /// </summary>
        /// <param name="sessionanalytics">Takes session data to be saved in the file</param>
        /// <returns>returns true if saved successfully</returns>
        private bool XML_save(SessionAnalytics sessionanalytics,string sessionId)
        {
            var score = "0";
            if ( sessionanalytics.sessionSummary != null && sessionanalytics.sessionSummary.score != null) 
                score = sessionanalytics.sessionSummary.score.ToString();
            var path = TelemetryAnalyticsPath + sessionId;
            var mostengaged = "None";
            var temp = 0;
            var maxcount = 0;
            // finding maximum user count in the session at a time
            var val = sessionanalytics.userCountVsTimeStamp.Values.ToArray();
            if (val != null)
            {
                for (var i = 0; i < val.Length; i++)
                {
                    if (maxcount < val[i]) maxcount = val[i];
                }
            }
            // finding most engaged user
            var val1 = sessionanalytics.userNameVsChatCount;
            if (val1 != null)
            {
                foreach (var i in val1)
                {
                    if (i.Value > temp)
                    {
                        temp = i.Value;
                        mostengaged = i.Key;
                    }
                }
            }
            try
            {

                // if directory does not exist create the directory
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                // Writing the XML file
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
        /// <summary>
        /// Plotting ChatCountVsUserName
        /// </summary>
        /// <param name="ChatCountForEachUser"> using session data from Telemetry </param>
        /// <returns>returns true if saved successfully</returns>

        private bool ChatCountVsUserName(Dictionary<string, int> ChatCountForEachUser, string sessionId)
        {
            var p1 = TelemetryAnalyticsPath + sessionId;
            if(ChatCountForEachUser == null)
            {
                Trace.WriteLine("null exception at chat count");
                return false;
            }
            // converting data into the format required to save using scottplot
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
            // create labels
            var labels1 = lb1.ToArray();
            // fixing position of the x axis
            var positions1 = pos1.ToArray();
            // create a plot of the given dimension
            var plt1 = new Plot(600, 400);

            plt1.AddBar(values1, positions1);

            plt1.XTicks(positions1, labels1);
            plt1.SetAxisLimits(yMin: 0);

            plt1.YAxis.ManualTickSpacing(1);

            plt1.XLabel("UserName");
            plt1.YLabel("ChatCount for any User");

            try
            {
                // creating directory if not exist already
                if (!Directory.Exists(p1)) Directory.CreateDirectory(p1);
                plt1.SaveFig(Path.Combine(p1, "ChatCountVsUserID.png"));
                bool isSaved = true;
                Trace.WriteLine("ChatCountVsUserID.png saved");
                return isSaved;
            }
            catch (Exception except)
            {
                Trace.WriteLine(except.Message);
                bool isSaved = false;
                return isSaved;
            }
        }
        /// <summary>
        /// Plotting UserCountVsTimeStamp
        /// </summary>
        /// <param name="UserCountAtAnyTime"> using session data from Telemetry </param>
        /// <returns>returns true if saved successfully</returns>

        private bool UserCountVsTimeStamp(Dictionary<DateTime, int> UserCountAtAnyTime,
            string sessionId)
        {
            // converting data into the format required by the scottplot
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

            temp.FillColor = Color.Yellow;

            plt.XLabel("TimeStamp");
            plt.YLabel("UserCount At Any Instant");
            var p1 = TelemetryAnalyticsPath + sessionId;
            try
            {
                // creating a Directory if path already does not exist
                if (!Directory.Exists(p1)) Directory.CreateDirectory(p1);
                plt.SaveFig(Path.Combine(p1, "UserCountVsTimeStamp.png"));
                Trace.WriteLine("UserCountVsTimeStamp.png saved");
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