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

using System;
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
            _entityClient = new();
            _sessionUrl = sessionUrl;
            _submissionUrl = submissionUrl;
            Trace.WriteLine("[Cloud] New entity client created");
        }

        /// <summary>
        /// Makes a "GET" call to our Azure function APIs to get all files for given username.
        /// </summary>
        /// <param name="username">Username of the user details we are searching for</param>
        /// <returns>Returns all files in submission table with the given username. </returns>
        public async Task<IReadOnlyList<SubmissionEntity>> GetFilesByUserAsync(string username)
        {
            try
            {
                var response = await _entityClient.GetAsync(_submissionUrl + $"/users/{username}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,

                };

                IReadOnlyList<SubmissionEntity> entities = JsonSerializer.Deserialize<IReadOnlyList<SubmissionEntity>>(result, options);
                Trace.WriteLine("[Cloud] Retreived all submissions for " + username);
                return entities;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[Cloud] Network Error Exception " + ex);
                return new List<SubmissionEntity>();
            }
        }

        /// <summary>
        /// Makes a "GET" call to our Azure function APIs to get all files for given sessionid. 
        /// </summary>
        /// <param name="sessionId">The unqiue id for the given session</param>
        /// <returns>Return all rows in Sessions with given session Id. </returns>
        public async Task<IReadOnlyList<SubmissionEntity>> GetFilesBySessionIdAsync(string sessionId)
        {
            try
            {
                var response = await _entityClient.GetAsync(_submissionUrl + $"/sessions/{sessionId}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,

                };

                IReadOnlyList<SubmissionEntity> entities = JsonSerializer.Deserialize<IReadOnlyList<SubmissionEntity>>(result, options);
                Trace.WriteLine("[Cloud] Retreived all files for " + sessionId);
                return entities;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[Cloud] Network Error Exception " + ex);
                return new List<SubmissionEntity>();
            }
        }

        /// <summary>
        /// Makes a "GET" call to our Azure function APIs to get all sessions for given username.
        /// </summary>
        /// <param name="hostUsername">Username of the user details we are searching for who conducted sessions</param>
        /// <returns>Return all rows in session table with username of host given</returns>
        public async Task<IReadOnlyList<SessionEntity>> GetSessionsByUserAsync(string hostUsername)
        {
            try
            {
                var response = await _entityClient.GetAsync(_sessionUrl + $"/{hostUsername}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,

                };

                IReadOnlyList<SessionEntity> entities = JsonSerializer.Deserialize<IReadOnlyList<SessionEntity>>(result, options);
                Trace.WriteLine("[Cloud] Retreived all sessions for " + hostUsername);
                return entities;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[Cloud] Network Error Exception " + ex);
                return new List<SessionEntity>();
            }
        }

        /// <summary>
        /// This Async takes cares for deleting files in the Submissions table.
        /// </summary>
        /// <returns>Returns a Http response with true when successfully exectues the delete operation.</returns>
        public async Task DeleteAllFilesAsync()
        {
            try
            {
                using HttpResponseMessage response = await _entityClient.DeleteAsync(_submissionUrl);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[cloud] Network Error Exception " + ex);
            }
        }

        /// <summary>
        /// This Async takes cares for deleting files in the sessions table. 
        /// </summary>
        /// <returns>Returns a Http response with true when successfully exectues the delete operation.</returns>
        public async Task DeleteAllSessionsAsync()
        {
            try
            {
                using HttpResponseMessage response = await _entityClient.DeleteAsync(_sessionUrl);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[cloud] Network Error Exception " + ex);
            }
        }

        /* 
        public async Task DeleteFilesbyUserAsync(string username)
        {
            using HttpResponseMessage response = await _entityClient.DeleteAsync(_submissionUrl + $"/users/{username}");
            response.EnsureSuccessStatusCode();
        }*/
    }
}