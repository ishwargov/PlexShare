/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains unit tests for the class SocketListener
/// </summary>

using System.Net.Sockets;
using Xunit;

namespace PlexShareNetwork.Communication.Test
{
    public class CommunicatorTests
    {
        private CommunicatorClient _communicatorClient = new();
        private CommunicatorServer _communicatorServer = new();
        private readonly TestNotificationHandler _testNotificationHandlerClient = new();
        private readonly TestNotificationHandler _testNotificationHandlerServer = new();
        private readonly string _module = "Test Module";
        private readonly string _clientID = "Test Client ID";

        private void ServerAndClientStartTest()
        {
            string serverIPAndPort = _communicatorServer.Start();
            string[] IPAndPort = serverIPAndPort.Split(":");

            // first subscribe the module on the server so that it can be notified with the socket object when client joins
            _communicatorServer.Subscribe(_module, _testNotificationHandlerServer, true);

            string communicatorClientReturn = _communicatorClient.Start(IPAndPort[0], IPAndPort[1]);
            Assert.Equal("success", communicatorClientReturn);
            _communicatorClient.Subscribe(_module, _testNotificationHandlerClient, true);

            _testNotificationHandlerServer.WaitForEvent();
            Assert.Equal("OnClientJoined", _testNotificationHandlerServer.GetLastEvent());
            Assert.True(_testNotificationHandlerServer.GetLastEventSocket().Connected);
            _communicatorServer.AddClient(_clientID, _testNotificationHandlerServer.GetLastEventSocket());
        }

        private void ServerAndClientStopTest()
        {
            _communicatorClient.Stop();
            _communicatorServer.Stop();
        }

        private void ClientSendToServerTest()
        {
            _communicatorClient.Send("Hello from client to server", "Test Module", null);

            _testNotificationHandlerServer.WaitForEvent();
            Assert.Equal("OnDataReceived", _testNotificationHandlerServer.GetLastEvent());
            Assert.Equal("Hello from client to server", _testNotificationHandlerServer.GetLastEventData());
        }

        private void ServerUnicastToClientTest()
        {
            _communicatorServer.Send("Hello from server to client", "Test Module", _clientID);

            _testNotificationHandlerClient.WaitForEvent();
            Assert.Equal("OnDataReceived", _testNotificationHandlerClient.GetLastEvent());
            Assert.Equal("Hello from server to client", _testNotificationHandlerClient.GetLastEventData());
        }

        private void ServerBroadcastToClientTest()
        {
            _communicatorServer.Send("Hello from server to all clients", "Test Module", null);

            _testNotificationHandlerClient.WaitForEvent();
            Assert.Equal("OnDataReceived", _testNotificationHandlerClient.GetLastEvent());
            Assert.Equal("Hello from server to all clients", _testNotificationHandlerClient.GetLastEventData());
        }

        [Fact]
        public void RunAllTests()
        {
            ServerAndClientStartTest();
            ClientSendToServerTest();
            ServerUnicastToClientTest();
            ServerBroadcastToClientTest();
            ServerAndClientStopTest();
        }
    }
}
