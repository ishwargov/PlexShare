/******************************************************************************
 * Filename    = FileDownloadApi.cs
 *
 * Author      = Polisetty Vamsi
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareCloud
 *
 * Description = Provides Api functionality for the user to get the file from cloud. 
 *****************************************************************************/

using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlexShareCloud
{
    /// <summary>
    /// Helper class for calling our REST APIs.
    /// </summary>
    public class FileDownloadApi
    {
        private readonly HttpClient _entityClient;
        private readonly string _sessionUrl; //seperate url for session and submission. 
        private readonly string _submissionUrl;

        /// <summary>
        /// Creates an instance of the RestClient class.
        /// </summary>
        /// <param name="sessionUrl">Head Url for the session request</param>
        /// <param name="submissionUrl">Head Url for the submission request</param>
        public FileDownloadApi(string sessionUrl, string submissionUrl)
        {
            Trace.WriteLine("New entity client created");
            _entityClient = new();
            _sessionUrl = sessionUrl;
            _submissionUrl = submissionUrl;
        }

        /// <summary>
        /// Makes a "GET" call to our Azure function APIs to get all files for given username.
        /// </summary>
        /// <param name="username">Username of the user details we are searching for</param>
        /// <returns>Returns all files in submission table with the given username. </returns>
        public async Task<IReadOnlyList<SubmissionEntity>> GetFilesByUserAsync(string username)
        {
            var response = await _entityClient.GetAsync(_submissionUrl + $"/users/{username}");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,

            };

            IReadOnlyList<SubmissionEntity> entities = JsonSerializer.Deserialize<IReadOnlyList<SubmissionEntity>>(result, options);
            Trace.WriteLine("Retreived all submissions for " + username);
            //Trace need to be added. 
            return entities;
        }

        /// <summary>
        /// Makes a "GET" call to our Azure function APIs to get all files for given sessionid. 
        /// </summary>
        /// <param name="sessionId">The unqiue id for the given session</param>
        /// <returns>Return all rows in Sessions with given session Id. </returns>
        public async Task<IReadOnlyList<SubmissionEntity>> GetFilesBySessionIdAsync(string sessionId)
        {
            var response = await _entityClient.GetAsync(_submissionUrl + $"/sessions/{sessionId}");
            //response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,

            };

            IReadOnlyList<SubmissionEntity> entities = JsonSerializer.Deserialize<IReadOnlyList<SubmissionEntity>>(result, options);
            Trace.WriteLine("Retreived all files for " + sessionId);
            return entities;
        }

        /// <summary>
        /// Makes a "GET" call to our Azure function APIs to get all sessions for given username.
        /// </summary>
        /// <param name="hostUsername">Username of the user details we are searching for who conducted sessions</param>
        /// <returns>Return all rows in session table with username of host given</returns>
        public async Task<IReadOnlyList<SessionEntity>> GetSessionsByUserAsync(string hostUsername)
        {
            var response = await _entityClient.GetAsync(_sessionUrl + $"/{hostUsername}");
            //response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,

            };

            IReadOnlyList<SessionEntity> entities = JsonSerializer.Deserialize<IReadOnlyList<SessionEntity>>(result, options);
            //Trace to be added. 
            Trace.WriteLine("Retreived all sessions for " + hostUsername);
            return entities;
        }

        /// <summary>
        /// This Async takes cares for deleting files in the Submissions table.
        /// </summary>
        /// <returns>Returns a Http response with true when successfully exectues the delete operation.</returns>
        public async Task DeleteAllFilesAsync()
        {
            using HttpResponseMessage response = await _entityClient.DeleteAsync(_submissionUrl);
            Trace.WriteLine("Deleted all rows in submissions table");
            //response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// This Async takes cares for deleting files in the sessions table. 
        /// </summary>
        /// <returns>Returns a Http response with true when successfully exectues the delete operation.</returns>
        public async Task DeleteAllSessionsAsync()
        {
            using HttpResponseMessage response = await _entityClient.DeleteAsync(_sessionUrl);
            Trace.WriteLine("Deleted all rows in session table");
            //response.EnsureSuccessStatusCode();
        }

        /* 
        public async Task DeleteFilesbyUserAsync(string username)
        {
            using HttpResponseMessage response = await _entityClient.DeleteAsync(_submissionUrl + $"/users/{username}");
            response.EnsureSuccessStatusCode();
        }*/
    }
}