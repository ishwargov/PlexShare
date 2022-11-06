using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Routing;

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
    }
}
//How to detect a file from the link mapped in the ui
//Does it require all file download function for the instructor.
//Where should be delete file function need to be written whether in client or server side.
//In the get or download file whether it is by file name or session id or which have preference first. 


//get sessions based on username. 
//and submission based on session id. and also in case of user by username. 

//create session
//create and update submission
//delete submission

//Route = Route + "/{sessionid}" + "/{username}" + "/{pdf}" 
//below Ilogger log write the sessionid's and username, pdf. 
//only get request. after the req write the Table name and deatils. 