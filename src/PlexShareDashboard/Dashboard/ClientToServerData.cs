using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShare.Dashboard
{
    public class ClientToServerData
    {
        public string eventType;
        public int userID;
        public string username;

        //parametrized constructor 
        public ClientToServerData(string eventName, string clientName, int clientID = -1)
        {
            eventType = eventName;
            username = clientName;
            userID = clientID;
        }

        //default constructor for serialization
        public ClientToServerData()
        {

        }
    }
}
