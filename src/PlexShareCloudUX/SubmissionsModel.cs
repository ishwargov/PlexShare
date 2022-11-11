/******************************************************************************
 * Filename    = SubmissionModel.cs
 *
 * Author      = Polisetty Vamsi
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareCloud
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
        private const string SubmissionUrl = @"http://localhost:7213/api/submission";
        private const string SessionUrl = @"http://localhost:7213/api/session";
        private FileDownloadApi fileDownloadApi;
        public SubmissionsModel()
        {
            fileDownloadApi = new FileDownloadApi(SessionUrl, SubmissionUrl);
        }

        public IReadOnlyList<SubmissionEntity>? SubmissionsList;

        public async Task<IReadOnlyList<SubmissionEntity>> GetSubmissions(string sessionId)
        {
            IReadOnlyList<SubmissionEntity>? getEntity = await fileDownloadApi.GetFilesBySessionIdAsync("sessionId");
            SubmissionsList = getEntity;
            return getEntity;
        }
        public static string GetDownloadFolderPath()
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

        public void DownloadPdf(int num)
        {
            byte[] pdf = SubmissionsList[num].Pdf;

            //var path = Process.Start("shell:Downloads");

            string path = GetDownloadFolderPath();
            File.WriteAllBytes(path, pdf);
        }
    }
}
