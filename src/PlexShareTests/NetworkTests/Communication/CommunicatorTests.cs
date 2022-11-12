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

        private static void ServerAndClientsStartAndStopTest(int numClients)
        {
            CommunicatorServer communicatorServer = new();
            CommunicatorClient[] communicatorsClient = NetworkTestGlobals.GetCommunicatorsClient(numClients);
            NetworkTestGlobals.StartServerAndClients(communicatorServer, communicatorsClient);
            NetworkTestGlobals.StopServerAndClients(communicatorServer, communicatorsClient);
        }

        [Fact]
        public void ServerAndSingleClientStartAndStopTest()
        {
            ServerAndClientsStartAndStopTest(1);
        }

        [Fact]
        public void ServerAndMultipleClientsStartAndStopTest()
        {
            ServerAndClientsStartAndStopTest(10);
        }

        [Fact]
        public void ServerAndClientsSubscribeTest()
        {
            CommunicatorServer communicatorServer = new();
            CommunicatorClient[] communicatorsClient = NetworkTestGlobals.GetCommunicatorsClient(10);
            NetworkTestGlobals.StartServerAndClients(communicatorServer, communicatorsClient);
            TestNotificationHandler testNotificationHandlerServer = new();
            TestNotificationHandler[] testNotificationHandlersClient = NetworkTestGlobals.GetTestNotificationHandlers(10);
            NetworkTestGlobals.SubscribeOnServerAndClient(communicatorServer, communicatorsClient,
                testNotificationHandlerServer, testNotificationHandlersClient);
            NetworkTestGlobals.StopServerAndClients(communicatorServer, communicatorsClient);
        }

        private void ClientsSendDataToServerTest(int numClients)
        {
            CommunicatorServer communicatorServer = new();
            CommunicatorClient[] communicatorsClient = NetworkTestGlobals.GetCommunicatorsClient(numClients);
            NetworkTestGlobals.StartServerAndClients(communicatorServer, communicatorsClient);
            TestNotificationHandler testNotificationHandlerServer = new();
            TestNotificationHandler[] testNotificationHandlersClient = NetworkTestGlobals.GetTestNotificationHandlers(10);
            NetworkTestGlobals.SubscribeOnServerAndClient(communicatorServer, communicatorsClient,
                testNotificationHandlerServer, testNotificationHandlersClient);
            for (var i = 0; i < communicatorsClient.Length; i++)
            {
                communicatorsClient[i].Send("Hello to server from client" + i, _module, null);
                testNotificationHandlerServer.WaitForEvent();
                Assert.Equal("OnDataReceived", testNotificationHandlerServer.GetLastEvent());
                Assert.Equal("Hello to server from client" + i, testNotificationHandlerServer.GetLastEventData());
            }
            NetworkTestGlobals.StopServerAndClients(communicatorServer, communicatorsClient);
        }

        [Fact]
        public void SingleClientSendDataToServerTest()
        {
            ClientsSendDataToServerTest(1);
        }

        [Fact]
        public void MultipleClientsSendDataToServerTest()
        {
            ClientsSendDataToServerTest(10);
        }

        private void ServerSendDataToClientsTest(int numClients, bool doBroadcast)
        {
            CommunicatorServer communicatorServer = new();
            CommunicatorClient[] communicatorsClient = NetworkTestGlobals.GetCommunicatorsClient(numClients);
            NetworkTestGlobals.StartServerAndClients(communicatorServer, communicatorsClient);
            TestNotificationHandler testNotificationHandlerServer = new();
            TestNotificationHandler[] testNotificationHandlersClient = NetworkTestGlobals.GetTestNotificationHandlers(numClients);
            NetworkTestGlobals.SubscribeOnServerAndClient(communicatorServer, communicatorsClient,
                testNotificationHandlerServer, testNotificationHandlersClient);

            if (doBroadcast)
            {
                communicatorServer.Send("Hello from server to all clients", _module, null);
                for (var i = 0; i < communicatorsClient.Length; i++)
                {
                    testNotificationHandlersClient[i].WaitForEvent();
                    Assert.Equal("OnDataReceived", testNotificationHandlersClient[i].GetLastEvent());
                    Assert.Equal("Hello from server to all clients", testNotificationHandlersClient[i].GetLastEventData());
                }
            }
            else
            {
                for (var i = 0; i < communicatorsClient.Length; i++)
                {
                    communicatorServer.Send("Hello from server to client" + i, _module, _clientID + i);
                    testNotificationHandlersClient[i].WaitForEvent();
                    Assert.Equal("OnDataReceived", testNotificationHandlersClient[i].GetLastEvent());
                    Assert.Equal("Hello from server to client" + i, testNotificationHandlersClient[i].GetLastEventData());
                }
            }
            NetworkTestGlobals.StopServerAndClients(communicatorServer, communicatorsClient);
        }

        [Fact]
        public void ServerUnicastDataToSingleClientTest()
        {
            ServerSendDataToClientsTest(1, false);
        }

        [Fact]
        public void ServerUnicastDataToMultipleClientsTest()
        {
            ServerSendDataToClientsTest(10, false);
        }

        [Fact]
        public void ServerBroadcastDataToClientsTest()
        {
            ServerSendDataToClientsTest(10, true);
        }
    }
}
