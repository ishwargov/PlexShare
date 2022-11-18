/// <author> Mohammad Umar Sultan </author>
/// <created> 16/10/2022 </created>
/// <summary>
/// This file contains unit tests for the class PacketString
/// </summary>

using PlexShareNetwork.Queues;
using PlexShareNetwork.Sockets;

namespace PlexShareTests.NetworkTests.Sockets
{
    public class PacketStringTests
    {
        /// <summary>
        /// Tests conversion from packet to packet string and then
        /// back to packet
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void PacketToStringAndStringToPacketTest()
        {
            Packet packet = new("Data", "Destination", "Module");
            string pktStr = 
                PacketString.PacketToPacketString(packet);
            Packet packet1 = PacketString.PacketStringToPacket(pktStr);
            NetworkTestGlobals.AssertPacketEquality(packet, packet1);
        }

        /// <summary>
        /// Tests error catch when converting packet to packet string
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void PacketToPacketStringErrorCatchTest()
        {
            string pktStr = PacketString.PacketToPacketString(null);
            Assert.Equal("null", pktStr);
        }

        /// <summary>
        /// Tests error catch when converting packet string to packet
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void PacketStringToPacketErrorCatchTest()
        {
            Packet packet = new("null", "null", "null");
            Packet packet1 = PacketString.PacketStringToPacket(null);
            NetworkTestGlobals.AssertPacketEquality(packet, packet1);
        }
    }
}
