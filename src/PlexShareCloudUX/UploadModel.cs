using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareCloudUX
{
    internal class UploadModel
    {
        public string SessionId;
        public string UserName;
        public UploadModel(string sessionId, string userName)
        {
            SessionId = sessionId;
            UserName = userName;
        }
        public bool UploadDocument(string fileName)
        {
            //Upload File
            /*System.IO.StreamReader sr = new System.IO.StreamReader(fileDialog.FileName);
            MessageBox.Show(sr.ReadToEnd());
            sr.Close();*/
            return false;
        }
    }
}
