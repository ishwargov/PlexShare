using System;
using System.Collections.Generic;
using System.Text;

namespace PlexShareNetworking
{
    public class Packet
    {
        private string SerializedData;
        private string ModuleIdentifier;

        // Getters
        public string getSerializedData()
        {
            return SerializedData;
        }

        public string getModuleIdentifier()
        {
            return ModuleIdentifier;
        }

        // Setters
        public void setSerializedData(string SerializedData)
        {
            this.SerializedData = SerializedData;
        }

        public void setModuleIdentifier(string ModuleIdentifier)
        {
            this.ModuleIdentifier = ModuleIdentifier;
        }
    }
}
