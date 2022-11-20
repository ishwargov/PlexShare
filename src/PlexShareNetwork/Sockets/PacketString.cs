/// <author> Mohammad Umar Sultan </author>
/// <created> 16/10/2022 </created>
/// <summary>
/// This file contains the class definition of PacketString.
/// </summary>

using PlexShareNetwork.Queues;
using PlexShareNetwork.Serialization;
using System;
using System.Diagnostics;

namespace PlexShareNetwork.Sockets
{
    public static class PacketString
    {
        // serializer to serialize packets
        static readonly Serializer _serializer = new();

        /// <summary>
        /// Convertes a packet to a serialized and framed string.
        /// </summary>
        /// <param name="packet">
        /// The packet which needs to be converted to string.
        /// </param>
        /// <returns>
        /// The serialized and framed string of the packet.
        /// </returns>
        public static string PacketToPacketString(Packet packet)
        {
            try
            {
                // serialize the packet
                string packetString = _serializer.Serialize(packet);

                // replace the "END" from the string by "NOTEND" because
                // we are going to mark the end of the string by "END"
                packetString = packetString.Replace("END", "NOTEND");

                // frame the packet string by "BEGIN" and "END"
                // and return this final string
                packetString = "BEGIN" + packetString + "END";

                return packetString;
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Networking] Error in " +
                    "PacketString.PacketToPacketString(): " +
                    e.Message);
                return "null";
            }

        }

        /// <summary>
        /// Convertes the packet string back to packet. It removes the
        /// frame and deserializes the string to get back the packet.
        /// </summary>
        /// <param name="packetString">
        /// The packet string which needs to be converted to packet.
        /// </param>
        /// <returns>
        /// The packet which is in the packet string.
        /// </returns>
        public static Packet PacketStringToPacket(string packetString)
        {
            try
            {
                // remove the "BEGIN" and "END" frame from the string
                packetString = 
                    packetString[5..(packetString.Length - 3)];

                // replace "NOTEND" by "END" because we had replaced
                // "END" by "NOTEND" when converting the packet to
                // packet string
                packetString = packetString.Replace("NOTEND", "END");

                // deserialize the packet string to get back the packet
                // and return this packet
                Packet packet =
                    _serializer.Deserialize<Packet>(packetString);

                return packet;
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Networking] Error in " +
                    "PacketString.PacketToPacketString(): " +
                    e.Message);
                return new Packet("null", "null", "null");
            }
        }
    }
}
