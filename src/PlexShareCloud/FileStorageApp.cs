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
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;

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
        /// <param name="req">req is HttpRequest which we are not using here.</param>
        /// <param name="tableClient">Submission Table which contains files.</param>
        /// <param name="username">Username of the user details we are searching for</param>
        /// <returns></returns>
        [FunctionName("GetFilesbyUsername")]
        public static async Task<IActionResult> GetFilesByUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = SubmissionRoute + "/users/{username}")] HttpRequest req,
        [Table(SubmissionTableName, SubmissionEntity.PartitionKeyName, Connection = ConnectionName)] TableClient tableClient,
        string username)
        {
            //log.LogInformation($"Getting entities by {username}");
            //Trace log need to be implemented. 

            var page = await tableClient.QueryAsync<SubmissionEntity>(filter: $"UserName eq '{username}'").AsPages().FirstAsync();
            return new OkObjectResult(page.Values);
        }

        /// <summary>
        /// This function returns all files for a given sessionId. 
        /// </summary>
        /// <param name="req">req is HttpRequest which we are not using here.</param>
        /// <param name="tableClient">Submission Table which contains files.</param>
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
            
            var page = await tableClient.QueryAsync<SubmissionEntity>(filter: $"SessionId eq '{sessionid}'").AsPages().FirstAsync();
            return new OkObjectResult(page.Values);
        }

        /// <summary>
        /// For a given username it will return all session rows which are created by the username. 
        /// </summary>
        /// <param name="req">req is HttpRequest which we are not using here.</param>
        /// <param name="tableClient">Session Table</param>
        /// <param name="username">Username of the user details we are searching for</param>
        /// <returns></returns>
        [FunctionName("GetSessionsbyUsername")]
        public static async Task<IActionResult> GetSessionsByUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = SessionRoute + "/{username}")] HttpRequest req,
        [Table(SessionTableName, SubmissionEntity.PartitionKeyName, Connection = ConnectionName)] TableClient tableClient,
        string username)
        {
            //log.LogInformation($"Getting entity {username}");
            //Trace log need to implemented. 

            var page = await tableClient.QueryAsync<SessionEntity>(filter: $"HostUserName eq '{username}'").AsPages().FirstAsync();
            //added the filter for user name. 

            return new OkObjectResult(page.Values);
        }

        [FunctionName("CreateSession")]
        public static async Task<IActionResult> CreateSession(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = SessionRoute + "/{hostUserName}")] HttpRequest req,
        [Table(SessionTableName, Connection = ConnectionName)] IAsyncCollector<SessionEntity> entityTable,
        string hostUserName)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string sessionId = JsonConvert.DeserializeObject<string>(requestBody);
            SessionEntity value = new(hostUserName, sessionId);
            await entityTable.AddAsync(value);
            //log.LogInformation($"New entity created Id = {value.SessionId}, Name = {value.HostUserName}.");
            return new OkObjectResult(value);
        }

        //create and update submission
        [FunctionName("CreateSubmission")]
        public static async Task<IActionResult> CreateSubmission(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = SubmissionRoute + "/{sessionId}/{username}")] HttpRequest req,
        [Table(SubmissionTableName, Connection = ConnectionName)] IAsyncCollector<SubmissionEntity> entityTable,
        string username, string sessionId)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            byte[] pdf = JsonConvert.DeserializeObject<byte[]>(requestBody);

            SubmissionEntity value = new(sessionId, username, pdf);
            await entityTable.AddAsync(value);
            //log.LogInformation($"New entity created Id = {value.SessionId}, Name = {value.UserName}.");
            return new OkObjectResult(value);
        }

        [FunctionName("UpdateSubmission")]
        public static async Task<IActionResult> UpdateSubmission(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = SubmissionRoute + "/{sessionId}/{username}")] HttpRequest req,
        [Table(SubmissionTableName, Connection = ConnectionName)] TableClient tableClient,
        string username,
        string sessionId)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            byte[] updatedPdf = JsonConvert.DeserializeObject<byte[]>(requestBody);
            //log.LogInformation($"Updating item with sessionId = {sessionId}");
            SubmissionEntity existingRow;
            try
            {
                //var findResult = await tableClient.GetEntityAsync<SubmissionEntity>(SubmissionEntity.PartitionKeyName, sessionId);
                var findResult = await tableClient.QueryAsync<SubmissionEntity>(filter: $"(UserName eq '{username}') and (SessionId eq '{sessionId}')").AsPages().FirstAsync();
                existingRow = findResult.Values[0];
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                return new NotFoundResult();
            }
            existingRow.Pdf = updatedPdf;
            await tableClient.UpdateEntityAsync(existingRow, existingRow.ETag, TableUpdateMode.Replace);
            return new OkObjectResult(existingRow);

        }

        //Delete all files 
        /// <summary>
        /// Deleting all rows in the Submission Table.
        /// </summary>
        /// <param name="req">req is HttpRequest which we are not using here.</param>
        /// <param name="entityClient"></param>
        /// <returns></returns>
        [FunctionName("DeleteAllFiles")]
        public static async Task<IActionResult> DeleteAllFiles(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = SubmissionRoute)] HttpRequest req,
        [Table(SubmissionTableName, ConnectionName)] TableClient entityClient)
        {
            //log.LogInformation($"Deleting all entity items");
            //Trace need to be added. 
            try
            {
                await entityClient.DeleteAsync();
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                return new NotFoundResult();
            }

            return new OkResult();
        }

        //Delete all sessions
        /// <summary>
        /// Deleting all rows in sessions table. 
        /// </summary>
        /// <param name="req">req is HttpRequest which we are not using here.</param>
        /// <param name="entityClient"></param>
        /// <returns></returns>
        [FunctionName("DeleteAllSessions")]
        public static async Task<IActionResult> DeleteAllSessions(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = SessionRoute)] HttpRequest req,
        [Table(SessionTableName, ConnectionName)] TableClient entityClient)
        {
            //log.LogInformation($"Deleting all entity items");
            //Trace Need to be added. 
            try
            {
                await entityClient.DeleteAsync();
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                return new NotFoundResult();
            }

            return new OkResult();
        }

        /*
        //Delete all files 
        [FunctionName("DeleteAllFilesOfUser")]
        public static async Task<IActionResult> DeleteAllFilesOfUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = SubmissionRoute + "/users/{username}")] HttpRequest req,
        [Table(SubmissionTableName, ConnectionName)] TableClient entityClient,
        string username)
        {
            //Trace need to be added. 
            //log.LogInformation($"Deleting entity by {username}");
            try
            {
                await entityClient.DeleteEntityAsync(SubmissionEntity.PartitionKeyName, username, ETag.All);
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                return new NotFoundResult();
            }

            return new OkResult();
        }*/

        /*
        Delete files by username??
        Delete file by sessionid and username
        Delete session by sessionid
        Delete session by hostusername
        */
    }
}
