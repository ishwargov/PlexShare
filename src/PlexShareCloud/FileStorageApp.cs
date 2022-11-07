using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Routing;
using Azure.Data.Tables;
using Azure;
using System;

namespace PlexShareCloud
{
    public static class FileStorageApp
    {

        private const string SubmissionTableName = "SubmissionTable"; //SubmittedFiles
        private const string SessionTableName = "SessionTable";
        private const string ConnectionName = "AzureWebJobsStorage"; //Need to change
        private const string SubmissionRoute = "submission"; // Need to change. (files)
        private const string SessionRoute = "session";

        [FunctionName("GetFilesbyUsername")]
        public static async Task<IActionResult> GetFilesByUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = SubmissionRoute + "/{username}")] HttpRequest req,
        [Table(SubmissionTableName, SubmissionEntity.PartitionKeyName, "{username}", Connection = ConnectionName)] TableClient tableClient,
        ILogger log,
        string username)
        {
            log.LogInformation($"Getting entities by {username}");
            /*if (entity == null)
            {
                log.LogInformation($"Entity {username} not found");
                return new NotFoundResult();
            }

            return new OkObjectResult(entity);*/
            var page = await tableClient.QueryAsync<SubmissionEntity>().AsPages().FirstAsync();
            return new OkObjectResult(page.Values);
        }

        [FunctionName("GetFilesbySessionId")]
        public static async Task<IActionResult> GetFilesBySessionId(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = SubmissionRoute + "/{sessionid}")] HttpRequest req,
        [Table(SubmissionTableName, SubmissionEntity.PartitionKeyName, "{sessionid}", Connection = ConnectionName)] TableClient tableClient,
        ILogger log,
        string sessionid)
        {
            log.LogInformation($"Getting entity {sessionid}");
            /*if (entity == null)
            {
                log.LogInformation($"Entity {sessionid} not found");
                return new NotFoundResult();
            }

            return new OkObjectResult(entity);*/
            var page = await tableClient.QueryAsync<SubmissionEntity>().AsPages().FirstAsync();
            return new OkObjectResult(page.Values);
        }

        [FunctionName("GetSessionsbyUsername")]
        public static async Task<IActionResult> GetSessionsByUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = SessionRoute + "/{username}")] HttpRequest req,
        [Table(SessionTableName, SubmissionEntity.PartitionKeyName, "{username}", Connection = ConnectionName)] TableClient tableClient,
        ILogger log,
        string username)
        {
            log.LogInformation($"Getting entity {username}");
            /*if (entity == null)
            {
                log.LogInformation($"Entity {username} not found");
                return new NotFoundResult();
            }

            return new OkObjectResult(entity);*/
            var page = await tableClient.QueryAsync<SessionEntity>().AsPages().FirstAsync();
            return new OkObjectResult(page.Values);
        }


        [FunctionName("CreateSession")]
        public static async Task<IActionResult> CreateSession(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = SessionRoute + "/{hostUserName}")] HttpRequest req,
        [Table(SessionTableName, Connection = ConnectionName)] IAsyncCollector<SessionEntity> entityTable,
        ILogger log, string hostUserName)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string sessionId = JsonConvert.DeserializeObject<string>(requestBody);
            SessionEntity value = new(hostUserName, sessionId);
            await entityTable.AddAsync(value);

            log.LogInformation($"New entity created Id = {value.SessionId}, Name = {value.HostUserName}.");

            return new OkObjectResult(value);
        }

        //create and update submission
        [FunctionName("CreateSubmission")]
        public static async Task<IActionResult> CreateSubmission(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = SubmissionRoute + "/{sessionId}/{username}")] HttpRequest req,
        [Table(SubmissionTableName, Connection = ConnectionName)] IAsyncCollector<SubmissionEntity> entityTable,
        ILogger log, string username, string sessionId)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            byte[] pdf = JsonConvert.DeserializeObject<byte[]>(requestBody);

            SubmissionEntity value = new(sessionId, username, pdf);
            await entityTable.AddAsync(value);

            log.LogInformation($"New entity created Id = {value.SessionId}, Name = {value.UserName}.");

            return new OkObjectResult(value);
        }

        [FunctionName("UpdateSubmission")]
        public static async Task<IActionResult> UpdateSubmission(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = SubmissionRoute + "/{sessionId}/{username}")] HttpRequest req,
        [Table(SubmissionTableName, Connection = ConnectionName)] TableClient tableClient,
        ILogger log,
        string userName,
        string sessionId)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedName = JsonConvert.DeserializeObject<string>(requestBody);
            log.LogInformation($"Updating item with sessionId = {sessionId}");
            SubmissionEntity existingRow;
            try
            {
                var findResult = await tableClient.GetEntityAsync<SubmissionEntity>(SubmissionEntity.PartitionKeyName, sessionId);
                existingRow = findResult.Value;
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                return new NotFoundResult();
            }

            existingRow.UserName = updatedName;
            await tableClient.UpdateEntityAsync(existingRow, existingRow.ETag, TableUpdateMode.Replace);

            return new OkObjectResult(existingRow);
        }
    }
}

//delete submission
/*[FunctionName("DeleteSubmission")]
public static async Task<IActionResult> DeleteSubmission(
[HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Route + "/{id}")] HttpRequest req,
[Table(SubmissionTableName, ConnectionName)] TableClient entityClient,
ILogger log,
string id)
{
    log.LogInformation($"Deleting entity by {id}");
    try
    {
        await entityClient.DeleteEntityAsync(SubmissionEntity.PartitionKeyName, id, ETag.All);
    }
    catch (RequestFailedException e) when (e.Status == 404)
    {
        return new NotFoundResult();
    }

    return new OkResult();
}*/

//Route = Route + "/{sessionid}" + "/{username}" + "/{pdf}" 
//below Ilogger log write the sessionid's and username, pdf. 
//only get request. after the req write the Table name and deatils. 

//How to detect a file from the link mapped in the ui
//Does it require all file download function for the instructor.
//Where should be delete file function need to be written whether in client or server side.
//In the get or download file whether it is by file name or session id or which have preference first. 


//get sessions based on username. 
//and submission based on session id. and also in case of user by username. 

//create session