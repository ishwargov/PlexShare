/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains the class definition of a packet
/// </summary>

namespace Networking.Queues
{
    public class Packet
    {
        // Serialized data being transmitted
        private string _serializedData;

        // Destination client ID of the packet
        private string _destination;

        // Module which the packet belongs to
        private string _moduleOfPacket;

        public Packet(string serializedData, string destination, string moduleOfPacket)
        {
            this._serializedData = serializedData;
            this._destination = destination;
            this._moduleOfPacket = moduleOfPacket;
        }

        // Getters
        public string getSerializedData()
        {
            return _serializedData;
        }

        public string getDestination()
        {
            return _destination;
        }

        public string getModuleOfPacket()
        {
            return _moduleOfPacket;
        }

        // Setters
        public void setSerializedData(string serializedData)
        {
            this._serializedData = serializedData;
        }

        public void setDestination(string destination)
        {
            this._destination = destination;
        }

        public void setModuleOfPacket(string moduleOfPacket)
        {
            this._moduleOfPacket = moduleOfPacket;
        }
    }
}
