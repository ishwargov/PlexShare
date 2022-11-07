using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Data.Tables;
using Azure;
using System;

namespace PlexShareCloud
{
    public static class FileStorageApp
    {
        private const string SubmissionTableName = "SubmissionTable"; //Table for storing the updated files. 
        private const string SessionTableName = "SessionTable"; //Table for storing the information about the session
        private const string ConnectionName = "AzureWebJobsStorage"; 
        private const string SubmissionRoute = "submission";
        private const string SessionRoute = "session";

        /// <summary>
        /// This function returns all the files that are submitted by user with the given username. 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="tableClient"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [FunctionName("GetFilesbyUsername")]
        public static async Task<IActionResult> GetFilesByUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = SubmissionRoute + "/users/{username}")] HttpRequest req,
        [Table(SubmissionTableName, SubmissionEntity.PartitionKeyName, Connection = ConnectionName)] TableClient tableClient,
        string username)
        {
            //log.LogInformation($"Getting entities by {username}");
            //Trace log need to be implemented. 

            var page = await tableClient.QueryAsync<SubmissionEntity>(filter: $"UserName:{username}").AsPages().FirstAsync();
            return new OkObjectResult(page.Values);
        }

        /// <summary>
        /// This function returns all files for a given sessionId. 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="tableClient"></param>
        /// <param name="sessionid"></param>
        /// <returns></returns>
        [FunctionName("GetFilesbySessionId")]
        public static async Task<IActionResult> GetFilesBySessionId(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = SubmissionRoute + "/sessions/{sessionid}")] HttpRequest req,
        [Table(SubmissionTableName, SubmissionEntity.PartitionKeyName, Connection = ConnectionName)] TableClient tableClient,
        string sessionid)
        {
            //log.LogInformation($"Getting entity {sessionid}");
            //Trace log need to be implemented. 
            
            var page = await tableClient.QueryAsync<SubmissionEntity>(filter: $"SessionId:{sessionid}").AsPages().FirstAsync();
            return new OkObjectResult(page.Values);
        }

        /// <summary>
        /// For a given username it will return all session rows which are created by the username. 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="tableClient"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [FunctionName("GetSessionsbyUsername")]
        public static async Task<IActionResult> GetSessionsByUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = SessionRoute + "/{username}")] HttpRequest req,
        [Table(SessionTableName, SubmissionEntity.PartitionKeyName, Connection = ConnectionName)] TableClient tableClient,
        string username)
        {
            //log.LogInformation($"Getting entity {username}");
            //Trace log need to implemented. 

            var page = await tableClient.QueryAsync<SessionEntity>(filter: $"UserName:{username}").AsPages().FirstAsync();
            //added the filter for user name. 

            return new OkObjectResult(page.Values);
        }


    }
}
