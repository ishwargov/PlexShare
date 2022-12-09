/***********************************
 * Name : Saurabh Kumar
 * Roll : 111901046
 * Module : Dashboard
 * File Name :  ClientSessionManager.cs
 * This file contains the implemetation of ClientSessionManager
 ***********************************/
using Dashboard;
using PlexShare.Dashboard.Client.SessionManagement;
using PlexShareContent.Client;
using PlexShareDashboard.Dashboard.Server.Telemetry;
using PlexShareNetwork;
using PlexShareNetwork.Communication;
using PlexShareScreenshare.Client;
using PlexShareWhiteboard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace PlexShareDashboard.Dashboard.Client.SessionManagement
{
    public delegate void NotifyEndMeet();

    public delegate void NotifyAnalyticsCreated(SessionAnalytics analytics);

    public delegate void NotifySummaryCreated(string summary);

    public delegate void NotifySessionModeChanged(string sessionMode);

    /// <summary>
    ///  ClientSessionManager class is used to maintain the client side
    ///     session data and requests from the user. It communicates to the server session manager
    ///     to update the current session or to fetch summary and analytics.
    /// </summary>  
    public class ClientSessionManager : IUXClientSessionManager, INotificationHandler
    {
        private readonly List<IClientSessionNotifications> _clients;
        private readonly ICommunicator _communicator;
        private readonly IContentClient _contentClient;
        private readonly IDashboardSerializer _serializer;
        private readonly string moduleIdentifier;
        private string _chatSummary;
        public SessionData clientSessionData;

        private readonly ScreenshareClient _screenshareClient;

        private SessionAnalytics _sessionAnalytics;

        private UserData _user;
        private readonly bool testmode;

        //     Default constructor that will initialize communicator, contentclient,
        //     clientBoardStateManager and user side client data.
        public ClientSessionManager()
        {

            moduleIdentifier = "Dashboard";

            _serializer = new DashboardSerializer();
            _communicator = CommunicationFactory.GetCommunicator();
            _communicator.Subscribe(moduleIdentifier, this);
            _contentClient = ContentClientFactory.GetInstance();
            if (_clients == null) _clients = new List<IClientSessionNotifications>();
            clientSessionData = new SessionData();
            _user = null;
            _chatSummary = null;
            _screenshareClient = ScreenshareClient.GetInstance();
            Trace.WriteLine("[Dashboard] Created Client Session Manager");
        }

        //add constructor when testing
        public ClientSessionManager(ICommunicator communicator)
        {

            moduleIdentifier = "Dashboard";
            _serializer = new DashboardSerializer();
            _communicator = communicator;
            _communicator.Subscribe(moduleIdentifier, this);
            if (_clients == null) _clients = new List<IClientSessionNotifications>();
            clientSessionData = new SessionData();
            _chatSummary = null;
            testmode = true;
        }


        //     This function will handle the serialized data received from the networking module.
        //     It will first deserialize and then handle the appropriate cases.
        public void OnDataReceived(string serializedData)
        {
            if (serializedData == null)  //if recieved string is null
            {
                Trace.WriteLine("[Dashboard] Null Serialized Data recieved from network");
                throw new ArgumentNullException("[Dashboard] Null SerializedObject as Argument");
                // return;
            }
            Trace.WriteLine("[Dashboard] Data Recieved from Network");
            // Deserialize the data when it arrives
            var deserializedObject = _serializer.Deserialize<ServerToClientData>(serializedData);

            // check the event type and get the object sent from the server side
            var eventType = deserializedObject.eventType;

            // based on the type of event, calling the appropriate functions 
            switch (eventType)
            {
                case "toggleSessionMode":
                    UpdateClientSessionModeData(deserializedObject);
                    return;

                case "addClient":
                    UpdateClientSessionData(deserializedObject);
                    return;

                case "getSummary":
                    UpdateSummary(deserializedObject);
                    return;

                case "getAnalytics":
                    UpdateAnalytics(deserializedObject);
                    return;

                case "removeClient":
                    UpdateClientSessionData(deserializedObject);
                    return;

                case "endMeet":
                    CloseProgram();
                    return;

                case "newID":
                    SetClientID(deserializedObject);
                    return;

                default:
                    return;
            }
        }


        //     A helper function that sends data from Client to the server side. The data consists
        //     of the event type and the user who requested for that event.
        private void SendDataToServer(string eventName, string username, int userID = -1, string userEmail = null, string photoUrl = null)
        {
            ClientToServerData clientToServerData;
            lock (this)
            {
                clientToServerData = new ClientToServerData(eventName, username, userID, userEmail, photoUrl);
                var serializedData = _serializer.Serialize(clientToServerData);
                _communicator.Send(serializedData, moduleIdentifier, null);
            }
            Trace.WriteLine("[Dashboard] Data send to Network module to transfer Server");
        }


        //     Adds a user to the meeting.
        public bool AddClient(string ipAddress, int port, string username, string email = null, string photoUrl = null)  //added
        {
            Trace.WriteLine("[Dashboard] AddClient() is called");
            // Null or whitespace named users are not allowed
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            ipAddress = ipAddress.Trim();

            lock (this)
            {
                Trace.WriteLine("[Dashboard] Sending to Network for connecting");
                // trying to connect
                var connectionStatus = _communicator.Start(ipAddress, port.ToString());

                // if the IP address and/or the port number are incorrect
                if (connectionStatus == "failure")
                {
                    Trace.WriteLine("[Dashboard] Connection not established");
                    return false;
                }
            }
            Trace.WriteLine("[Dashboard] Connection established");
            _user = new(username, -1, email, photoUrl);
            return true;
        }

        //     End the meeting for all, creating and storing the summary and analytics.
        public void EndMeet()
        {
            Trace.WriteLine("[Dashboard] End Meet is called from Dashboard UX. Sending to Server to End Meet");
            SendDataToServer("endMeet", _user.username, _user.userID);
        }

        //     Gather analytics of the users and messages.
        public void GetAnalytics()
        {
            Trace.WriteLine("[Dashboard] GetAnalytics() is called from Dashboard UX");

            SendDataToServer("getAnalytics", _user.username, _user.userID);
        }

        //     Get the summary of the chats that were sent from the start of the
        //     meet till the function was called.
        public void GetSummary()
        {
            Trace.WriteLine("[Dashboard] GetSummary() is called from Dashboard UX");
            SendDataToServer("getSummary", _user.username, _user.userID);
        }

        //     Fetches the Userdata object of the client.
        public UserData GetUser() //aded
        {
            Trace.WriteLine("[Dashboard] GetUser() is Called from Dashboard UX");
            return _user;
        }

        //change the session mode from lab mode to exam mode and vice versa
        public void ToggleSessionMode()
        {
            Trace.WriteLine("[Dashboard] ToggleSessionMode() is Called from Dashboard UX");
            SendDataToServer("toggleSession", _user.username, _user.userID);
        }




        //     Removes the user from the meeting by deleting their
        ///     data from the session.
        public void RemoveClient()
        {

            // Asking the server to remove client from the server side.
            SendDataToServer("removeClient", _user.username, _user.userID);

            Thread.Sleep(2000);

            // Stopping the network communicator.
            _communicator.Stop();
            MeetingEnded?.Invoke();
            if (testmode == false)
            {
                CloseProgram();

            }

        }

        //     Used to subcribe for any changes in the
        ///     Session object.
        public void SubscribeSession(IClientSessionNotifications listener)
        {
            lock (this)
            {
                _clients.Add(listener);
            }
        }

        public event NotifyEndMeet MeetingEnded;
        public event NotifySummaryCreated SummaryCreated;
        public event NotifyAnalyticsCreated AnalyticsCreated;
        public event NotifySessionModeChanged SessionModeChanged;

        //     Used to fetch the sessionData for the client.
        public SessionData GetSessionData()
        {
            Trace.WriteLine("[Dashboard] Sending Session Data to Caller. ");
            return clientSessionData;
        }

        public SessionAnalytics GetStoredAnalytics()
        {
            return _sessionAnalytics;
        }

        //     Used to fetch the stored summary for the client. Helpful for testing and debugging.
        public string GetStoredSummary()
        {
            return _chatSummary;
        }

        //     Will Notifiy UX about the changes in the Session
        public void NotifyUXSession()
        {
            for (var i = 0; i < _clients.Count; ++i)
                lock (this)
                {
                    Trace.WriteLine("[Dashboard] Notifying subscribed UX about the session change. ");
                    _clients[i].OnClientSessionChanged(clientSessionData);
                }
        }

        private void SetClientID(ServerToClientData receivedData)
        {
            if (_user.userID == -1)
            {
                lock (this)
                {

                    _user.userID = receivedData._user.userID;

                    // upon successfull connection, the request to add the client is sent to the server side.
                    SendDataToServer("addClient", _user.username, _user.userID, _user.userEmail, _user.userPhotoUrl);
                    // clientBoardStateManager.SetUser(_user.userID.ToString());
                    // Whiteboard's user ID set.;
                    if (testmode == false)
                    {
                        WhiteBoardViewModel WBviewModel = WhiteBoardViewModel.Instance;
                        WBviewModel.SetUserId(_user.userID);



                        // ScreenShare's user ID and username set.
                        _screenshareClient.SetUser(_user.userID.ToString(), _user.username);

                        ContentClientFactory.SetUser(_user.userID);
                        // Content's user ID set. 
                    }
                }
            }
        }

        //for testing we will be adding setuser
        public void SetUser(string userName, int userID = 1, string userEmail = null, string photoUrl = null)
        {
            _user = new UserData(userName, userID, userEmail, photoUrl);
        }

        //for testing we will add set session data
        public void SetSessionUsers(List<UserData> users)
        {
            // _clientSessionData.users = users;
            for (int i = 0; i < users.Count; ++i)
            {
                clientSessionData.AddUser(users[i]);
            }

        }

        private void UpdateAnalytics(ServerToClientData receivedData)
        {
            _sessionAnalytics = receivedData.sessionAnalytics;
            var receiveduser = receivedData.GetUser();
            Trace.WriteLine("Notifying UX about the Analytics.");

            AnalyticsCreated?.Invoke(_sessionAnalytics);
        }


        //     Updates the locally stored summary at the client side to the summary received from the
        //     server side. The summary will only be updated fro the user who requsted it.
        private void UpdateSummary(ServerToClientData receivedData)
        {
            // Extract the summary string and the user.
            var receivedSummary = receivedData.summaryData;
            var receivedUser = receivedData.GetUser();

            // check if the current user is the one who requested to get the summary
            if (receivedUser.userID == _user.userID)
                lock (this)
                {
                    _chatSummary = receivedSummary.summary;
                    Trace.WriteLine("Notifying UX about the summary.");
                    SummaryCreated?.Invoke(_chatSummary);
                }
        }

        //update client side session data
        private void UpdateClientSessionModeData(ServerToClientData receivedData)
        {
            // fetching the session data and user received from the server side
            var receivedSessionData = receivedData.sessionData;

            // update the session data on the client side and notify the UX about it.
            lock (this)
            {
                clientSessionData = receivedSessionData;
            }
            SessionModeChanged?.Invoke(clientSessionData.sessionMode);
            NotifyUXSession();
        }


        //     Compares the server side session data to the client side and update the
        //     client side data if they are different.
        private void UpdateClientSessionData(ServerToClientData receivedData)
        {
            // fetching the session data and user received from the server side
            var receivedSessionData = receivedData.sessionData;
            var user = receivedData.GetUser();


            // if there was no change in the data then nothing needs to be done
            if (receivedSessionData != null && clientSessionData != null &&
                receivedSessionData.users.Equals(clientSessionData.users))
                return;

            // a null _user denotes that the user is new and has not be set because all 
            // the old user (already present in the meeting) have their _user set.
            if (_user == null)
            {
                _user = user;
                // Client added to the client session.");


            }

            // The user received from the server side is equal to _user only in the case of 
            // client departure. So, the _user and received session data are set to null to indicate this departure
            else if (_user.Equals(user) && receivedData.eventType == "removeClient")
            {
                _user = null;
                // Client removed from the client session data.
                receivedSessionData = null;
            }

            // update the session data on the client side and notify the UX about it.
            lock (this)
            {
                clientSessionData = receivedSessionData;
            }

            NotifyUXSession();
        }

        public void CloseProgram()
        {
            Trace.WriteLine("[Dashboard] Calling Network to Stop listening ");
            _communicator.Stop();
            MeetingEnded?.Invoke();

            Trace.WriteLine("[Dashboard] Shutdown Application");

            //if (testmode == false)
            //{
            //    Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            //    {
            //        Application.Current.Shutdown();
            //        System.Environment.Exit(0);
            //    });
            //}
            return;
        }

    }
}