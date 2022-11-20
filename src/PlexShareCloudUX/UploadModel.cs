/******************************************************************************
 * Filename    = UploadModel.cs
 *
 * Author      = Yagnesh Katragadda
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareCloud
 *
 * Description = Consists of functions to upload to drive by making API calls. 
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PlexShareCloud;
using System.Diagnostics;

namespace PlexShareCloudUX
{
    internal class UploadModel
    {
        public string SessionId;
        public string UserName;
        private const string SubmissionUrl = @"https://plexsharecloud20221118104530.azurewebsites.net/api/submission";
        private const string SessionUrl = @"https://plexsharecloud20221118104530.azurewebsites.net/api/session";
        private FileUploadApi _uploadClient;
        private bool isUploaded;
        public UploadModel(string sessionId, string userName, bool isServer)
        {
            SessionId = sessionId;
            UserName = userName;
            _uploadClient = new(SessionUrl, SubmissionUrl);
            isUploaded = false;
            if(isServer)
            {
                _uploadClient.PostSessionAsync(sessionId, userName);
            }
            Trace.WriteLine("[Cloud] Uplod View object created");
        }
        
        /// <summary>
        /// Takes the fileName(path) as argument and uploads the content of the file to the cloud as byte array through API call.
        /// </summary>
        /// <param name="fileName">Path of the file to be submitted.</param>
        /// <returns>Boolean Value of True for successful upload and false if it fails.</returns>
        public bool UploadDocument(string fileName)
        {
            //Upload File
            try
            {
                if(!isUploaded)
                {
                    UploadDocumentAsync(fileName);
                    isUploaded = true;
                }
                else
                {
                    ReUploadDocumentAsync(fileName);
                }
                return true;
            }
            catch 
            {
                return false;
            }
        }

        public async void UploadDocumentAsync(string fileName)
        {
            byte[] fileContent = File.ReadAllBytes(fileName);
            SubmissionEntity? postEntity = await _uploadClient.PostSubmissionAsync(SessionId, UserName, fileContent);
        }

        public async void ReUploadDocumentAsync(string fileName)
        {
            byte[] fileContent = File.ReadAllBytes(fileName);
            SubmissionEntity? putEntity = await _uploadClient.PutSubmissionAsync(SessionId, UserName, fileContent);
        }
    }
}
