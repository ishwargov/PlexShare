/// <author> Mohammad Umar Sultan </author>
/// <created> 16/10/2022 </created>
/// <summary>
/// This file contains the class definition of SendString.
/// </summary>

using PlexShareNetwork.Queues;
using PlexShareNetwork.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareNetwork.Sockets
{
    public static class SendString
    {
        // serializer to serialize packets
        static readonly Serializer _serializer = new();

        /// <summary>
        /// Convertes a packet to a string which can be sent.
        /// It serializes the packet and frames it.
        /// </summary>
        /// <param name="packet">
        /// The packet which needs to be converted to send string.
        /// </param>
        /// <returns>
        /// The serialized and framed string.
        /// </returns>
        public static string PacketToSendString(Packet packet)
        {
            // serialize the packet
            string sendString = _serializer.Serialize(packet);

            // replace the "END" from send string by "NOTEND" because
            // we are going to mark the end by "END"
            sendString = sendString.Replace("END", "NOTEND");

            // frame the send string by "BEGIN" and "END"
            sendString = "BEGIN" + sendString + "END";

            // return this final send string
            return sendString;
        }

        /// <summary>
        /// Convertes the send string back to packet.
        /// It removes the frame and deserializes the string.
        /// </summary>
        /// <param name="packet">
        /// The packet which needs to be converted to send string.
        /// </param>
        /// <returns>
        /// The serialized and framed string.
        /// </returns>
        public static Packet SendStringToPacket(string sendString)
        {
            // replace "NOTEND" by "END" because we had replaced "END"
            // by "NOTEND" when converting the packet to the send string
            sendString = sendString.Replace("NOTEND", "END");

            // deserialize the send string and return it
            return _serializer.Deserialize<Packet>(sendString);
        }
    }
}
