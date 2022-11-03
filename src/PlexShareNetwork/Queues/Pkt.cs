/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains the class definition of a packet
/// </summary>

namespace Networking.Queues
{
    public class Pkt
    {
        // Serialized data being transmitted
        public string _serializedData;

        // Destination client ID of the packet
        public string _destination;

        // Module which the packet belongs to
        public string _moduleOfPacket;

        public Pkt()
        { }

        public Pkt(string serializedData, string destination, string moduleOfPacket)
        {
            this._serializedData = serializedData;
            this._destination = destination;
            this._moduleOfPacket = moduleOfPacket;
        }

    }
}
