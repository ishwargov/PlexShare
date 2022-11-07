using Dashboard;
using Dashboard.Server.SessionManagement;
using PlexShare.Dashboard;
using PlexShareDashboard.Dashboard.Client.SessionManagement;
using PlexShareDashboard.Dashboard.Server.SessionManagement;
using PlexShareDashboard.Dashboard.Server.Telemetry;
using PlexShareDashboard.Dashboard;
using PlexShareTests.DashboardTests.SessionManagement.TestModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using PlexShareNetwork.Serialization;
using NuGet.Frameworks;

namespace PlexShareTests.DashboardTests.SessionManagement
{
    public class SessionManagementTest
    {
       private  FakeCommunicator _fakeCommunicator = new FakeCommunicator();
        private FakeContentServer _fakeContentServer = new FakeContentServer();
        private IDashboardSerializer _serializer = new DashboardSerializer();
        private ClientSessionManager _clientSessionManager; 
        private ClientSessionManager _clientSessionManagerLast;
        private ClientSessionManager _clientSessionManagerNew;

       
        private ServerSessionManager _serverSessionManager = SessionManagerFactory.GetServerSessionManager();


        public void Setup()
        {
            _fakeCommunicator = new FakeCommunicator();
            _fakeContentServer = new FakeContentServer();
            _serializer = new DashboardSerializer();
            _clientSessionManager = SessionManagerFactory.GetClientSessionManager(_fakeCommunicator);
            _serverSessionManager = SessionManagerFactory.GetServerSessionManager(_fakeCommunicator);
        }

        [Fact]
        public void GetClientSessionManager_TwoInstancesCreated_MustHaveSameReference()
        {
            IUXClientSessionManager clientSessionManager1 = SessionManagerFactory.GetClientSessionManager();
            IUXClientSessionManager clientSessionManager2 = SessionManagerFactory.GetClientSessionManager();

            
            Assert.True(clientSessionManager2.Equals(clientSessionManager1));
        }

        [Fact]
        public void GetServerSessionManager_TwoInstancesCreated_MustHaveSameReference()
        {
            IUXServerSessionManager serverSessionManager1 = SessionManagerFactory.GetServerSessionManager();
            IUXServerSessionManager serverSessionManager2 = SessionManagerFactory.GetServerSessionManager();

            Assert.True(serverSessionManager1.Equals(serverSessionManager2));
        }

        [Fact]
        public void NotifyUX_SessionDataChanges_UXShouldBeNotified()
        {
            Setup();
            FakeClientUX fakeClientUX = new(_clientSessionManager);
            fakeClientUX.sessionSummary = null;

            var users = Utils.GetUsers();
           
            _clientSessionManager.SetSessionUsers(users);

            _clientSessionManager.NotifyUXSession();

            Assert.Equal(users, fakeClientUX.sessionData.users);
        }

        [Theory]
        [InlineData("192.168.1.1:8080")]
        [InlineData("195.148.23.101:8585")]
        [InlineData("223.152.44.2:2222")]
        public void GetPortsAndIPAddress_ValidAddress_ReturnsTrue(string inputMeetAddress)
        {
            Setup();
            _fakeCommunicator.meetAddress = inputMeetAddress;
            //IUXServerSessionManager serverSessionManager = SessionManagerFactory.GetServerSessionManager(_fakeCommunicator);
            var meetCreds = _serverSessionManager.GetPortsAndIPAddress();
            var returnedMeetAddress = meetCreds.ipAddress + ":" + meetCreds.port;
            Assert.Equal(_fakeCommunicator.meetAddress, returnedMeetAddress);
        }
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
           // IUXServerSessionManager serverSessionManager = SessionManagerFactory.GetServerSessionManager(_fakeCommunicator);
            var meetCreds = _serverSessionManager.GetPortsAndIPAddress();
            Assert.Equal(meetCreds,null);
        }

        [Theory]
        [InlineData("192.168.20.1:8080", "192.168.20.1", 8080, "Jake Vickers")]
        [InlineData("192.168.201.4:480", "192.168.201.4", 480, "Antonio")]
        public void AddClient_ValidCredentials_ReturnsTrue(string meetAddress, string ipAddress, int port,
           string username)
        {
            Setup();
            _fakeCommunicator.meetAddress = meetAddress;
           // IUXClientSessionManager clientSessionManager = SessionManagerFactory.GetClientSessionManager(_fakeCommunicator);
            var clientAdded = _clientSessionManager.AddClient(ipAddress, port, username);
            Assert.Equal(true, clientAdded);
        }

        [Theory]
        [InlineData("", "", 51, "")]
        [InlineData(null, null, null, null)]
        [InlineData(null, "162.212.3.1", 20, "Chang Jia-han")]
        [InlineData("192.168.201.4:480", "192.230.201.4", 480, "Antonio")]
        [InlineData("192.168.20.1:8080", "192.168.20.1", 8081, "Jake Vickers")]
        public void AddClient_InvalidCredentials_ReturnsFalse(string meetAddress, string ipAddress, int port,
            string username)
        {
            Setup();
            _fakeCommunicator.meetAddress = meetAddress;
          //  IUXClientSessionManager clientSessionManager = SessionManagerFactory.GetClientSessionManager(_fakeCommunicator);
            var clientAdded = _clientSessionManager.AddClient(ipAddress, port, username);
            Assert.Equal(false, clientAdded);
        }

        [Theory]
        [InlineData("192.168.1.1", 8080, "Jake")]
        [InlineData("192.168.1.1", 8080, "Lake")]
        [InlineData("192.168.1.1", 8080, "Bake")]
        public void ClientArrivalProcedure_ClientArrives_BroadcastsNewUser(string ipAddress, int port, string username)
        {
            Setup();
           
            Console.WriteLine("Session Before\n\t" + _clientSessionManager.GetSessionData());
            var clientAdded = _clientSessionManager.AddClient(ipAddress, port, username);
            
            _serverSessionManager.OnClientJoined<TcpClient>(null);
 
            ServerToClientData serverToClientData = _serializer.Deserialize<ServerToClientData>(_fakeCommunicator.transferredData);
            int userID = serverToClientData._user.userID;
            string userName = serverToClientData._user.username;
            ClientToServerData clientToServerData = new ClientToServerData("addClient",userName, userID);
            var serialized = _serializer.Serialize(clientToServerData);
            _serverSessionManager.OnDataReceived(serialized);
            var s1 = _fakeCommunicator.transferredData;

            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            Assert.Equal(s1, _fakeCommunicator.transferredData);

            Console.WriteLine("Session After\n\t" + _clientSessionManager.GetSessionData());
            UserData updatedUser = _clientSessionManager.GetUser();
            Assert.Equal(updatedUser.username, username);
            Assert.NotNull(updatedUser.userID);
          
          
        }

        [Theory]
        [InlineData("Jake")]
        [InlineData (null)]
        public void AddClientProcedureServerSide_ClientArrives_NewClientAddedToServer(string username)
        {
            ServerSessionManager serverSessionManager = SessionManagerFactory.GetServerSessionManager(_fakeCommunicator);
            ClientSessionManager clientSessionManager = SessionManagerFactory.GetClientSessionManager(_fakeCommunicator);
            ClientToServerData clientToServerData = new("addClient", username);
            string serializedData = _serializer.Serialize(clientToServerData);
         
            serverSessionManager.OnClientJoined<TcpClient>(null);

            try
            {
                serverSessionManager.OnDataReceived(serializedData);
                var serverToClientData = _serializer.Deserialize<ServerToClientData>(_fakeCommunicator.transferredData);
                var receiveduser = serverToClientData.GetUser();

                // serverSessionManager.FakeClientArrivalProcedure(clientToServerData);
                Assert.Equal(serverToClientData.eventType, "addClient");
                Assert.Equal(receiveduser.username, username);
                Assert.NotNull(receiveduser.userID);
            }
            catch(Exception e)
            {
                Assert.Equal(e.Message, "Value cannot be null.");
            }        
        }

        [Fact]
        public void AddClientProcedureServerSide_MultipleClientsArrives_UsersAddedToServerSession()
        {
            // Clients that arrives are added to the server side
            var users = Utils.GetUsers();
            ServerSessionManager serverSessionManager = SessionManagerFactory.GetServerSessionManager(_fakeCommunicator);

            for (int i = 0; i < users.Count; i++)
            {
                ClientToServerData clientToServerData = new("addClient", users[i].username, users[i].userID);
                var serializedData = _serializer.Serialize(clientToServerData);

                serverSessionManager.OnClientJoined<TcpClient>(null);
                serverSessionManager.FakeClientArrivalProcedure(clientToServerData);
            }

            // The updated session data which includes new users is now sent from server to the client side
            // the deserializedData.sessionData is the updated session received from the server 
           // var deserializedData = _serializer.Deserialize<ServerToClientData>(_fakeCommunicator.transferredData);
            var returnedSessionData = serverSessionManager.GetSessionData();

            // The recieved session must not be null and have the same users that were added
            Assert.NotNull(returnedSessionData);
            Assert.Equal(returnedSessionData.users[0].userID, 1);


            Assert.Equal(users.Count, returnedSessionData.users.Count);

            for (int i = 0; i < users.Count; i++)
            {
                Assert.Equal(users[i].username, returnedSessionData.users[i].username);
                Assert.Equal(users[i].userID, returnedSessionData.users[i].userID);

            }

        }

        [Fact]
        public void UpdatingSessionDataOnArrival_ClientArrives_ClientSessionUpdated()
        {
            // Client session managers for the nth and n+1 th user respectively
            _clientSessionManagerLast = new ClientSessionManager(_fakeCommunicator);
            _clientSessionManagerNew = new ClientSessionManager(_fakeCommunicator);

            var serverSession = Utils.GetSessionData();
            // nth user
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

            // Assertion to check if both nth and the (n+1)th user have the same session
            Assert.NotNull(_clientSessionManagerLast.GetUser());
            Assert.NotNull(_clientSessionManagerNew.GetUser());
            Assert.NotNull(_clientSessionManagerLast.GetSessionData());
            Assert.NotNull(_clientSessionManagerNew.GetSessionData());
            SessionData sessionDataNew = _clientSessionManagerNew.GetSessionData();
            SessionData sessionDataLast = _clientSessionManagerLast.GetSessionData();
            for(int i = 0; i < sessionDataNew.users.Count; i++)
            {
                Assert.Equal(sessionDataNew.users[i].username, sessionDataLast.users[i].username);
                Assert.Equal(sessionDataNew.users[i].userID, sessionDataLast.users[i].userID);
            }
           

            Assert.Equal(serverSession.users.Count,_clientSessionManagerLast.GetSessionData().users.Count );
        }

        [Fact]
        public void GetSummary_RequestSummary_ReturnsSummary()
        {

            ClientSessionManager clientSessionManager = SessionManagerFactory.GetClientSessionManager(_fakeCommunicator);
            ServerSessionManager serverSessionManager = SessionManagerFactory.GetServerSessionManager(_fakeCommunicator);
            FakeClientUX fakeClientUX = new(clientSessionManager);
            fakeClientUX.sessionSummary = null;

            UserData user = new("Jake Vickers", 1);

            clientSessionManager.SetUser(user.username, user.userID);
            clientSessionManager.SetSessionUsers(new List<UserData> { user });
         
            ClientToServerData clientToServerData = new("addClient", user.username);
            var serializedData = _serializer.Serialize(clientToServerData);
          //  Assert.NotNull(serializedData);
            serverSessionManager.OnClientJoined<TcpClient>(null);
            serverSessionManager.OnDataReceived(serializedData);
            Assert.NotNull(_fakeCommunicator.transferredData);
            //_fakeCommunicator.Send("Hello", "Dashboard");
            //Assert.NotNull(_fakeCommunicator.transferredData);
            clientSessionManager.GetSummary();
            serverSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
           // Assert.NotNull(_fakeCommunicator.transferredData);
        
            var clientSummary = clientSessionManager.GetStoredSummary();
            var serverSummary = serverSessionManager.GetStoredSummary();
            
            Assert.NotNull(clientSummary);
            Assert.NotNull(serverSummary);
            Assert.NotNull(fakeClientUX.sessionSummary); 
        }

        [Fact]
        public void GetAnalytics_RequestAnalytics_ReturnSessionAnalyticsAndNotifyUX()
        {
            Setup();
            FakeClientUX fakeClientUX = new(_clientSessionManager);
            FakeTelemetry fakeTelemetry = new(_serverSessionManager);

            UserData user = new("Jake Vickers", 1);

            _clientSessionManager.SetUser(user.username, user.userID);
            _clientSessionManager.SetSessionUsers(new List<UserData> { user });
         
            _clientSessionManager.GetAnalytics();
         
            _serverSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            Assert.NotNull(_clientSessionManager.GetStoredAnalytics());
            Assert.NotNull(fakeClientUX.sessionAnalytics);
        }

        [Fact]
       public void ToggleSessionMode_ReauestedSessionChange_ServerSessionModeUpdatedAndNotifyUX()
        {
            Setup();
            FakeClientUX fakeClientUX = new(_clientSessionManager);
            FakeTelemetry fakeTelemetry = new(_serverSessionManager);
            UserData user = new("Jake Vickers", 1);

            _clientSessionManager.SetUser(user.username, user.userID);
            _clientSessionManager.SetSessionUsers(new List<UserData> { user });
            Assert.Equal(fakeClientUX.sessionMode, "LabMode");
            Assert.Equal(_serverSessionManager.GetSessionData().sessionMode, "LabMode");
            Assert.Equal(_clientSessionManager.GetSessionData().sessionMode, "LabMode");
            _clientSessionManager.ToggleSessionMode();
            _serverSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            Assert.Equal(_serverSessionManager.GetSessionData().sessionMode, "ExamMode");
            Assert.Equal(_clientSessionManager.GetSessionData().sessionMode, _serverSessionManager.GetSessionData().sessionMode);
            Assert.Equal(fakeClientUX.sessionMode, "ExamMode");

        }

        [Fact]
        public void RemoveClient_ClientDeparture_UserRemovedFromServerAndClientSide()
        {
            Setup();
            var users = Utils.GetUsersSet2();

            for (int i = 0; i < users.Count; i++)
            {
                ClientToServerData clientToServerData = new("addClient", users[i].username, users[i].userID);
                var serializedData = _serializer.Serialize(clientToServerData);

                _serverSessionManager.OnClientJoined<TcpClient>(null);
                _serverSessionManager.FakeClientArrivalProcedure(clientToServerData);
            }

            _clientSessionManager.SetUser(users.Last().username, users.Last().userID);
            _clientSessionManager.SetSessionUsers(users);

            // The last user in the list departs
            users.Remove(users.Last());
            _clientSessionManager.RemoveClient();
            _serverSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
        
            Assert.Null(_clientSessionManager.GetUser());
            Assert.Null(_clientSessionManager.GetSessionData());
            Assert.Equal(users.Count, _serverSessionManager.GetSessionData().users.Count); 
        }

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

                _serverSessionManager.OnClientJoined<TcpClient>(null);
                _serverSessionManager.OnDataReceived(serializedData);
            }

            _clientSessionManager.SetUser(users.Last().username, users.Last().userID);
            _clientSessionManager.SetSessionUsers(users);

            _clientSessionManager.EndMeet();
            _serverSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            Assert.Equal(fakeServerUX.meetingEnded, true);
            Assert.Equal(fakeClientUx.meetingEnded, true);
            Assert.Equal(_serverSessionManager.summarySaved, true);
        }


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
                Assert.Equal("Value cannot be null. (Parameter 'Null SerializedObject as Argument')", e.Message);
            }
        }

        [Fact]
        public void OnClientLeft_ClientDisconnects_UserRemoved_ServerSessionChanged()
        {
            Setup();
            // Adding the users to the session and the client side
            var users = Utils.GetUsers();


            for (int i = 0; i < users.Count; i++)
            {
                ClientToServerData clientToServerData = new("addClient", users[i].username, users[i].userID);
                var serializedData = _serializer.Serialize(clientToServerData);

                _serverSessionManager.OnClientJoined<TcpClient>(null);
                _serverSessionManager.FakeClientArrivalProcedure(clientToServerData);
            }


            _clientSessionManager.SetUser(users.Last().username, users.Last().userID);
            _clientSessionManager.SetSessionUsers(users);

            // removing the last user from the meet because of the disconnection
            var disconnectedUser = users.Last();
            users.Remove(users.Last());

            // The client disconnects and the client side is notified
            _serverSessionManager.OnClientLeft(disconnectedUser.userID.ToString());
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            // Check if the session on the server side was updated and the user and session data on the client side are removed.
//            Assert.Equal(users, _serverSessionManager.GetSessionData().users);
            Assert.Null(_clientSessionManager.GetSessionData());
            Assert.Null(_clientSessionManager.GetUser());
        }

        [Fact]
        public void EndMeet_LastUserLeaves_MeetingShouldEnd()
        {
            Setup();
            FakeClientUX fakeClientUx = new(_clientSessionManager);
            FakeServerUX fakeServerUX = new(_serverSessionManager);

            fakeClientUx.meetingEnded = false;
            fakeServerUX.meetingEnded = false;

            List<UserData> users = new();
            users.Add(new UserData("Justin", 1));

            
            for (int i = 0; i < users.Count; i++)
            {
                ClientToServerData _clientToServerData = new("addClient", users[i].username, users[i].userID);
                var serializedData = _serializer.Serialize(_clientToServerData);

                _serverSessionManager.OnClientJoined<TcpClient>(null);
                _serverSessionManager.OnDataReceived(serializedData);
            }



            _clientSessionManager.SetSessionUsers(users);
            _clientSessionManager.SetUser(users[0].username, users[0].userID);

            _clientSessionManager.RemoveClient();
            _serverSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            Assert.Equal(true,fakeServerUX.meetingEnded);
            Assert.Equal(true,fakeClientUx.meetingEnded);
            Assert.Equal(_serverSessionManager.summarySaved, true);
        }

       
    }

}
