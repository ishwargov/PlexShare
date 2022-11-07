using FileStorageApp;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Policy;
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
        private readonly string _sessionUrl;
        private readonly string _submissionUrl;

        /// <summary>
        /// Creates an instance of the RestClient class.
        /// </summary>
        /// <param name="url">The base URL of the http client.</param>
        public FileDownloadApi(string sessionUrl, string submissionUrl)
        {
            _entityClient = new();
            _sessionUrl = sessionUrl;
            _submissionUrl = submissionUrl;
        }

        /// <summary>
        /// Makes a "GET" call to our Azure function APIs to get all entities.
        /// </summary>
        /// <returns>All the entities created so far</returns>
        public async Task<IReadOnlyList<SubmissionEntity>?> GetFilesByUserAsync(string? username)
        {
            var response = await _entityClient.GetAsync(_submissionUrl + $"/{username}");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,

            };

            IReadOnlyList<SubmissionEntity>? entities = JsonSerializer.Deserialize<IReadOnlyList<SubmissionEntity>>(result, options);
            return entities;
        }

        /// <summary>
        /// Makes a "GET" call to our Azure function APIs to get all entities.
        /// </summary>
        /// <returns>All the entities with the given sessionId created so far</returns>
        public async Task<IReadOnlyList<SubmissionEntity>?> GetFilesBySessionIdAsync(string? sessionId)
        {
            var response = await _entityClient.GetAsync(_submissionUrl + $"/{sessionId}");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,

            };

            IReadOnlyList<SubmissionEntity>? entities = JsonSerializer.Deserialize<IReadOnlyList<SubmissionEntity>>(result, options);
            return entities;
        }

        /// <summary>
        /// Makes a "GET" call to our Azure function APIs to get all entities.
        /// </summary>
        /// <returns>All the entities created so far</returns>
        public async Task<IReadOnlyList<SessionEntity>?> GetSessionsByUserAsync(string? hostUsername)
        {
            var response = await _entityClient.GetAsync(_sessionUrl + $"/{hostUsername}");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,

            };

            IReadOnlyList<SessionEntity>? entities = JsonSerializer.Deserialize<IReadOnlyList<SessionEntity>>(result, options);
            return entities;
        }
    }
}