/// <author>Mayank Singla</author>
/// <summary>
/// Defines the enum "ClientDataHeader", which enumerates all the headers
/// that could be present in the data packet sent by the client
/// </summary>

using System.Runtime.Serialization;

namespace PlexShareScreenshare
{
    /// <summary>
    /// Enumerates all the headers that could be present in the data packet
    /// sent by the client
    /// </summary>
    public enum ClientDataHeader
    {
        /// <summary>
        /// Register a client for screen sharing
        /// </summary>
        [EnumMember(Value = "REGISTER")]
        Register,

        /// <summary>
        /// De-register a client for screen sharing
        /// </summary>
        [EnumMember(Value = "DEREGISTER")]
        Deregister,

        /// <summary>
        /// Image received from the client
        /// </summary>
        [EnumMember(Value = "IMAGE")]
        Image,

        /// <summary>
        /// Confirmation packet received from the client
        /// </summary>
        [EnumMember(Value = "CONFIRMATION")]
        Confirmation
    }
}
