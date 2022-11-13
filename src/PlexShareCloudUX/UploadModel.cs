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

namespace PlexShareCloudUX
{
    internal class UploadModel
    {
        public string SessionId;
        public string UserName;
        private const string SubmissionUrl = @"http://localhost:7213/api/submission";
        private const string SessionUrl = @"http://localhost:7213/api/session";
        private FileUploadApi _uploadClient;
        public UploadModel(string sessionId, string userName)
        {
            SessionId = sessionId;
            UserName = userName;
            _uploadClient = new(SessionUrl, SubmissionUrl);
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
                UploadDocumentAsync(fileName);
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

        /// <summary>
        /// Takes the file path that is to be Resubmitted. Makes a call to the PutSubmission API to resubmit the file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns> Returns true upon successful resubmission. If it fails, it returns false.</returns>
        public bool ReUploadDocument(string fileName)
        {
            try
            {
                ReUploadDocumentAsync(fileName);
                return true;
            }

            catch
            {
                return false;
            }
        }

        public async void ReUploadDocumentAsync(string fileName)
        {
            byte[] fileContent = File.ReadAllBytes(fileName);
            SubmissionEntity? putEntity = await _uploadClient.PutSubmissionAsync(SessionId, UserName, fileContent);
        }
    }
}
