/******************************************************************************
 * Filename    = SubmissionEntity.cs
 *
 * Author      = Polisetty Vamsi
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareCloud
 *
 * Description = Model for the entity which stores the structure of submitted file.
 *****************************************************************************/

using Azure;
using System;
using System.Text.Json.Serialization;
using ITableEntity = Azure.Data.Tables.ITableEntity;

namespace PlexShareCloud
{
    public class SubmissionEntity : ITableEntity
    {
        public const string PartitionKeyName = "SubmissionEntityPartitionKey";

        public SubmissionEntity(string sessionId, string username, byte[] pdf)
        {
            //structure of the submission entitiy. 
            PartitionKey = PartitionKeyName;
            RowKey = Guid.NewGuid().ToString();
            Id = RowKey;
            SessionId = sessionId;
            UserName = username;
            Pdf = pdf;
            Timestamp = DateTime.Now;
        }

        public SubmissionEntity() : this(null, null, null) { }

        /// <summary>
        /// To store the session id of the submission.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("SessionId")] //Unique id for the session conducted
        public string SessionId { get; set; }

        /// <summary>
        /// To store the submitted pdf.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("Pdf")] //Pdf file submitted by user. 
        public byte[] Pdf { get; set; }

        [JsonInclude]
        [JsonPropertyName("Id")] //unique id for storing the primary key. 
        public string Id { get; set; }

        /// <summary>
        /// To store the username of the submitted person.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("UserName")] //Username of the user who submitted file.
        public string UserName { get; set; }

        [JsonInclude]
        [JsonPropertyName("PartitionKey")]
        public string PartitionKey { get; set; }

        [JsonInclude]
        [JsonPropertyName("RowKey")]
        public string RowKey { get; set; }

        /// <summary>
        /// To store the Timestamp of the submission.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("Timestamp")]// time stamp when submission occured. 
        public DateTimeOffset? Timestamp { get; set; }

        [JsonIgnore]
        public ETag ETag { get; set; }
    }
}
