/*
 * Name : Saurabh Kumar
 * Roll : 111901046
 * FileName: SessionManagementTest.cs
 * This file is used for testing the SessionManager functionality
 */
using Dashboard;
using Dashboard.Server.SessionManagement;
using PlexShare.Dashboard;
using PlexShareDashboard.Dashboard;
using PlexShareDashboard.Dashboard.Client.SessionManagement;
using PlexShareDashboard.Dashboard.Server.SessionManagement;
using PlexShareTests.DashboardTests.SessionManagement.TestModules;


namespace PlexShareTests.DashboardTests.SessionManagement
{
    public class SessionManagementTest
    {
        private FakeCommunicator _fakeCommunicator = new FakeCommunicator();
        private IDashboardSerializer _serializer = new DashboardSerializer();
        private ClientSessionManager _clientSessionManager;
        private ClientSessionManager _clientSessionManagerLast;
        private ClientSessionManager _clientSessionManagerNew;
        private ServerSessionManager _serverSessionManager;

        /// <summary>
        /// Creating SetUp function which will be called while testing for instantiation
        /// </summary>

        public void Setup()
        {
            _fakeCommunicator = new FakeCommunicator();
            _serializer = new DashboardSerializer();
            _clientSessionManager = SessionManagerFactory.GetClientSessionManager(_fakeCommunicator);
            _serverSessionManager = SessionManagerFactory.GetServerSessionManager(_fakeCommunicator);
        }

        /// <summary>
        /// This test is for checking the sigleton functionality.
        /// That is when trying to instantiate the same class it will return the same object which was already instantiated
        /// </summary>

        [Fact]
        public void GetClientSessionManager_TwoInstancesCreated_MustHaveSameReference()
        {
            IUXClientSessionManager clientSessionManager1 = SessionManagerFactory.GetClientSessionManager();
            IUXClientSessionManager clientSessionManager2 = SessionManagerFactory.GetClientSessionManager();

            //Assert that both the instance have the same reference
            Assert.True(clientSessionManager2.Equals(clientSessionManager1));
        }

        /// <summary>
        /// This test is for checking the sigleton functionality.
        /// That is when trying to instantiate the same class it will return the same object which was already instantiated
        /// </summary>
        [Fact]
        public void GetServerSessionManager_TwoInstancesCreated_MustHaveSameReference()
        {
            IUXServerSessionManager serverSessionManager1 = SessionManagerFactory.GetServerSessionManager();
            IUXServerSessionManager serverSessionManager2 = SessionManagerFactory.GetServerSessionManager();
            //Assert that both the instance have the same reference
            Assert.True(serverSessionManager1.Equals(serverSessionManager2));
        }

        /// <summary>
        /// This is test for checking if the the UX is getting notified when the sessionData gets changed
        /// </summary>
        [Fact]
        public void NotifyUX_SessionDataChanges_UXShouldBeNotified()
        {
            //Arrange
            Setup();
            FakeClientUX fakeClientUX = new(_clientSessionManager);
            fakeClientUX.sessionSummary = null;

            //Act
            //get one user from the util
            var users = Utils.GetUsers();
            //Add the user in the Session
            _clientSessionManager.SetSessionUsers(users);
            //Notify the UX about the changes made in the client side
            _clientSessionManager.NotifyUXSession();

            //Assert
            Assert.Equal(users, fakeClientUX.sessionData.users);
        }

        /// <summary>
        /// This test is for the function GetPortsAndIPAddress() which will 
        /// fetch ip and port of the server from the networking
        /// </summary>
        /// <param name="inputMeetAddress"></param>
        [Theory]
        [InlineData("192.168.1.1:8080")]
        [InlineData("195.148.23.101:8585")]
        [InlineData("223.152.44.2:2222")]
        public void GetPortsAndIPAddress_ValidAddress_ReturnsTrue(string inputMeetAddress)
        {
            //Arrange
            Setup();

            //Act
            //Assigning the meeting credential of server
            _fakeCommunicator.meetAddress = inputMeetAddress;
            //calling the GetPortsAndIPAddress() function for fetching the meeting credential.
            var meetCreds = _serverSessionManager.GetPortsAndIPAddress();
            var returnedMeetAddress = meetCreds.ipAddress + ":" + meetCreds.port;

            //Assert
            Assert.Equal(_fakeCommunicator.meetAddress, returnedMeetAddress);
        }

        /// <summary>
        /// This is used for checking the functionality if the user has given the invalid meeting credential 
        /// while joining.
        /// </summary>
        /// <param name="inputMeetAddress"></param>
        [Theory]
        [InlineData("")]
        [InlineData("256.0.1.3:8080")]
        [InlineData("2$.3$%.5:3512")]
        [InlineData("192.168.1.1:70000")]
        [InlineData(null)]
        [InlineData("3gdgfjh")]
        public void GetPortsAndIPAddress_ValidAddress_ReturnsNull(string inputMeetAddress)
        {
            Setup();
            _fakeCommunicator.meetAddress = inputMeetAddress;
            var meetCreds = _serverSessionManager.GetPortsAndIPAddress();
            Assert.Null(meetCreds);
        }

        /// <summary>
        /// This function is to test the Dashboard Serializer
        /// </summary>
        [Fact]
        public void Serializing_Deserializing()
        {
            Setup();
            ClientToServerData clientToServerData1 = new ClientToServerData("addClient", "Saurabh", 2);
            var serialized = _serializer.Serialize(clientToServerData1);
            ClientToServerData clienToServerData2 = _serializer.Deserialize<ClientToServerData>(serialized);
            Assert.True(clientToServerData1.eventType == clienToServerData2.eventType);
            Assert.True(clientToServerData1.userID == clienToServerData2.userID);
            Assert.True(clientToServerData1.username == clienToServerData2.username);

        }


        /// <summary>
        /// This function checks if the client is added in the session
        /// if it gives the correct meeting credentials
        /// </summary>
        /// <param name="meetAddress"></param>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        [Theory]
        [InlineData("192.168.20.1:8080", "192.168.20.1", 8080, "Jake Vickers")]
        [InlineData("192.168.201.4:480", "192.168.201.4", 480, "Antonio")]
        public void AddClient_ValidCredentials_ReturnsTrue(string meetAddress, string ipAddress, int port,
           string username)
        {
            Setup();
            _fakeCommunicator.meetAddress = meetAddress;
            var clientAdded = _clientSessionManager.AddClient(ipAddress, port, username);
            Assert.True(clientAdded);
        }

        /// <summary>
        /// This function checks if the client is added in the session
        /// if it gives the correct meeting credentials
        /// </summary>
        /// <param name="meetAddress"></param>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        [Theory]
        [InlineData("", "", 51, "")]
        [InlineData(null, null, null, null)]
        [InlineData(null, "162.212.3.1", 20, "Chang Jia-han")]
        [InlineData("192.168.201.4:480", "192.230.201.4", 480, "Antonio")]
        [InlineData("192.168.20.1:8080", "192.168.20.1", 8081, "Jake Vickers")]
        public void AddClient_InvalidCredentials_ReturnsFalse(string meetAddress, string ipAddress, int port, string username)
        {
            Setup();
            _fakeCommunicator.meetAddress = meetAddress;
            var clientAdded = _clientSessionManager.AddClient(ipAddress, port, username);
            Assert.False(clientAdded);
        }

        /// <summary>
        /// This test the working of the ClientArrivalProcedure() functionality
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        [Theory]
        [InlineData("192.168.1.1", 8080, "Jake")]
        [InlineData("192.168.1.1", 8080, "Lake")]
        [InlineData("192.168.1.1", 8080, "Bake")]
        public void ClientArrivalProcedure_ClientArrives_BroadcastsNewUser(string ipAddress, int port, string username)
        {
            Setup();

            Console.WriteLine("Session Before\n\t" + _clientSessionManager.GetSessionData());
            var clientAdded = _clientSessionManager.AddClient(ipAddress, port, username);
            //Network would call the OnClientJoined() 
            _serverSessionManager.OnClientJoined(null);

            ServerToClientData serverToClientData = _serializer.Deserialize<ServerToClientData>(_fakeCommunicator.transferredData);
            int userID = serverToClientData._user.userID;
            ClientToServerData clientToServerData = new ClientToServerData("addClient", username, userID);
            var serialized = _serializer.Serialize(clientToServerData);

            //serverSessionManager when recieves the data from network about addclient event,
            //it will add the user into the session
            //and then sendDataToClient to update Client Session data
            _serverSessionManager.OnDataReceived(serialized);
            var s1 = _fakeCommunicator.transferredData;

            //client session Manager will update the client session data
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            Assert.Equal(s1, _fakeCommunicator.transferredData);
            Console.WriteLine("Session After\n\t" + _clientSessionManager.GetSessionData());
            UserData updatedUser = _clientSessionManager.GetUser();
            Assert.Equal(username, updatedUser.username);
            Assert.NotNull(updatedUser.userID);
        }

        /// <summary>
        /// This is used to test when user is joining to meeting in clientsession and AddClientProcedure in the server side
        /// </summary>
        /// <param name="username"></param>
        [Theory]
        [InlineData("Jake")]
        [InlineData(null)]
        public void AddClientProcedureServerSide_ClientArrives_NewClientAddedToServer(string username)
        {
            Setup();
            _clientSessionManager.SetUser(username, -1);
            ClientToServerData clientToServerData = new("addClient", username);
            string serializedData = _serializer.Serialize(clientToServerData);
            _serverSessionManager.OnClientJoined(null);
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            try
            {
                _serverSessionManager.OnDataReceived(serializedData);
                var serverToClientData = _serializer.Deserialize<ServerToClientData>(_fakeCommunicator.transferredData);
                var receiveduser = serverToClientData.GetUser();
                Assert.Equal("addClient", serverToClientData.eventType);
                Assert.Equal(receiveduser.username, username);
                Assert.NotEqual(-1, _clientSessionManager.GetUser().userID);
            }
            catch (Exception e)
            {
                Assert.Equal("Value cannot be null.", e.Message);
            }
        }

        /// <summary>
        /// This is used to test AddClientProcedureServerSide function when Multiple Clients Arrives 
        /// </summary>
        [Fact]
        public void AddClientProcedureServerSide_MultipleClientsArrives_UsersAddedToServerSession()
        {
            Setup();
            //Arrange

            // Clients that arrives are added to the server side
            var users = Utils.GetUsers();

            //Act

            for (int i = 0; i < users.Count; i++)
            {
                ClientToServerData clientToServerData = new("addClient", users[i].username, users[i].userID);
                var serializedData = _serializer.Serialize(clientToServerData);

                _serverSessionManager.OnClientJoined(null);
                _serverSessionManager.ClientArrivalProcedure(clientToServerData);
            }

            // The updated session data which includes new users is now sent from server to the client side
            // the deserializedData.sessionData is the updated session received from the server 
            var returnedSessionData = _serverSessionManager.GetSessionData();

            //Assert

            // The recieved session must not be null and have the same users that were added
            Assert.NotNull(returnedSessionData);
            Assert.Equal(1, returnedSessionData.users[0].userID);


            Assert.Equal(users.Count, returnedSessionData.users.Count);

            for (int i = 0; i < users.Count; i++)
            {
                Assert.Equal(users[i].username, returnedSessionData.users[i].username);
                Assert.Equal(users[i].userID, returnedSessionData.users[i].userID);

            }

        }

        /// <summary>
        /// This function is used to test Update Session Data function when Client Arrives
        /// </summary>
        [Fact]
        public void UpdatingSessionDataOnArrival_ClientArrives_ClientSessionUpdated()
        {
            //Arrange

            // Client session managers for the nth and n+1 th user respectively
            _clientSessionManagerLast = new ClientSessionManager(_fakeCommunicator);
            _clientSessionManagerNew = new ClientSessionManager(_fakeCommunicator);

            var serverSession = Utils.GetSessionData();

            //Act
            var indexLastUser = serverSession.users.Count - 1;
            var lastUser = serverSession.users[indexLastUser];

            // Till now, the nth user has arrived and the server session data has been updated
            // Now, the server will send the new session to the client side to update it
            ServerToClientData serverToClientData = new("addClient", serverSession, null, null, lastUser);
            var serializedData = _serializer.Serialize(serverToClientData);

            // Updating the client side session for the nth user
            _clientSessionManagerLast.OnDataReceived(serializedData);

            // The (n+1)th user arrives and the server session data is updated
            UserData newUser = new("Yuzuhiko", serverSession.users.Count + 1);
            serverSession.AddUser(newUser);

            // Server Notifies the Client side about the addition of the new user
            ServerToClientData serverToClientDataNew = new("addClient", serverSession, null, null, newUser);
            var serializedDataNew = _serializer.Serialize(serverToClientDataNew);

            // Updating the already present nth users session
            _clientSessionManagerLast.OnDataReceived(serializedDataNew);

            // Updating the new user's session
            _clientSessionManagerNew.OnDataReceived(serializedDataNew);


            //Assert

            // Assertion to check if both nth and the (n+1)th user have the same session
            Assert.NotNull(_clientSessionManagerLast.GetUser());
            Assert.NotNull(_clientSessionManagerNew.GetUser());
            Assert.NotNull(_clientSessionManagerLast.GetSessionData());
            Assert.NotNull(_clientSessionManagerNew.GetSessionData());

            SessionData sessionDataNew = _clientSessionManagerNew.GetSessionData();
            SessionData sessionDataLast = _clientSessionManagerLast.GetSessionData();
            for (int i = 0; i < sessionDataNew.users.Count; i++)
            {
                Assert.Equal(sessionDataNew.users[i].username, sessionDataLast.users[i].username);
                Assert.Equal(sessionDataNew.users[i].userID, sessionDataLast.users[i].userID);
            }

            Assert.Equal(serverSession.users.Count, _clientSessionManagerLast.GetSessionData().users.Count);
        }

        /// <summary>
        /// This is used to test functionality of GetSummary when client request for Summary
        /// </summary>
        [Fact]
        public void GetSummary_RequestSummary_ReturnsSummary()
        {
            //Arrange
            Setup();
            FakeClientUX fakeClientUX = new(_clientSessionManager);
            fakeClientUX.sessionSummary = null;

            //Act
            UserData user = new("Jake Vickers", 1);

            _clientSessionManager.SetUser(user.username, user.userID);
            _clientSessionManager.SetSessionUsers(new List<UserData> { user });

            ClientToServerData clientToServerData = new("addClient", user.username);
            var serializedData = _serializer.Serialize(clientToServerData);
            _serverSessionManager.OnClientJoined(null);
            _serverSessionManager.OnDataReceived(serializedData);
            _clientSessionManager.GetSummary();
            _serverSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            var clientSummary = _clientSessionManager.GetStoredSummary();
            var serverSummary = _serverSessionManager.GetStoredSummary();
            //Assert
            Assert.NotNull(clientSummary);
            Assert.NotNull(serverSummary);
            Assert.NotNull(fakeClientUX.sessionSummary);
        }

        /// <summary>
        /// This is used to test GetAnalytics Functionality when client request for Analytics
        /// </summary>
        [Fact]
        public void GetAnalytics_RequestAnalytics_ReturnSessionAnalyticsAndNotifyUX()
        {
            //Arrange
            Setup();
            FakeClientUX fakeClientUX = new(_clientSessionManager);
            FakeTelemetry fakeTelemetry = new(_serverSessionManager);

            //Act
            UserData user = new("Jake Vickers", 1);

            _clientSessionManager.SetUser(user.username, user.userID);
            _clientSessionManager.SetSessionUsers(new List<UserData> { user });

            _clientSessionManager.GetAnalytics();

            _serverSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            //Assert
            Assert.NotNull(_clientSessionManager.GetStoredAnalytics());
            Assert.NotNull(fakeClientUX.sessionAnalytics);
        }

        /// <summary>
        /// This is used test functionality of ToggleSession Mode
        /// </summary>
        [Fact]
        public void ToggleSessionMode_ReauestedSessionChange_ServerSessionModeUpdatedAndNotifyUX()
        {
            //Arrange 
            Setup();
            FakeClientUX fakeClientUX = new(_clientSessionManager);
            FakeTelemetry fakeTelemetry = new(_serverSessionManager);
            UserData user = new("Jake Vickers", 1);

            //Act
            _clientSessionManager.SetUser(user.username, user.userID);
            _clientSessionManager.SetSessionUsers(new List<UserData> { user });

            _clientSessionManager.ToggleSessionMode();
            _serverSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            //Assert
            Assert.Equal("ExamMode", _serverSessionManager.GetSessionData().sessionMode);
            Assert.Equal(_clientSessionManager.GetSessionData().sessionMode, _serverSessionManager.GetSessionData().sessionMode);
            Assert.Equal("ExamMode", fakeClientUX.sessionMode);

            //Act
            _clientSessionManager.ToggleSessionMode();
            _serverSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            //Assert
            Assert.Equal("LabMode", _serverSessionManager.GetSessionData().sessionMode);
            Assert.Equal(_clientSessionManager.GetSessionData().sessionMode, _serverSessionManager.GetSessionData().sessionMode);
            Assert.Equal("LabMode", fakeClientUX.sessionMode);

        }

        /// <summary>
        /// This is test RemoveClient functionality when a client leaves the meeting
        /// </summary>
        [Fact]
        public void RemoveClient_ClientDeparture_UserRemovedFromServerAndClientSide()
        {
            //Arrange
            Setup();
            var users = Utils.GetUsersSet2();

            //Act
            for (int i = 0; i < users.Count; i++)
            {
                ClientToServerData clientToServerData = new("addClient", users[i].username, users[i].userID);
                var serializedData = _serializer.Serialize(clientToServerData);

                _serverSessionManager.OnClientJoined(null);
                _serverSessionManager.ClientArrivalProcedure(clientToServerData);
            }

            _clientSessionManager.SetUser(users.Last().username, users.Last().userID);
            _clientSessionManager.SetSessionUsers(users);

            // The last user in the list departs
            users.Remove(users.Last());
            _clientSessionManager.RemoveClient();
            _serverSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            //Assert
            Assert.Null(_clientSessionManager.GetUser());
            Assert.Null(_clientSessionManager.GetSessionData());
            Assert.Equal(users.Count, _serverSessionManager.GetSessionData().users.Count);
        }

        /// <summary>
        /// This is to check functionality of End Meet
        /// </summary>
        [Fact]
        public void EndMeet_MeetingEnded_UXNotified()
        {
            Setup();
            FakeClientUX fakeClientUx = new(_clientSessionManager);
            FakeServerUX fakeServerUX = new(_serverSessionManager);

            fakeClientUx.meetingEnded = false;
            fakeServerUX.meetingEnded = false;

            var users = Utils.GetUsers();

            for (var i = 0; i < users.Count; ++i)
            {
                ClientToServerData clientToServerData = new("addClient", users[i].username);
                var serializedData = _serializer.Serialize(clientToServerData);

                _serverSessionManager.OnClientJoined(null);
                _serverSessionManager.OnDataReceived(serializedData);
            }

            _clientSessionManager.SetUser(users.Last().username, users.Last().userID);
            _clientSessionManager.SetSessionUsers(users);
            _clientSessionManager.EndMeet();
            _serverSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            Assert.True(fakeServerUX.meetingEnded);
            Assert.True(fakeClientUx.meetingEnded);
            Assert.True(_serverSessionManager.summarySaved);
        }

        /// <summary>
        /// This is to check if Null Data is recieved on ServerSide
        /// </summary>
        [Fact]
        public void OnDataReceivedServerSide_SendingNullData_TraceAndReturn()
        {
            Setup();
            try
            {
                _serverSessionManager.OnDataReceived(null);
            }
            catch (ArgumentNullException e)
            {
                Assert.Equal("Value cannot be null. (Parameter 'Null serializedObject Exception')", e.Message);
            }
        }

        /// <summary>
        /// This is to check if Null Data is recieved on ClientSide
        /// </summary>
        [Fact]
        public void OnDataReceivedClientSide_SendingNullData_TraceAndReturn()
        {
            Setup();
            try
            {
                _clientSessionManager.OnDataReceived(null);
            }
            catch (Exception e)
            {
                Assert.Equal("Value cannot be null. (Parameter '[Dashboard] Null SerializedObject as Argument')", e.Message);
            }
        }
        /// <summary>
        /// To check the functionality of OnClientLeft when a client Disconnects
        /// </summary>
        [Fact]
        public void OnClientLeft_ClientDisconnects_UserRemoved_ServerSessionChanged()
        {
            //Arrange
            Setup();
            // Adding the users to the session and the client side
            var users = Utils.GetUsers();

            //Act

            for (int i = 0; i < users.Count; i++)
            {
                ClientToServerData clientToServerData = new("addClient", users[i].username, users[i].userID);
                var serializedData = _serializer.Serialize(clientToServerData);

                _serverSessionManager.OnClientJoined(null);
                _serverSessionManager.ClientArrivalProcedure(clientToServerData);
            }
            _clientSessionManager.SetUser(users.Last().username, users.Last().userID);
            _clientSessionManager.SetSessionUsers(users);

            // removing the last user from the meet because of the disconnection
            var disconnectedUser = users.Last();
            users.Remove(users.Last());

            // The client disconnects and the client side is notified
            _serverSessionManager.OnClientLeft(disconnectedUser.userID.ToString());
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            //Assert
            // Check if the session on the server side was updated and the user and session data on the client side are removed.
            Assert.Null(_clientSessionManager.GetSessionData());
            Assert.Null(_clientSessionManager.GetUser());
        }

        /// <summary>
        /// This function is used to check functionality of both End Meet and when last user leaves
        /// </summary>
        [Fact]
        public void EndMeet_LastUserLeaves_MeetingShouldEnd()
        {
            //Arrange
            Setup();
            FakeClientUX fakeClientUx = new(_clientSessionManager);
            FakeServerUX fakeServerUX = new(_serverSessionManager);

            fakeClientUx.meetingEnded = false;
            fakeServerUX.meetingEnded = false;

            //Act

            //Adding the users 
            List<UserData> users = new();
            users.Add(new UserData("Justin", 1));
            for (int i = 0; i < users.Count; i++)
            {
                ClientToServerData _clientToServerData = new("addClient", users[i].username, users[i].userID);
                var serializedData = _serializer.Serialize(_clientToServerData);
                _serverSessionManager.OnClientJoined(null);
                _serverSessionManager.OnDataReceived(serializedData);
            }

            _clientSessionManager.SetSessionUsers(users);
            _clientSessionManager.SetUser(users[0].username, users[0].userID);

            _clientSessionManager.RemoveClient();
            _serverSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            //Assert

            Assert.True(fakeServerUX.meetingEnded);
            Assert.True(fakeClientUx.meetingEnded);
            Assert.True(_serverSessionManager.summarySaved);
        }


    }

}
