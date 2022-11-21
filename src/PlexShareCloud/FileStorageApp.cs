/******************************************************************************
 * Filename    = FileStorageApp.cs
 *
 * Author      = Polisetty Vamsi, Yagnesh Katragadda
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareCloud
 *
 * Description = Consists of function app for all functionalities expected by user. 
 *****************************************************************************/

using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            //Trace log need to be implemented. 
            Trace.WriteLine($"[cloud] Getting entities by {username}");
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
            //Trace log need to be implemented. 
            Trace.WriteLine("[cloud] Getting files by" + sessionid);
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
            //Trace log need to implemented.
            Trace.WriteLine("[cloud] Gettings sessions by username " + username);
            var page = await tableClient.QueryAsync<SessionEntity>(filter: $"HostUserName eq '{username}'").AsPages().FirstAsync();
            //added the filter for user name. 

            return new OkObjectResult(page.Values);
        }

        /// <summary>
        /// Adds the session entry to the session entity under the name of the specified User.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="entityTable">Table that stores details of all the Sessions.</param>
        /// <param name="hostUserName">The Name of the user who is organising the session.</param>
        /// <returns>Confirmation status code upon successful addition of Session details to the SessionEntity Table.</returns>
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

            Trace.WriteLine($"[cloud] New entity created Id = {value.SessionId}, Name = {value.HostUserName}");
            return new OkObjectResult(value);
        }

        /// <summary>
        /// For a specified session and user, the submitted file is added to the Submission Table.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="entityTable">Table containing the details of all the submissions by the client.</param>
        /// <param name="username"> User submitting the files.</param>
        /// <param name="sessionId"> Unique ID of the session where the user is submitting the files.</param>
        /// <returns>Confirmation status code upon successful submission of the file by the user(Added to Submission Table)</returns>
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
            Trace.WriteLine($"[cloud] New entity created Id = {value.SessionId}, Name = {value.UserName}");
            return new OkObjectResult(value);
        }

        /// <summary>
        /// Update the submission which already exists for a user for a particular session.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="tableClient"></param>
        /// <param name="username"> Name of the user who wishes to submit the file. </param>
        /// <param name="sessionId"> ID of the session where we are trying to update the submissions. </param>
        /// <returns> Confirmation status code that the submission details are updated succesfully.</returns>
        [FunctionName("UpdateSubmission")]
        public static async Task<IActionResult> UpdateSubmission(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = SubmissionRoute + "/{sessionId}/{username}")] HttpRequest req,
        [Table(SubmissionTableName, Connection = ConnectionName)] TableClient tableClient,
        string username,
        string sessionId)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            byte[] updatedPdf = JsonConvert.DeserializeObject<byte[]>(requestBody);
            Trace.WriteLine($"[cloud] Updating item with sessionId = {sessionId}");
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
            Trace.WriteLine($"[cloud] Deleting all submission items");
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
            Trace.WriteLine($"[cloud] Deleting all session items");
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
    }
}
