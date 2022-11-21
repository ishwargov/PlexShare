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
    public class UploadModel
    {
        public string SessionId;
        public string UserName;
        string[] paths;
        private string SubmissionUrl;//@"http://localhost:7213/api/submission";
        private string SessionUrl;//@"http://localhost:7213/api/session";
        private FileUploadApi _uploadClient;
        private bool isUploaded;
        public UploadModel(string sessionId, string userName, bool isServer)
        {
            SessionId = sessionId;
            UserName = userName;
            paths = GetOfflinePaths("Urls.txt");
            SubmissionUrl = @paths[0];
            SessionUrl = @paths[1];
            _uploadClient = new(SessionUrl, SubmissionUrl);
            isUploaded = false;
            if(isServer)
            {
                //_uploadClient.PostSessionAsync(sessionId, userName);
            }
            Trace.WriteLine("[Cloud] Uplod View object created");
        }
        
        /// <summary>
        /// Takes the fileName(path) as argument and uploads the content of the file to the cloud as byte array through API call.
        /// </summary>
        /// <param name="fileName">Path of the file to be submitted.</param>
        /// <returns>Boolean Value of True for successful upload and false if it fails.</returns>
        public async Task<bool> UploadDocument(string fileName)
        {
            //Upload File
            if(!isUploaded)
            {
                bool result =  await UploadDocumentAsync(fileName);
                Trace.WriteLine("[Cloud] Uploaded document");
                isUploaded = true;
                return result;
            }
            else
            {
                bool result = await ReUploadDocumentAsync(fileName);
                Trace.WriteLine("[Cloud] Updated document");
                return result;
            }
        }

        public async Task<bool> UploadDocumentAsync(string fileName)
        {
            byte[] fileContent = File.ReadAllBytes(fileName);
            try
            {
                SubmissionEntity? postEntity = await _uploadClient.PostSubmissionAsync(SessionId, UserName, fileContent);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> ReUploadDocumentAsync(string fileName)
        {
            byte[] fileContent = File.ReadAllBytes(fileName);
            try
            {
                SubmissionEntity? putEntity = await _uploadClient.PutSubmissionAsync(SessionId, UserName, fileContent);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
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
