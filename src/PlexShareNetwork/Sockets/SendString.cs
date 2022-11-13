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
        static readonly Serializer _serializer = new();

        public static string PacketToSendString(Packet packet)
        {
            return "BEGIN" + _serializer.Serialize(packet).Replace("END", "NOTEND") + "END";
        }

        public static Packet SendStringToPacket(string sendString)
        {
            return _serializer.Deserialize<Packet>(sendString.Replace("NOTEND", "END"));
        }
    }
}
