/******************************************************************************
 * Filename    = SubmissionModel.cs
 *
 * Author      = Polisetty Vamsi
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareCloudUX
 *
 * Description = Created Model for the downloading functionality. 
 *****************************************************************************/

using PlexShareCloud;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareCloudUX
{
    public class SubmissionsModel
    {
        //getting path from the files
        string[] paths;
        private string SubmissionUrl; //@"http://localhost:7213/api/submission";
        private string SessionUrl; //@"http://localhost:7213/api/session";
        private FileDownloadApi fileDownloadApi; //creating an instance of the FiledowloadApi.

        public SubmissionsModel() //constructor for the submissionmodel class. 
        {
            paths = GetOfflinePaths("OfflineSetup_Path.txt");
            SubmissionUrl = @paths[0];
            SessionUrl = @paths[1];
            fileDownloadApi = new FileDownloadApi(SessionUrl, SubmissionUrl);
        }

        public IReadOnlyList<SubmissionEntity>? SubmissionsList; //creating the submission list to store the details of type submission model. 
        
        /// <summary>
        /// uses the async function to reterieve the file from the cloud. 
        /// </summary>
        /// <param name="sessionId">Unique id for a session</param>
        /// <returns>Returns the submission entity for given session id</returns>
        public async Task<IReadOnlyList<SubmissionEntity>> GetSubmissions(string sessionId)
        {
            IReadOnlyList<SubmissionEntity>? getEntity = await fileDownloadApi.GetFilesBySessionIdAsync(sessionId);
            SubmissionsList = getEntity;
            return getEntity;
        }

        /// <summary>
        /// For getting the path of user with respect to their local system.. 
        /// </summary>
        /// <returns>Return a path to download folder</returns>
        public static string GetDownloadFolderPath() //Getting the path to folder where the downloads folder contains. 
        {
            /*if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                string pathDownload = System.IO.Path.Combine(GetHomePath(), "Downloads");
                return pathDownload;
            }*/

            return System.Convert.ToString(
                Microsoft.Win32.Registry.GetValue(
                     @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders"
                    , "{374DE290-123F-4565-9164-39C4925E467B}"
                    , String.Empty
                )
            );
        }

        /// <summary>
        /// Writes the file to the download folder. 
        /// </summary>
        /// <param name="num">Index in the submission list.</param>
        public void DownloadPdf(int num) //function for converting into pdf and write file at given download path. 
        {
            byte[] pdf = SubmissionsList[num].Pdf;
            string path = GetDownloadFolderPath() + "\\" + SubmissionsList[num].UserName + "_" + SubmissionsList[num].SessionId + ".pdf";
            File.WriteAllBytes(path, pdf);
        }

        /// <summary>
        /// this function will take the filename of file containing the urls of sumbision and session. 
        /// </summary>
        /// <param name="filename">Filename of the file we need to read.</param>
        /// <returns>Array of the urls</returns>
        public static string[] GetOfflinePaths(string filename)
        {
            string[] lines = FileRead.GetPaths(filename);
            return lines;
        }
    }
}
