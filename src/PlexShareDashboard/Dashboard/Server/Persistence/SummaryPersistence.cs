/// <author>Parmanand Kumar</author>
/// <created>15/11/2021</created>
/// <summary>
///     It contains the SummaryPersistence class
///     It implements the ISummaryPersistence interface functions.
/// </summary> 

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Dashboard.Server.Persistence
{
    public class SummaryPersistence : ISummaryPersistence
    {
        public SummaryPersistence()
        {
            var configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folderPath = Path.Combine(configPath, "plexshare");
            string path = folderPath + "/Server/Persistence/PersistenceDownloads/SummaryDownloads/";
            path = path + DateTime.Now.ToString("MM/dd/yyyy");
            summaryPath = path;
        }

        public string summaryPath { get; set; }

        public bool SaveSummary(string message)
        {
            var sessionId1 = string.Format("Summary_{0:yyyy - MM - dd_hh - mm - ss - tt}", DateTime.Now);
            var createText = "Summary : --------- " + Environment.NewLine + message + Environment.NewLine;


            try
            {
                if (!Directory.Exists(summaryPath)) Directory.CreateDirectory(summaryPath);

                File.WriteAllText(Path.Combine(summaryPath, sessionId1 + ".txt"), createText);
                Trace.WriteLine("Summary saved Suceessfully!!");
                bool isSaved = true;
                return isSaved;
            }
            catch (Exception except)
            {
                Trace.WriteLine(except.Message);
                bool isSaved = true;
                return isSaved;
            }
        }
    }
}