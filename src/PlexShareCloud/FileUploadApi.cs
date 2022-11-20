/******************************************************************************
 * Filename    = FileUploadApi.cs
 *
 * Author      = Yagnesh Katragadda
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareCloud
 *
 * Description = Consists of Rest API functions for the Submission and Session. 
 *****************************************************************************/

using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlexShareCloud
{
    /// <summary>
    /// Helper class for calling our REST APIs.
    /// </summary>
    public class FileUploadApi
    {
        private readonly HttpClient _entityClient;
        private readonly string _sessionUrl;
        private readonly string _submissionUrl;

        /// <summary>
        /// Creates an instance of the RestClient class.
        /// </summary>
        /// <param name="url">The base URL of the http client.</param>
        public FileUploadApi(string sessionUrl, string submissionUrl)
        {
            _entityClient = new();
            _sessionUrl = sessionUrl;
            _submissionUrl = submissionUrl;
        }

        /// <summary>
        /// Makes a "PUT" call to our Azure function APIs to update an entity.
        /// </summary>
        /// <param name="id">The Id of the entity to be updated.</param>
        /// <param name="newName">The new name of the entity.</param>
        /// <returns>The updated entity.</returns>
        public async Task<SubmissionEntity> PutSubmissionAsync(string sessionId, string userName, byte[] newPdf)
        {
            using HttpResponseMessage response = await _entityClient.PutAsJsonAsync<byte[]>(_submissionUrl + $"/{sessionId}/{userName}", newPdf);
            //response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,

            };

            SubmissionEntity entity = JsonSerializer.Deserialize<SubmissionEntity>(result, options);
            return entity;
        }

        /// <summary>
        /// Makes a "POST" call to our Azure function APIs to add a submission
        /// </summary>
        /// <param name="name">Name of the entity.</param>
        /// <returns>A new entity with the given name.</returns>
        public async Task<SubmissionEntity> PostSubmissionAsync(string sessionId, string userName, byte[] pdf)
        {
            using HttpResponseMessage response = await _entityClient.PostAsJsonAsync<byte[]>(_submissionUrl + $"/{sessionId}/{userName}", pdf);
            //response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,

            };

            SubmissionEntity entity = JsonSerializer.Deserialize<SubmissionEntity>(result, options);
            Trace.WriteLine("[Cloud] PDF Posted Successfully");
            return entity;
        }

        /// <summary>
        /// Makes a "POST" call to our Azure Function APIs to add a session.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<SessionEntity> PostSessionAsync(string sessionId, string userName)
        {
            using HttpResponseMessage response = await _entityClient.PostAsJsonAsync<string>(_sessionUrl + $"/{userName}", sessionId);
            //response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,

            };

            SessionEntity entity = JsonSerializer.Deserialize<SessionEntity>(result, options);
            Trace.WriteLine("[Cloud] Session Details Posted Successfully");
            return entity;
        }
    }
}