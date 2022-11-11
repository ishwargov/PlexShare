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
        public async Task<IReadOnlyList<SessionEntity>> GetSessionsDetails(string userName)
        {
            IReadOnlyList<SessionEntity>? getEntity = await fileDownloadApi.GetSessionsByUserAsync(userName);
            return getEntity;
        }
    }
}
