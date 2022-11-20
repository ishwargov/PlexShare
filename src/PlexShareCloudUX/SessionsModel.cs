/******************************************************************************
 * Filename    = SessionModel.cs
 *
 * Author      = Polisetty Vamsi
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareCloudUx
 *
 * Description = Model logic for the sessions. 
 *****************************************************************************/

using PlexShareCloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareCloudUX
{
    internal class SessionsModel
    {
        string[] paths;
        private string SubmissionUrl;   //@"http://localhost:7213/api/submission";
        private string SessionUrl;  //@"http://localhost:7213/api/session";
        private FileDownloadApi fileDownloadApi;
        public SessionsModel()
        {
            paths = GetOfflinePaths("OfflineSetup_Path.txt");
            SubmissionUrl = @paths[0];
            SessionUrl = @paths[1];
            fileDownloadApi = new FileDownloadApi(SessionUrl, SubmissionUrl);
        }
        /// <summary>
        /// Takes the username and retreive the information from the session table. 
        /// </summary>
        /// <param name="userName">Username of the user we consider</param>
        /// <returns>will return the session entity for given username.</returns>
        public async Task<IReadOnlyList<SessionEntity>> GetSessionsDetails(string userName)
        {
            IReadOnlyList<SessionEntity>? getEntity = await fileDownloadApi.GetSessionsByUserAsync(userName);
            return getEntity;
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
