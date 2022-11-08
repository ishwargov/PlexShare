using PlexShareCloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareCloudUX
{
    internal class SubmissionsModel
    {
        public SubmissionsModel()
        {

        }

        public List<SubmissionEntity>? SubmissionsList;

        public List<SubmissionEntity> GetSubmissions(string sessionId, string userName)
        {
            SubmissionsList = new List<SubmissionEntity>();
            return SubmissionsList;
        }
        public void DownloadPdf(int num)
        {
            //string path = "";
            //File.WriteAllBytes(path, SubmissionsList[i].Pdf);
        }
    }
}
