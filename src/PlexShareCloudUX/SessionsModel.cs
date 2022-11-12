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
        private const string SubmissionUrl = @"http://localhost:7213/api/submission";
        private const string SessionUrl = @"http://localhost:7213/api/session";
        private FileDownloadApi fileDownloadApi;
        public SessionsModel()
        {
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
    }
}
