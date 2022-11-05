using Dashboard.Server.SessionManagement;
using PlexShare.Dashboard;
using PlexShareDashboard.Dashboard.Client.SessionManagement;
using PlexShareNetwork.Serialization;
using PlexShareTests.DashboardTests.SessionManagement.TestModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareTests.DashboardTests.SessionManagement
{
    public class SessionManagementTest
    {
        private ClientSessionManager _clientSessionManager;
        private ClientSessionManager _clientSessionManagerLast;
        private ClientSessionManager _clientSessionManagerNew;

        private FakeCommunicator _fakeCommunicator;
        private FakeContentServer _fakeContentServer;
        private ISerializer _serializer;
        private ServerSessionManager _serverSessionManager;

        public void Setup()
        {
            _fakeCommunicator = new FakeCommunicator();
            _fakeContentServer = new FakeContentServer();
            _serializer = new Serializer();
            _clientSessionManager = SessionManagerFactory.GetClientSessionManager(_fakeCommunicator);
            _serverSessionManager =
                SessionManagerFactory.GetServerSessionManager(_fakeCommunicator);
        }
        [Fact]
        public void GetClientSessionManager_TwoInstancesCreated_MustHaveSameReference()
        {
            IUXClientSessionManager clientSessionManager1 = SessionManagerFactory.GetClientSessionManager();
            IUXClientSessionManager clientSessionManager2 = SessionManagerFactory.GetClientSessionManager();

            //Assert(ReferenceEquals(clientSessionManager1, clientSessionManager2));
            Assert.True(ReferenceEquals(clientSessionManager1, clientSessionManager2));
        }
    }
    
}
