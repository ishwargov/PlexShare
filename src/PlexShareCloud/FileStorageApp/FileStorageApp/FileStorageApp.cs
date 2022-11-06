using System.IO;
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

namespace FileStorageApp
{
    public static class FileStorageApp
    {
        
        private const string SubmissionTableName = "SubmissionTable"; //SubmittedFiles
        private const string SessionTableName = "SessionTable";
        private const string ConnectionName = "AzureWebJobsStorage"; //Need to change
        private const string SubmissionRoute = "submission"; // Need to change. (files)
        private const string SessionRoute = "session";

        [FunctionName("GetFilesbyUsername")]
        public static IActionResult GetFilesByUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = SubmissionRoute + "/{username}")] HttpRequest req,
        [Table(SubmissionTableName, SubmissionEntity.PartitionKeyName, "{username}", Connection = ConnectionName)] SubmissionEntity entity,
        ILogger log,
        string username)
        {
            log.LogInformation($"Getting entity {username}");
            if (entity == null)
            {
                log.LogInformation($"Entity {username} not found");
                return new NotFoundResult();
            }

            return new OkObjectResult(entity);
        }

        [FunctionName("GetFilesbySessionId")]
        public static IActionResult GetFilesBySessionId(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = SubmissionRoute + "/{sessionid}")] HttpRequest req,
        [Table(SubmissionTableName, SubmissionEntity.PartitionKeyName, "{sessionid}", Connection = ConnectionName)] SubmissionEntity entity,
        ILogger log,
        string sessionid)
        {
            log.LogInformation($"Getting entity {sessionid}");
            if (entity == null)
            {
                log.LogInformation($"Entity {sessionid} not found");
                return new NotFoundResult();
            }

            return new OkObjectResult(entity);
        }

        [FunctionName("GetSessionsbyUsername")]
        public static IActionResult GetSessionsByUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = SessionRoute + "/{username}")] HttpRequest req,
        [Table(SessionTableName, SubmissionEntity.PartitionKeyName, "{username}", Connection = ConnectionName)] SubmissionEntity entity,
        ILogger log,
        string username)
        {
            log.LogInformation($"Getting entity {username}");
            if (entity == null)
            {
                log.LogInformation($"Entity {username} not found");
                return new NotFoundResult();
            }

            return new OkObjectResult(entity);
        }


        [FunctionName("CreateSession")]
        public static async Task<IActionResult> CreateSession(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Route)] HttpRequest req,
        [Table(SessionTableName, Connection = ConnectionName)] IAsyncCollector<SessionEntity> entityTable,
        ILogger log, string sessionId, string hostUserName)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string name = JsonSerializer.Deserialize<string>(requestBody);
            SessionEntity value = new(hostUserName);
            await entityTable.AddAsync(value);

            log.LogInformation($"New entity created Id = {value.Id}, Name = {value.Name}.");

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
            byte[] name = JsonSerializer.Deserialize<byte[]>(requestBody);
            SubmissionEntity value = new(sessionId, username, name);
            await entityTable.AddAsync(value);

            log.LogInformation($"New entity created Id = {value.SessionId}, Name = {value.UserName}.");

            return new OkObjectResult(value);
        }
        
        [FunctionName("UpdateSubmission")]
        public static async Task<IActionResult> UpdateSubmission(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route =  SubmissionRoute + "/{sessionId}/{username}")] HttpRequest req,
        [Table(SubmissionTableName, Connection = ConnectionName)] TableClient tableClient,
        ILogger log,
        string id)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedName = JsonSerializer.Deserialize<string>(requestBody);
            log.LogInformation($"Updating item with id = {id}");
            SubmissionEntity existingRow;
            try
            {
                var findResult = await tableClient.GetEntityAsync<SubmissionEntity>(SubmissionEntity.PartitionKeyName, id);
                existingRow = findResult.Value;
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                return new NotFoundResult();
            }

            existingRow.Name = updatedName;
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