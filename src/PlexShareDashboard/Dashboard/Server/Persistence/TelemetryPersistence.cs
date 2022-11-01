/// <author>Parmanand Kumar</author>
/// <created>15/11/2021</created>
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




        /// <summary>
        ///     save the UserCountVsTimeStamp, UserIdVsChatCount, InsincereMember data as png after each session.
        /// </summary>
        /// <param name="sessionAnalyticsData"> takes sessionAnalyticsData from Telemetry. </param>
        public ResponseEntity Save(SessionAnalytics sessionAnalyticsData)
        {
            // create folder of name sessionId to store all analytics data

            var sessionId = string.Format("Analytics_{0:yyyy - MM - dd_hh - mm - ss - tt}", DateTime.Now);

            // Logic to plot and save UserCount Vs TimeStamp


            var t1 = UserCountVsTimeStamp_PlotUtil(sessionAnalyticsData.userCountVsTimeStamp, sessionId);

            // Logic to plot and save ChatCount Vs UserID

            var t2 = ChatCountVsUserID_PlotUtil(sessionAnalyticsData.chatCountForEachUser, sessionId);

            // Logic to save InsincereMembers list

            var t3 = InsincereMembers_SaveUtil(sessionAnalyticsData.listOfInSincereMembers, sessionId);

            //var l1 = new List<string>();
            //l1.Add(t1.FileName);
            //l1.Add(t2.FileName);
            //l1.Add(t3.FileName);

            var response = new ResponseEntity();
            response.IsSaved = t1.IsSaved & t2.IsSaved & t3.IsSaved;
            response.FileName = sessionId;
            //response.TelemetryAnalyticsFiles = l1;

            PersistenceFactory.lastSaveResponse = response;

            return response;
        }
        /// <summary>
        ///     save the InsincereMember data as png after each session.
        /// </summary>
        /// <param name="InsincereMembers"> takes InsincereMembers from Telemetry. </param>
        /// ///
        /// <param name="sessionId"> takes sessionId from Telemetry. </param>
        private ResponseEntity InsincereMembers_SaveUtil(List<int> InsincereMembers, string sessionId)
        {
            //Saving the Path to save find the XML file.
            var p1 = TelemetryAnalyticsPath + sessionId;
            var TextToSave = "Followings are UserIDs of InsincereMembers : " + Environment.NewLine;
            var response = new ResponseEntity();
            response.FileName = "insincereMembersList.txt";
            foreach (var w in InsincereMembers) TextToSave = TextToSave + w + Environment.NewLine;

            try
            {
                //Check if directory exists if not create a directory.
                if (!Directory.Exists(p1)) Directory.CreateDirectory(p1);

                //Writing the Text to Text file.
                File.WriteAllText(Path.Combine(p1, "insincereMembersList.txt"), TextToSave);
                Trace.WriteLine("insincereMembersList.txt saved Successfully!!");
                response.IsSaved = true;
                return response;
            }
            catch (Exception except)
            {
                Trace.WriteLine(except.Message);
                response.IsSaved = false;
                return response;
            }
        }

        /// <summary>
        ///     save the ChatCountForEachUser data as png after each session.
        /// </summary>
        /// <param name="ChatCountForEachUser"> takes ChatCountForEachUser from Telemetry. </param>
        /// ///
        /// <param name="sessionId"> takes sessionId from Telemetry. </param>
        private ResponseEntity ChatCountVsUserID_PlotUtil(Dictionary<int, int> ChatCountForEachUser, string sessionId)
        {
            var p1 = TelemetryAnalyticsPath + sessionId;
            // Converting the data Value of dictionary to Array, inorder to use ScottPlot library
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

            //Creating the Fixed labels
            var labels1 = lb1.ToArray();

            //Fixing the positions of X-labels
            var positions1 = pos1.ToArray();

            //Creating ScottPlot fig of mentioned dimension
            var plt1 = new Plot(600, 400);

            // Actually plotting the Bars
            plt1.AddBar(values1, positions1);

            //Adding the Xticks
            plt1.XTicks(positions1, labels1);
            plt1.SetAxisLimits(yMin: 0);

            //Fixing the Y spacing to 1, to enable ease of readability
            plt1.YAxis.ManualTickSpacing(1);

            // Giving names to X and Y axes
            plt1.XLabel("UserID");
            plt1.YLabel("ChatCount for any User");
            var response = new ResponseEntity();
            response.FileName = "ChatCountVsUserID.png";

            try
            {
                //Creating Directory if required and save
                if (!Directory.Exists(p1)) Directory.CreateDirectory(p1);
                plt1.SaveFig(Path.Combine(p1, "ChatCountVsUserID.png"));
                response.IsSaved = true;
                Trace.WriteLine("ChatCountVsUserID.png saved Successfully!!");
                return response;
            }
            catch (Exception except)
            {
                Trace.WriteLine(except.Message);
                response.IsSaved = false;
                return response;
            }
        }

        /// <summary>
        ///     save the UserCountAtAnyTime data as png after each session.
        /// </summary>
        /// <param name="UserCountAtAnyTime"> takes UserCountAtAnyTime from Telemetry. </param>
        /// ///
        /// <param name="sessionId"> takes sessionId from Telemetry. </param>
        private ResponseEntity UserCountVsTimeStamp_PlotUtil(Dictionary<DateTime, int> UserCountAtAnyTime,
            string sessionId)
        {
            // Converting the data Value of dictionary to Array, inorder to use ScottPlot library
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

            //Creating the Fixed labels
            var labels = lb.ToArray();

            //Fixing the positions of X-labels
            var positions = pos.ToArray();

            //Creating ScottPlot fig of mentioned dimension
            var plt = new Plot(600, 400);

            // Actually plotting the Bars
            var temp = plt.AddBar(values, positions);

            //Adding the Xticks
            plt.XTicks(positions, labels);

            //Fixing the Y spacing to 1, to enable ease of readability
            plt.YAxis.ManualTickSpacing(1);
            plt.SetAxisLimits(yMin: 0);

            // Changing BarColor to Green
            temp.FillColor = Color.Green;

            // Giving names to X and Y axes
            plt.XLabel("TimeStamp");
            plt.YLabel("UserCount At Any Instant");
            var response = new ResponseEntity();
            response.FileName = "UserCountVsTimeStamp.png";
            var p1 = TelemetryAnalyticsPath + sessionId;

            try
            {
                //Creating Directory if required and save
                if (!Directory.Exists(p1)) Directory.CreateDirectory(p1);
                plt.SaveFig(Path.Combine(p1, "UserCountVsTimeStamp.png"));
                Trace.WriteLine("UserCountVsTimeStamp.png saved Successfully!!");
                response.IsSaved = true;
                return response;
            }
            catch (Exception except)
            {
                Trace.WriteLine(except.Message);
                response.IsSaved = false;
                return response;
            }
        }
    }
}