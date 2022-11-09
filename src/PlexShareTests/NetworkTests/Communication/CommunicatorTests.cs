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
        private readonly string _module = "Test Module";
        private readonly string _clientID = "Client ID";

        [Fact]
        public void ServerAndSingleClientStartAndStopTest()
        {
            CommunicatorServer communicatorServer = new();
            CommunicatorClient[] communicatorsClient = { new() };
            NetworkTestGlobals.StartServerAndClients(communicatorServer, communicatorsClient);
            NetworkTestGlobals.StopServerAndClients(communicatorServer, communicatorsClient);
        }

        [Fact]
        public void ServerAndMultipleClientsStartAndStopTest()
        {
            CommunicatorServer communicatorServer = new();
            CommunicatorClient[] communicatorsClient = { new(), new(), new(), new(), new() };
            NetworkTestGlobals.StartServerAndClients(communicatorServer, communicatorsClient);
            NetworkTestGlobals.StopServerAndClients(communicatorServer, communicatorsClient);
        }

        [Fact]
        public void SingleClientSendDataToServerTest()
        {
            CommunicatorServer communicatorServer = new();
            CommunicatorClient[] communicatorsClient = { new() };
            NetworkTestGlobals.StartServerAndClients(communicatorServer, communicatorsClient);

            TestNotificationHandler testNotificationHandlerServer = new();
            TestNotificationHandler testNotificationHandlerClient = new();
            communicatorServer.Subscribe(_module, testNotificationHandlerServer, true);
            communicatorsClient[0].Subscribe(_module, testNotificationHandlerClient, true);
            communicatorsClient[0].Send("Hello from client to server", _module, null);

            testNotificationHandlerServer.WaitForEvent();
            Assert.Equal("OnDataReceived", testNotificationHandlerServer.GetLastEvent());
            Assert.Equal("Hello from client to server", testNotificationHandlerServer.GetLastEventData());
            NetworkTestGlobals.StopServerAndClients(communicatorServer, communicatorsClient);
        }

        [Fact]
        public void MultipleClientsSendDataToServerTest()
        {
            CommunicatorServer communicatorServer = new();
            CommunicatorClient[] communicatorsClient = { new(), new(), new(), new(), new() };
            NetworkTestGlobals.StartServerAndClients(communicatorServer, communicatorsClient);

            TestNotificationHandler testNotificationHandlerServer = new();
            communicatorServer.Subscribe(_module, testNotificationHandlerServer, true);
            for (var i = 0; i < communicatorsClient.Length; i++)
            {
                TestNotificationHandler testNotificationHandlerClient = new();
                communicatorsClient[i].Subscribe(_module, testNotificationHandlerClient, true);
                communicatorsClient[i].Send("Hello to server from client" + i, _module, null);

                testNotificationHandlerServer.WaitForEvent();
                Assert.Equal("OnDataReceived", testNotificationHandlerServer.GetLastEvent());
                Assert.Equal("Hello to server from client" + i, testNotificationHandlerServer.GetLastEventData());
            }
            NetworkTestGlobals.StopServerAndClients(communicatorServer, communicatorsClient);
        }

        [Fact]
        public void ServerUnicastDataToSingleClientTest()
        {
            CommunicatorServer communicatorServer = new();
            CommunicatorClient[] communicatorsClient = { new() };
            NetworkTestGlobals.StartServerAndClients(communicatorServer, communicatorsClient);

            TestNotificationHandler testNotificationHandlerServer = new();
            TestNotificationHandler testNotificationHandlerClient = new();
            communicatorServer.Subscribe(_module, testNotificationHandlerServer, true);
            communicatorsClient[0].Subscribe(_module, testNotificationHandlerClient, true);
            communicatorServer.Send("Hello from server to client", _module, _clientID + 0);

            testNotificationHandlerClient.WaitForEvent();
            Assert.Equal("OnDataReceived", testNotificationHandlerClient.GetLastEvent());
            Assert.Equal("Hello from server to client", testNotificationHandlerClient.GetLastEventData());
            NetworkTestGlobals.StopServerAndClients(communicatorServer, communicatorsClient);
        }

        [Fact]
        public void ServerUnicastDataToMultipleClientsTest()
        {
            CommunicatorServer communicatorServer = new();
            CommunicatorClient[] communicatorsClient = { new(), new(), new(), new(), new() };
            NetworkTestGlobals.StartServerAndClients(communicatorServer, communicatorsClient);

            TestNotificationHandler testNotificationHandlerServer = new();
            communicatorServer.Subscribe(_module, testNotificationHandlerServer, true);
            TestNotificationHandler[] testNotificationHandlersClient = { new(), new(), new(), new(), new() };

            for (var i = 0; i < communicatorsClient.Length; i++)
            {
                communicatorsClient[i].Subscribe(_module, testNotificationHandlersClient[i], true);
                communicatorServer.Send("Hello from server to client" + i, _module, _clientID + i);
                testNotificationHandlersClient[i].WaitForEvent();
                Assert.Equal("OnDataReceived", testNotificationHandlersClient[i].GetLastEvent());
                Assert.Equal("Hello from server to client" + i, testNotificationHandlersClient[i].GetLastEventData());
            }
            NetworkTestGlobals.StopServerAndClients(communicatorServer, communicatorsClient);
        }

        [Fact]
        public void ServerBroadcastDataToClientsTest()
        {
            CommunicatorServer communicatorServer = new();
            CommunicatorClient[] communicatorsClient = { new(), new(), new(), new(), new() };
            NetworkTestGlobals.StartServerAndClients(communicatorServer, communicatorsClient);

            TestNotificationHandler testNotificationHandlerServer = new();
            communicatorServer.Subscribe(_module, testNotificationHandlerServer, true);
            TestNotificationHandler[] testNotificationHandlersClient = { new(), new(), new(), new(), new() };
            for (var i = 0; i < communicatorsClient.Length; i++)
            {
                communicatorsClient[i].Subscribe(_module, testNotificationHandlersClient[i], true);
            }

            communicatorServer.Send("Hello from server to all clients", _module, null);

            for (var i = 0; i < communicatorsClient.Length; i++)
            {
                testNotificationHandlersClient[i].WaitForEvent();
                Assert.Equal("OnDataReceived", testNotificationHandlersClient[i].GetLastEvent());
                Assert.Equal("Hello from server to all clients", testNotificationHandlersClient[i].GetLastEventData());
            }
            NetworkTestGlobals.StopServerAndClients(communicatorServer, communicatorsClient);
        }
    }
}
