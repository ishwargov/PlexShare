/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains unit tests for the class SocketListener
/// </summary>

using Xunit;

namespace PlexShareNetwork.Communication.Test
{
    public class CommunicatorTests
    {
        private readonly ICommunicator _communicatorClient;
        private readonly ICommunicator _communicatorServer;
        private readonly TestNotificationHandler _testNotificationHandlerClient = new();
        private readonly TestNotificationHandler _testNotificationHandlerServer = new();

        public CommunicatorTests()
        {
            _communicatorClient = CommunicationFactory.GetCommunicator(true);
            _communicatorServer = CommunicationFactory.GetCommunicator(false);
            string serverIPAndPort = _communicatorServer.Start();

            // first subscribe the module on the server so that it can be notified with the socket object when client joins
            _communicatorServer.Subscribe("Test Module", _testNotificationHandlerServer, true);

            string[] IPAndPort = serverIPAndPort.Split(":");
            string communicatorClientReturn = _communicatorClient.Start(IPAndPort[0], IPAndPort[1]);
            Assert.Equal("success", communicatorClientReturn);
            _communicatorClient.Subscribe("Test Module", _testNotificationHandlerClient, true);

            _testNotificationHandlerServer.WaitForEvent();
            Assert.Equal("OnClientJoined", _testNotificationHandlerServer.Event);
            Assert.True(_testNotificationHandlerServer.Socket != null);
            _communicatorServer.AddClient("Client1 ID", _testNotificationHandlerServer.Socket);
        }

        [Fact]
        public void ClientSendToServerTest()
        {
            _testNotificationHandlerServer.Reset();

            _communicatorClient.Send("Hello from client to server", "Test Module", null);

            _testNotificationHandlerServer.WaitForEvent();
            Assert.Equal("OnDataReceived", _testNotificationHandlerServer.Event);
            Assert.Equal("Hello from client to server", _testNotificationHandlerServer.Data);
        }

        [Fact]
        public void ServerSendToClientTest()
        {
            _testNotificationHandlerClient.Reset();
            _communicatorServer.Send("Hello from server to client1", "Test Module", "Client1 ID");

            _testNotificationHandlerClient.WaitForEvent();
            Assert.Equal("OnDataReceived", _testNotificationHandlerClient.Event);
            Assert.Equal("Hello from server to client1", _testNotificationHandlerClient.Data);

            //_testNotificationHandlerClient.Reset();
            //_communicatorServer.Send("Hello from server to all clients", "Test Module", null);

            //_testNotificationHandlerClient.WaitForEvent();
            //Assert.Equal("OnDataReceived", _testNotificationHandlerClient.Event);
            //Assert.Equal("Hello from server to client1", _testNotificationHandlerClient.Data);
        }
    }
}
