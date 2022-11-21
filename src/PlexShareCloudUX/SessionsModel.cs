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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PlexShareCloudUX
{
    public class SessionsModel
    {
        string[] paths; //string array to store the read content of the url file. 
        private string SubmissionUrl;   //@"http://localhost:7213/api/submission";
        private string SessionUrl;  //@"http://localhost:7213/api/session";
        private FileDownloadApi fileDownloadApi;
        public SessionsModel()
        {
            paths = GetOfflinePaths("Urls.txt");
            SubmissionUrl = @paths[0];
            SessionUrl = @paths[1];
            Trace.WriteLine("[cloud] New FileDownloadApi instance created");
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
            Trace.WriteLine("[cloud] Retrieved all session details for " + userName);
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
            Trace.WriteLine("[cloud] Read the paths from the " + filename);
            return lines;
        }
    }
}
