/*************************************
 * Name : Saurabh Kumar
 * Roll : 111901046
 * Module : Dashboard
 * File Name : ServerSessionManager.cs
 * This file contains the implementation of Server session manager.
 *************************************/

using PlexShareContent.DataModels;
using PlexShareContent.Server;
using PlexShareDashboard.Dashboard;
using PlexShareDashboard.Dashboard.Server.SessionManagement;
using PlexShareDashboard.Dashboard.Server.Summary;
using PlexShareDashboard.Dashboard.Server.Telemetry;
using PlexShareNetwork;
using PlexShareNetwork.Communication;
using PlexShareScreenshare.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Dashboard.Server.SessionManagement
{
    // Delegate for the MeetingEnded event  this will be invoked when the meeting ended.
    public delegate void NotifyEndMeet();

    public class ServerSessionManager : ITelemetrySessionManager, IUXServerSessionManager, INotificationHandler
    {
        private readonly ICommunicator _communicator;
        private readonly IContentServer _contentServer;
        private readonly IDashboardSerializer _serializer;

        private readonly SessionData _sessionData;
        private readonly ISummarizer _summarizer;

        private readonly List<ITelemetryNotifications> _telemetrySubscribers;

        private readonly string moduleIdentifier;
        private readonly bool testmode;
        private MeetingCredentials _meetingCredentials;
        private SessionAnalytics _sessionAnalytics;
        private string _sessionSummary;
        private ITelemetry _telemetry;
        public bool summarySaved;
        private int userCount;
        private ScreenshareServer _screenShareServer;


        /// <summary>
        /// Constructor for the ServerSessionManager.
        ///It initialises whiteboard module,content module, screenshare module,
        ///networking module,summary module
        ///and creates a list for telemetry subscribers .
        ///Session manager is also subscribes to the communicator for notifications.
        ///It maintains the userCount.
        /// </summary>
        public ServerSessionManager()
        {

            moduleIdentifier = "Dashboard";
            summarySaved = false;
            _sessionData = new SessionData();
            _serializer = new DashboardSerializer();
            _telemetrySubscribers = new List<ITelemetryNotifications>();
            _summarizer = SummarizerFactory.GetSummarizer();

            userCount = 0;

            _communicator = CommunicationFactory.GetCommunicator(false);
            _communicator.Subscribe(moduleIdentifier, this);

            //  _ = ServerBoardCommunicator.Instance;
            //  _screenShareServer = ScreenShareFactory.GetScreenShareServer();
            _contentServer = ContentServerFactory.GetInstance();
        }

        /// <summary>
        /// constructor for testing to be added
        /// </summary>
        /// <param name="communicator"></param>
        public ServerSessionManager(ICommunicator communicator)
        {
            //  _contentServer = contentServer;
            _sessionData = new SessionData();
            _serializer = new DashboardSerializer();
            _telemetrySubscribers = new List<ITelemetryNotifications>();
            _summarizer = SummarizerFactory.GetSummarizer();

            userCount = 0;
            moduleIdentifier = "serverSessionManager";

            _communicator = communicator;
            _communicator.Subscribe(moduleIdentifier, this);
            summarySaved = false;
            testmode = true;

        }

        // This function is called by the networking module when a user joins the meeting.
        // The  SocketObject received from the networking module is then passed again but with a unique ID to identify object uniquely.
        public void OnClientJoined(TcpClient socketObject)
        {
            Trace.WriteLine("[Dashboard Server] Assigning the UserID to user");
            lock (this)
            {
                userCount += 1;
                if (userCount == 1)
                    _telemetry = testmode ? new Telemetry() : TelemetryFactory.GetTelemetryInstance();
                UserData tempUser = new(null, userCount, null, null);
                _communicator.AddClient(userCount.ToString(), socketObject);
                SendDataToClient("newID", null, null, null, tempUser, userCount);
            }

        }

        /// <summary>
        /// This function is called by the networking module when the user is disconnected from the meet.
        /// </summary>
        /// <param name="userIDString"></param>
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
                throw new ArgumentNullException("Null serializedObject Exception");
            }

            // the object is obtained by deserializing the string and handling the cases 
            // based on the 'eventType' field of the deserialized object. 
            var deserializedObj = _serializer.Deserialize<ClientToServerData>(serializedObject);

            // If a null object or username is received, return without further processing.
            if (deserializedObj == null || deserializedObj.username == null)
            {
                throw new ArgumentNullException();
            }

            switch (deserializedObj.eventType)
            {
                case "toggleSession":
                    ToggleSessionProcedure(deserializedObj);
                    return;

                case "addClient":
                    ClientArrivalProcedure(deserializedObj);
                    return;

                case "getSummary":
                    GetSummaryProcedure(deserializedObj);
                    return;

                case "getAnalytics":
                    GetAnalyticsProcedure(deserializedObj);
                    return;

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
                Trace.WriteLine("[Dashboard] Asking Network to create Server");
                var meetAddress = _communicator.Start();

                // Invalid credentials results in a returning a null object
                if (IsValidIPAddress(meetAddress) != true)
                {
                    return null;
                }

                // For valid IP address, a MeetingCredentials Object is created and returned
                var ipAddress = meetAddress[..meetAddress.IndexOf(':')];
                var port = Convert.ToInt32(meetAddress[(meetAddress.IndexOf(':') + 1)..]);

                _meetingCredentials = new MeetingCredentials(ipAddress, port);
                Trace.WriteLine("[Dashboard] Server Created Succesfully");
                return _meetingCredentials;
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
            Trace.WriteLine("[Dashboard Server] Session Mode changed in Session Data");
            // serialize and broadcast the data back to the client side.
            SendDataToClient("toggleSessionMode", _sessionData, null, null, null);

        }


        //    When client is added this function updates the session, notifies telemetry and
        //     broadcast the new session data to all users.
        public void ClientArrivalProcedure(ClientToServerData arrivedClient)
        {
            // create a new user and add it to the session. 
            var user = CreateUser(arrivedClient.userID, arrivedClient.username, arrivedClient.userEmail, arrivedClient.photoUrl);
            AddUserToSession(user);

            // Notify Telemetry about the change in the session object.
            NotifyTelemetryModule();

            // serialize and broadcast the data back to the client side.
            SendDataToClient("addClient", _sessionData, null, null, user);
        }



        //     Creates a new user based on the data arrived from the
        //     client side.
        private UserData CreateUser(int userID, string username, string userEmail, string photoUrl)
        {
            lock (this)
            {

                UserData user = new(username, userID, userEmail, photoUrl);
                return user;
            }
        }



        //     Used to create a summary by fetching all the chats from the
        //     content moudule and then calling the summary module to create a summary
        private SummaryData CreateSummary()
        {

            try
            {
                Trace.WriteLine("[Dashboard] Calling Chat Module to provide Chats");
                if (testmode == false)
                {
                    // fetching all the chats from the content module.
                    ChatThread[] allChatsTillNow;
                    allChatsTillNow = _contentServer.GetAllMessages().ToArray();

                    // creating the summary from the chats
                    _sessionSummary = _summarizer.GetSummary(allChatsTillNow);

                    // returning the summary
                    return new SummaryData(_sessionSummary);
                }
                else
                {
                    _sessionSummary = "This is testing summary";
                    return new SummaryData(_sessionSummary);
                }

            }
            catch (Exception e)
            {
                Trace.WriteLine("[Dashboard] Summary not created");
                return null;
            }
        }



        //     This method is called when the host wants to end the meeting. The summary and analytics
        //     of the session is created and stored locally. The UX server is then notified about the end of the
        //     meet and the client side session manager is also provided with the same information.
        private void EndMeetProcedure(ClientToServerData receivedObject)
        {
            Trace.WriteLine("[Dashboard] EndMeet Procedure...");

            var tries = 3;
            try
            {
                Trace.WriteLine("[Dashboard] Trying to Save summary and Analytics...");
                // n tries are made to save summary and analytics before ending the meet
                while (tries > 0 && summarySaved == false)
                {
                    if (testmode == false)
                    {
                        var allChats = _contentServer.GetAllMessages().ToArray();
                        summarySaved = _summarizer.SaveSummary(allChats);
                        _telemetry.SaveAnalytics(allChats);
                    }
                    // Fetching all the chats from the content module

                    if (testmode == true)
                    {
                        summarySaved = true;
                    }
                    tries--;
                }
                _sessionData.users.Clear();
                Trace.WriteLine("[Dashboard Server] Sending Client endMeet event");
                SendDataToClient("endMeet", _sessionData, null, null, null);
            }
            catch (Exception e)
            {
                // In case of any exception, the meeting is ended without saving the summary.
                // The user is notified about this
                SendDataToClient("endMeet", _sessionData, null, null, null);
            }
            Thread.Sleep(2000);
            // stopping the communicator and notifying UX server about the End Meet event.
            _communicator.Stop();
            //   _screenShareServer.Dispose();
            MeetingEnded?.Invoke();
            if (testmode == false)
            {
                // Environment.Exit(0);

            }
        }



        //     Fetches the chats from the content moudle and then asks telemetry to generate analytics on it.
        //     The analytics created are then sent to the client side again.
        private void GetAnalyticsProcedure(ClientToServerData receivedObject)
        {
            UserData user = new(receivedObject.username, receivedObject.userID, receivedObject.userEmail, receivedObject.photoUrl);



            try
            {
                // Fetching the chats and creating analytics on them
                if (testmode == false)
                {
                    var allChats = _contentServer.GetAllMessages().ToArray();
                    _sessionAnalytics = _telemetry.GetTelemetryAnalytics(allChats);
                }
                else
                {
                    _sessionAnalytics = new SessionAnalytics();
                }


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

        /// <summary>
        /// This function is for UX  to get session id info and session Mode
        /// </summary>
        /// <returns></returns>
        public SessionData GetSessionData()
        {
            return _sessionData;
        }


        //     This method is called when a request for getting summary reaches the server side.
        //     A summary is created along with a user object (with the ID and the name of the user who requested the summary)
        //     This data is then sent back to the client side.
        private void GetSummaryProcedure(ClientToServerData receivedObject)
        {
            var summaryData = CreateSummary();
            UserData user = new(receivedObject.username, receivedObject.userID, receivedObject.userEmail, receivedObject.photoUrl);
            Trace.WriteLine("Sending summary to client");
            SendDataToClient("getSummary", null, summaryData, null, user);
        }


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
            Trace.WriteLine("[Dashboard Server] In RemoveClientProcedure() removing user from sessionData");
            int userIDToRemove;
            if (userID == -1)
            {
                userIDToRemove = receivedObject.userID;
            }
            else
            {
                Trace.WriteLine("[Dashboard] Network called the RemoveClientProcedure() to remove user from sessionData");
                userIDToRemove = userID;
            }

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

        /// <summary>
        /// Function to send data from Server to client side of the session manager.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="sessionData"></param>
        /// <param name="summaryData"></param>
        /// <param name="sessionAnalytics"></param>
        /// <param name="user"></param>
        /// <param name="userId"></param>
        private void SendDataToClient(string eventName, SessionData sessionData, SummaryData summaryData,
           SessionAnalytics sessionAnalytics, UserData user, int userId = -1)
        {
            ServerToClientData serverToClientData;
            lock (this)
            {
                serverToClientData = new ServerToClientData(eventName, sessionData, summaryData, sessionAnalytics, user);
                string serializedSessionData = _serializer.Serialize(serverToClientData);

                if (userId == -1)
                {
                    Trace.WriteLine("[Dashboard]Sending To Network to braoadcast");
                    _communicator.Send(serializedSessionData, moduleIdentifier, null);
                }
                else
                {
                    Trace.WriteLine("[Dashboard] Sending To Network to notify to client ID:" + userId);
                    _communicator.Send(serializedSessionData, moduleIdentifier, userId.ToString());

                }
            }
        }
    }


}