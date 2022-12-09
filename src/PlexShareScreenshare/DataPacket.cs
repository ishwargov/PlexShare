/// <author>Mayank Singla</author>
/// <summary>
/// Defines the class "DataPacket" which represents the data packet sent
/// from server to client or the other way.
/// </summary>

using System.Text.Json.Serialization;

namespace PlexShareScreenshare
{
    /// <summary>
    /// Represents the data packet sent from server to client or the other way.
    /// </summary>
    public class DataPacket
    {
        /// <summary>
        /// Creates an instance of the DataPacket with empty string values for all
        /// the fields.
        /// </summary>
        public DataPacket()
        {
            Id = "";
            Name = "";
            Header = "";
            Data = "";
        }

        /// <summary>
        /// Creates an instance of the DataPacket containing the header field
        /// and data field in the packet used for communication between server
        /// and client.
        /// </summary>
        /// <param name="id">
        /// Id of the client/server.
        /// </param>
        /// <param name="name">
        /// Name of the client/server.
        /// </param>
        /// <param name="header">
        /// Header of the packet.
        /// </param>
        /// <param name="data">
        /// Data contained in the packet.
        /// </param>
        [JsonConstructor]
        public DataPacket(string id, string name, string header, string data)
        {
            Id = id;
            Name = name;
            Header = header;
            Data = data;
        }

        /// <summary>
        /// Gets the id field of the packet.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the name field of the packet.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the header field of the packet.
        /// Possible headers from the server: Send, Stop
        /// Possible headers from the client: Register, Deregister, Image, Confirmation
        /// </summary>
        public string Header { get; private set; }

        /// <summary>
        /// Gets the data field of the packet.
        /// Data corresponding to various headers:
        /// Server:
        ///     - Send: Serialized tuple representing the resolution of the image to send
        ///     - Stop: Empty
        /// Client:
        ///     - Register    : Empty
        ///     - Deregister  : Empty
        ///     - Image       : Serialized "Frame" representing the image
        ///     - Confirmation: Empty
        /// </summary>
        public string Data { get; private set; }
    }
}
