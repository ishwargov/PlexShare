/******************************************************************************
 * Filename    = CurrentSubmissionModel.cs
 *
 * Author      = Yagnesh Katragadda
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareCloudUx
 *
 * Description = Consists of function to retrieve submission in the current session.   
 *****************************************************************************/

using PlexShareCloud;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PlexShareCloudUX
{
    public class CurrentSubmissionsModel
    {
        /// <summary>
        /// Url to access submission table.
        /// </summary>
        private string SubmissionUrl; //@"https://plexsharecloud20221118104530.azurewebsites.net/api/submission";

        /// <summary>
        /// Url to access session table.
        /// </summary>
        private string SessionUrl; //@"https://plexsharecloud20221118104530.azurewebsites.net/api/session";
        private FileDownloadApi fileDownloadApi; //creating an instance of the FiledowloadApi.
        string[] paths;

        public CurrentSubmissionsModel() //constructor for the submissionmodel class. 
        {
            paths = GetOfflinePaths("Urls.txt");
            SubmissionUrl = @paths[0];
            SessionUrl = @paths[1];
            fileDownloadApi = new FileDownloadApi(SessionUrl, SubmissionUrl);
            Trace.WriteLine("[Cloud] Current Submission Model created");
        }

        /// <summary>
        /// To store the submission list.
        /// </summary>
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
            Trace.WriteLine("Retrieved Submissions by session with sessionid " + sessionId);
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
            Trace.WriteLine("[cloud] Read the urls from file for submission models");
            return lines;
        }
    }
}
