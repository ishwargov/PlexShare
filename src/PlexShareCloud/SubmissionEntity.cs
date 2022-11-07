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
            PartitionKey = PartitionKeyName;
            RowKey = Guid.NewGuid().ToString();
            Id = RowKey;
            SessionId = sessionId;
            UserName = username;
            Pdf = pdf;
            Timestamp = DateTime.Now;
        }

        public SubmissionEntity() : this(null, null, null) { }

        [JsonInclude]
        [JsonPropertyName("SessionId")]
        public string SessionId { get; set; }

        [JsonInclude]
        [JsonPropertyName("Pdf")]
        public byte[] Pdf { get; set; }

        [JsonInclude]
        [JsonPropertyName("Id")]
        public string Id { get; set; }

        [JsonInclude]
        [JsonPropertyName("UserName")]
        public string UserName { get; set; }

        [JsonInclude]
        [JsonPropertyName("PartitionKey")]
        public string PartitionKey { get; set; }

        [JsonInclude]
        [JsonPropertyName("RowKey")]
        public string RowKey { get; set; }

        [JsonInclude]
        [JsonPropertyName("Timestamp")]
        public DateTimeOffset? Timestamp { get; set; }

        [JsonIgnore]
        public ETag ETag { get; set; }
    }
}
