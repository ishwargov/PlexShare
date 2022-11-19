/// <author>Hrishi Raaj Singh</author>
/// <summary>
///     It contains the SummaryPersistence class which implements the ISummaryPersistence
/// </summary> 

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Dashboard.Server.Persistence
{
    // Implementing the ISummaryPersistence Interface
    public class SummaryPersistence : ISummaryPersistence
    {
        public SummaryPersistence()
        {
            var configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folderPath = Path.Combine(configPath, "plexshare");
            string path = folderPath + "/Server/Persistence/PersistenceDownloads/SummaryDownloads/";
            SummaryPath = path;
            SummaryPath = SummaryPath + "Summary_" + DateTime.Now.ToString("MM/dd/yyyy");
        }

        public string SummaryPath { get; set; }
        /// <summary>
        /// Saves the summary of the session
        /// </summary>
        /// <param name="message">Takes a message string as input</param>
        /// <returns>True if saved successfully</returns>
        public bool SaveSummary(string message)
        {
            // Summary creation
            var sessionId1 = "Summary_of_the_session";
            var createText = "Summary : --------- " + Environment.NewLine + message + Environment.NewLine;


            try
            {
                bool t = Directory.Exists(SummaryPath);
                // If directory does not exists create that directory
                if (!Directory.Exists(SummaryPath)) Directory.CreateDirectory(SummaryPath);
                // Writing into the text file
                File.WriteAllText(Path.Combine(SummaryPath, sessionId1 + ".txt"), createText);
                Trace.WriteLine("Summary saved");
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
