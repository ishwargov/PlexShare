// This file contains the implementation of Server session manager.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PlexShareContent;
using PlexShareDashboard;
using Dashboard;
//using PlexShareDashboard.Dashboard.Server.Summary;
using PlexShareDashboard.Dashboard.Server.Telemetry;
using Networking;
using PlexShare.Dashboard;
using PlexShare.Dashboard.Server.SessionManagement;
using PlexShareScreenshare;
using PlexShareWhiteboard;
using PlexShareDashboard.Dashboard.Server.SessionManagement;
using Networking.Serialization;
using PlexShareDashboard.Dashboard;

namespace Dashboard.Server.SessionManagement
{
    // Delegate for the MeetingEnded event
    public delegate void NotifyEndMeet();  //this will be invoked when the meeting ended.

    public class ServerSessionManager : ITelemetrySessionManager, IUXServerSessionManager, INotificationHandler
    {
        private readonly ICommunicator _communicator;
        //private readonly IContentServer _contentServer;
        private readonly ISerializer _serializer;

        private readonly SessionData _sessionData;
       // private readonly ISummarizer _summarizer;

        private readonly List<ITelemetryNotifications> _telemetrySubscribers;

        private readonly string moduleIdentifier;
        private readonly bool testmode;  
        private MeetingCredentials _meetingCredentials;
        private SessionAnalytics _sessionAnalytics;
        private string _sessionSummary;
        private ITelemetry _telemetry;
        public bool summarySaved;
        private int userCount;
       // private ScreenShareServer _screenShareServer;

        //Constructor for the ServerSessionManager.
        //It initialises whiteboard module,content module, screenshare module,
        //networking module,summary module, telemetry module
        //and creates a list for telemetry subscribers .
        //Session manager is also subscribes to the communicator for notifications.
        //It maintains the userCount.
        public ServerSessionManager()
        {
          
            moduleIdentifier = "Dashboard";
            summarySaved = false;
            _sessionData = new SessionData();
            _serializer = new Serializer();
            _telemetrySubscribers = new List<ITelemetryNotifications>();
          //  _summarizer = SummarizerFactory.GetSummarizer();

            userCount = 0;

          //  _communicator = CommunicationFactory.GetCommunicator(false);
           // _communicator.Subscribe(moduleIdentifier, this);

            //------------------------------------_telemetry = new Telemetry.Telemetry();
          //  _ = ServerBoardCommunicator.Instance;
          //  _screenShareServer = ScreenShareFactory.GetScreenShareServer();
         //   _contentServer = ContentServerFactory.GetInstance();
        }


        //constructor for testing to be added 


        // This function is called by the networking module when a user joins the meeting.
        // The  SocketObject received from the networking module is then passed again but with a unique ID to identify object uniquely.
        public void OnClientJoined<T>(T socketObject)
        {
            lock (this)
            {
                userCount += 1;
                if (userCount == 1)
                    _telemetry =  TelemetryFactory.GetTelemetryInstance();
                UserData tempUser = new("dummy", userCount);
                _communicator.AddClient(userCount.ToString(), socketObject);
                SendDataToClient("newID", null, null, null, tempUser, userCount);
            }

        }

        //     This function is called by the networking module when the user is disconnected from the meet.
        public void OnClientLeft(string userIDString)
        {
            var userIDInt = int.Parse(userIDString);
            RemoveClientProcedure(null, userIDInt);
        }

        //     Networking module calls this function once the data is sent from the client side.
        //     The SerializedObject is the data sent by the client module which is first deserialized and processed accordingly

        public void OnDataReceived(string serializedObject)
        {
            if (serializedObject == null)
            {
                return;
            }

            // the object is obtained by deserializing the string and handling the cases 
            // based on the 'eventType' field of the deserialized object. 
            var deserializedObj = _serializer.Deserialize<ClientToServerData>(serializedObject);

            // If a null object or username is received, return without further processing.
            if (deserializedObj == null || deserializedObj.username == null)
            {
                return;
            }

            switch (deserializedObj.eventType)
            {
                case "toggleSession":
                    ToggleSessionProcedure(deserializedObj);
                    return;

                case "addClient":
                    ClientArrivalProcedure(deserializedObj);
                    return;

                    /*
                case "getSummary":
                    GetSummaryProcedure(deserializedObj);
                    return;
                    

                case "getAnalytics":
                    GetAnalyticsProcedure(deserializedObj);
                    return;
                    
                    */

                case "removeClient":
                    RemoveClientProcedure(deserializedObj);
                    return;

                case "endMeet":
                    EndMeetProcedure(deserializedObj);
                    return;

                default:
                    return;
            }
        }

        //    Telemetry will Subscribes to changes in the session object
        public void Subscribe(ITelemetryNotifications listener)
        {
            lock (this)
            {
                _telemetrySubscribers.Add(listener);
            }
        }

        //     Returns the credentials required to Join the meeting
        public MeetingCredentials GetPortsAndIPAddress()
        {
            try
            {
                var meetAddress = _communicator.Start();

                // Invalid credentials results in a returnign a null object
                if (IsValidIPAddress(meetAddress) != true)
                {
                    return null;
                }

                // For valid IP address, a MeetingCredentials Object is created and returned
                var ipAddress = meetAddress[..meetAddress.IndexOf(':')];
                var port = Convert.ToInt32(meetAddress[(meetAddress.IndexOf(':') + 1)..]);

                return _meetingCredentials = new MeetingCredentials(ipAddress, port);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw;
            }
        }

        public event NotifyEndMeet MeetingEnded;


        //     Adds a user to the list of users present in the session
        private void AddUserToSession(UserData user)
        {
            lock (this)
            {
                _sessionData.users.Add(user);
            }
        }

        // When sesssion mode is changed this function uddates the session, notifies telemetry and
        // broadcast the new session data to all the user
        private void ToggleSessionProcedure(ClientToServerData receivedObject)
        {
            //calling function to toggle the session mode
            _sessionData.ToggleMode();

            // Notify Telemetry about the change in the session object.
            NotifyTelemetryModule();

            // serialize and broadcast the data back to the client side.
            SendDataToClient("toggleSessionMode", _sessionData, null, null, null);

        }


        //    When client is added this function updates the session, notifies telemetry and
        //     broadcast the new session data to all users.
        private void ClientArrivalProcedure(ClientToServerData arrivedClient)
        {
            // create a new user and add it to the session. 
            var user = new UserData(arrivedClient.username, arrivedClient.userID);
            AddUserToSession(user);

            // Notify Telemetry about the change in the session object.
            NotifyTelemetryModule();

            // serialize and broadcast the data back to the client side.
            SendDataToClient("addClient", _sessionData, null, null, user);
        }

        //     Creates a new user based on the data arrived from the
        //     client side.
        private UserData CreateUser(string username, int userID)
        {
            lock (this)
            {

                UserData user = new(username, userID);
                return user;
            }
        }


/*
        //     Used to create a summary by fetching all the chats from the
        //     content moudule and then calling the summary module to create a summary
        private SummaryData CreateSummary()
        {
            try
            {
                // fetching all the chats from the content module.
                ChatContext[] allChatsTillNow;
                allChatsTillNow = _contentServer.SGetAllMessages().ToArray();

                // creating the summary from the chats
                _sessionSummary = _summarizer.GetSummary(allChatsTillNow);

                // returning the summary
                return new SummaryData(_sessionSummary);
            }
            catch (Exception e)
            {
                return null;
            }
        }
     
*/



        //     This method is called when the host wants to end the meeting. The summary and analytics
        //     of the session is created and stored locally. The UX server is then notified about the end of the
        //     meet and the client side session manager is also provided with the same information.
        private void EndMeetProcedure(ClientToServerData receivedObject)
        {
            var tries = 3;
            try
            {
                // n tries are made to save summary and analytics before ending the meet
                while (tries > 0 && summarySaved == false)
                {
                    // Fetching all the chats from the content module
                //    var allChats = _contentServer.SGetAllMessages().ToArray();

                //    summarySaved = _summarizer.SaveSummary(allChats);
               //     _telemetry.SaveAnalytics(allChats);

                    tries--;
                }

                SendDataToClient("endMeet", _sessionData, null, null, null);
            }
            catch (Exception e)
            {
                // In case of any exception, the meeting is ended without saving the summary.
                // The user is notified about this
                SendDataToClient("endMeet", _sessionData, null, null, null);
            }

            // stopping the communicator and notifying UX server about the End Meet event.
            _communicator.Stop();
         //   _screenShareServer.Dispose();
            MeetingEnded?.Invoke();
        }


/*        
        //     Fetches the chats from the content moudle and then asks telemetry to generate analytics on it.
        //     The analytics created are then sent to the client side again.
        private void GetAnalyticsProcedure(ClientToServerData receivedObject)
        {
            UserData user = new(receivedObject.username, receivedObject.userID);
            try
            {
                // Fetching the chats and creating analytics on them
            //    var allChats = _contentServer.SGetAllMessages().ToArray();
                _sessionAnalytics = _telemetry.GetTelemetryAnalytics(allChats);
                SendDataToClient("getAnalytics", null, null, _sessionAnalytics, user);
            }
            catch (Exception e)
            {
                // In case of a failure, the user is returned a null object
                SendDataToClient("getAnalytics", null, null, null, user);
            }
        }

        //     A getter function to fetch the summary stored in the server side. returns summary in form of string.
        public string GetStoredSummary()
        {
            return _sessionSummary;
        }

        */
        

        //this function is just for testing 
        public SessionData GetSessionData()
        {
            return _sessionData;
        }



        /*
        //     This method is called when a request for getting summary reaches the server side.
        //     A summary is created along with a user object (with the ID and the name of the user who requested the summary)
        //     This data is then sent back to the client side.
        private void GetSummaryProcedure(ClientToServerData receivedObject)
        {
            var summaryData = CreateSummary();
            UserData user = new(receivedObject.username, receivedObject.userID);
            SendDataToClient("getSummary", null, summaryData, null, user);
        }
        */


        //     Checks if an IPAddress is valid or not.
        private static bool IsValidIPAddress(string IPAddress)
        {
            // Check for null string, whitespaces or absence of colon
            if (string.IsNullOrWhiteSpace(IPAddress) || IPAddress.Contains(':') == false) return false;

            // Take the part after the colon as the port number and check the range
            var port = IPAddress[(IPAddress.LastIndexOf(':') + 1)..];
            if (int.TryParse(port, out var portNumber))
                if (portNumber < 0 || portNumber > 65535)
                    return false;

            // Take the part before colon as the ip address
            IPAddress = IPAddress.Substring(0, IPAddress.IndexOf(':'));
            var byteValues = IPAddress.Split('.');

            // IPV4 contains 4 bytes separated by .
            if (byteValues.Length != 4) return false;

            // We have 4 bytes in a address
            //byte tempForParsing;

            // for each part(elements of byteValues list), we check whether the string 
            // can be successfully converted into a byte or not.
            return byteValues.All(r => byte.TryParse(r, out var tempForParsing));
        }

        //     All subscribers are notified about the new session by calling the
        //     OnAnalyticsChanged function for Notifying Telemetry module about the session data changes.
        public void NotifyTelemetryModule()
        {
            for (var i = 0; i < _telemetrySubscribers.Count; ++i)
                lock (this)
                {
                    _telemetrySubscribers[i].OnAnalyticsChanged(_sessionData);
                }
        }

        //     Removes the user received (from the ClientToServerData) object from the sessionData and
        //     Notifies telemetry about it. The new session is then broadcasted to all the users.
        private void RemoveClientProcedure(ClientToServerData receivedObject, int userID = -1)
        {
            int userIDToRemove;
            if (userID == -1)
                userIDToRemove = receivedObject.userID;
            else
                userIDToRemove = userID;

            var removedUser = _sessionData.RemoveUserFromSession(userIDToRemove);
            _communicator.RemoveClient(userIDToRemove.ToString());

            if (_sessionData.users.Count == 0)
            {
                EndMeetProcedure(receivedObject);
                return;
            }

            if (removedUser != null)
            {
                 NotifyTelemetryModule();
                SendDataToClient("removeClient", _sessionData, null, null, removedUser);
            }
        }

        //Function to send data from Server to client side of the session manager.
        private void SendDataToClient(string eventName, SessionData sessionData, SummaryData summaryData,
           SessionAnalytics sessionAnalytics, UserData user, int userId = -1)
        {
            ServerToClientData serverToClientData;
            lock (this)
            {
                serverToClientData =
                    new ServerToClientData(eventName, sessionData, summaryData, sessionAnalytics, user);
                // Sending data to the client
                var serializedSessionData = _serializer.Serialize(serverToClientData);

                if (userId == -1)
                    _communicator.Send(serializedSessionData, moduleIdentifier);
                else
                    _communicator.Send(serializedSessionData, moduleIdentifier, userId.ToString());
            }
        }
    }


}