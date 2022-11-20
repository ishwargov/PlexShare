/************************************************
 * Name : Saurabh Kumar
 * Roll : 111901046
 * Module : Dashboard
 * File Name: ClientToServerData.cs
 * This file is for creating class for datamodel of ClientToServer data
 **************************************************/

namespace PlexShareDashboard.Dashboard
{
    public class ClientToServerData
    {

        public string eventType;
        public int userID;
        public string username;
        public string userEmail;
        public string photoUrl;

        //parametrized constructor 
        public ClientToServerData(string eventName, string clientName, int clientID = -1, string clientEmail = null, string clientPhotoUrl = null)
        {
            eventType = eventName;
            username = clientName;
            userID = clientID;
            userEmail = clientEmail;
            photoUrl = clientPhotoUrl;
        }

        //default constructor for serialization
        public ClientToServerData()
        {

        }
    }

}
