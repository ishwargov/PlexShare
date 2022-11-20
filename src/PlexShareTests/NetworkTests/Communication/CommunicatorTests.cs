/// <author> Mohammad Umar Sultan </author>
/// <created> 16/10/2022 </created>
/// <summary>
/// This file contains tests for the server and client Communicator
/// </summary>

using PlexShareNetwork.Communication;
using Xunit;

namespace PlexShareTests.NetworkTests.Communication
{
    public class CommunicatorTests
    {
        // variables to be used in tests
        private static readonly string _module = "Test Module";
        private static readonly bool _modulePriority = true;
        private static readonly string _clientId = "Client Id";
        private static readonly int _multipleClientsCount = 10;

        /// <summary>
        /// Creates server and "numClients" number of clients and
        /// starts and stop them.
        /// </summary>
        /// <param name="numClients"> Number of clients to test.
        /// </param>
        /// <returns> void </returns>
        private void ServerAndClientsStartAndStopTest(
            int numClients)
        {
            // create server and clients and then staart and stop them
            CommunicatorServer communicatorServer = new();
            CommunicatorClient[] communicatorsClient =
                NetworkTestGlobals.GetCommunicatorsClient(numClients);
            NetworkTestGlobals.StartServerAndClients(
                communicatorServer, communicatorsClient, _clientId);
            NetworkTestGlobals.StopServerAndClients(
                communicatorServer, communicatorsClient);

            // now test error catch on start client and server
            CommunicatorClient communicatorClient = new();
            communicatorClient.Stop();
            communicatorServer.Stop();
            communicatorClient.Start("abc", "0");
            communicatorServer.Start();
        }

        /// <summary>
		/// Tests start and stop of server and single client.
		/// </summary>
        /// <returns> void </returns>
        [Fact]
        public void ServerAndSingleClientStartAndStopTest()
        {
            ServerAndClientsStartAndStopTest(1);
        }

        /// <summary>
        /// Tests start and stop of server and multiple client.
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void ServerAndMultipleClientsStartAndStopTest()
        {
            ServerAndClientsStartAndStopTest(_multipleClientsCount);
        }

        /// <summary>
        /// Tests module subscription on server and client.
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void ServerAndClientsSubscribeTest()
        {
            // start server and clients
            CommunicatorServer communicatorServer = new();
            CommunicatorClient[] communicatorsClient = 
                NetworkTestGlobals.GetCommunicatorsClient(
                    _multipleClientsCount);
            NetworkTestGlobals.StartServerAndClients(
                communicatorServer, communicatorsClient, _clientId);

            // create notification handlers
            TestNotificationHandler notificationHandlerServer = new();
            TestNotificationHandler[] notificationHandlersClient = 
                NetworkTestGlobals.GetTestNotificationHandlers(
                    _multipleClientsCount);
            
            // subscribe on client and server
            NetworkTestGlobals.SubscribeOnServerAndClient(
                communicatorServer, communicatorsClient,
                notificationHandlerServer, notificationHandlersClient,
                _module, _modulePriority);

            // subscribe error catch test
            NetworkTestGlobals.SubscribeOnServerAndClient(
                communicatorServer, communicatorsClient,
                notificationHandlerServer, notificationHandlersClient,
                _module, _modulePriority);

            // stop client and server
            NetworkTestGlobals.StopServerAndClients(
                communicatorServer, communicatorsClient);
        }

        [Fact]
        public void AddAndRemoveClientOnServerTest()
        {
            // start server and clients
            CommunicatorServer communicatorServer = new();
            CommunicatorClient[] communicatorsClient =
                NetworkTestGlobals.GetCommunicatorsClient(1);
            NetworkTestGlobals.StartServerAndClients(
                communicatorServer, communicatorsClient, _clientId);
            communicatorServer.RemoveClient(_clientId + 0);
        }

        /// <summary>
        /// Tests sending data from "numClients" number of clients
        /// to the server.
        /// </summary>
        /// <param name="numClients"> Number of clients to test.
        /// </param>
        /// <returns> void </returns>
        private void ClientsSendDataToServerTest(int numClients)
        {
            // start server and clients
            CommunicatorServer communicatorServer = new();
            CommunicatorClient[] communicatorsClient = 
                NetworkTestGlobals.GetCommunicatorsClient(numClients);
            NetworkTestGlobals.StartServerAndClients(
                communicatorServer, communicatorsClient, _clientId);

            // create notification handlers
            TestNotificationHandler notificationHandlerServer = new();
            TestNotificationHandler[] notificationHandlersClient = 
                NetworkTestGlobals.GetTestNotificationHandlers(
                    _multipleClientsCount);

            // subscribe on client and server
            NetworkTestGlobals.SubscribeOnServerAndClient(
                communicatorServer, communicatorsClient,
                notificationHandlerServer, notificationHandlersClient,
                _module, _modulePriority);

            // iterate through all clients to send data to server
            for (var i = 0; i < communicatorsClient.Length; i++)
            {
                // send the data from client to server
                string data = NetworkTestGlobals.RandomString(10);
                communicatorsClient[i].Send(data, _module, null);

                // assert data is received on the server
                notificationHandlerServer.WaitForEvent();
                Assert.Equal("OnDataReceived", 
                    notificationHandlerServer.GetLastEvent());
                Assert.Equal(data,
                    notificationHandlerServer.GetLastEventData());
            }

            // stop client and server
            NetworkTestGlobals.StopServerAndClients(
                communicatorServer, communicatorsClient);
        }

        /// <summary>
        /// Tests sending data from single client to the server.
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void SingleClientSendDataToServerTest()
        {
            ClientsSendDataToServerTest(1);
        }

        /// <summary>
        /// Tests sending data from multiple clients to the server.
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void MultipleClientsSendDataToServerTest()
        {
            ClientsSendDataToServerTest(_multipleClientsCount);
        }

        /// <summary>
        /// Tests sending data from server to "numClients" number 
        /// of clients. Tests both unicast and broadcast.
        /// </summary>
        /// <param name="numClients"> Number of clients to test.
        /// </param>
        /// <param name="doBroadcast">
        /// Boolean to tell whether to do broadcast or unicast.
        /// </param>
        /// <returns> void </returns>
        private void ServerSendDataToClientsTest(int numClients,
            bool doBroadcast)
        {
            // start server and clients
            CommunicatorServer communicatorServer = new();
            CommunicatorClient[] communicatorsClient = 
                NetworkTestGlobals.GetCommunicatorsClient(numClients);
            NetworkTestGlobals.StartServerAndClients(
                communicatorServer, communicatorsClient, _clientId);

            // create notification handlers
            TestNotificationHandler notificationHandlerServer = new();
            TestNotificationHandler[] notificationHandlersClient = 
                NetworkTestGlobals.GetTestNotificationHandlers(
                    numClients);

            // subscribe on client and server
            NetworkTestGlobals.SubscribeOnServerAndClient(
                communicatorServer, communicatorsClient,
                notificationHandlerServer,
                notificationHandlersClient,
                _module, _modulePriority);

            // if doBroadcast the do broadcast
            if (doBroadcast)
            {
                // broadcast data from server to all clients
                string data = NetworkTestGlobals.RandomString(10);
                communicatorServer.Send(data, _module, null);

                // assert data is received by all clients
                for (var i = 0; i < communicatorsClient.Length; i++)
                {
                    notificationHandlersClient[i].WaitForEvent();
                    Assert.Equal("OnDataReceived",
                        notificationHandlersClient[i].GetLastEvent());
                    Assert.Equal(data, notificationHandlersClient[i].
                        GetLastEventData());
                }
            }
            else
            {
                // iterate through all clients and unicast data from
                // server to each client
                for (var i = 0; i < communicatorsClient.Length; i++)
                {
                    // send data from server to the client
                    string data = NetworkTestGlobals.RandomString(10);
                    communicatorServer.Send(
                        data, _module, _clientId + i);
                    
                    // assert data is received by the client
                    notificationHandlersClient[i].WaitForEvent();
                    Assert.Equal("OnDataReceived", 
                        notificationHandlersClient[i].GetLastEvent());
                    Assert.Equal(data, notificationHandlersClient[i].
                        GetLastEventData());
                }
            }

            // stop client and server
            NetworkTestGlobals.StopServerAndClients(
                communicatorServer, communicatorsClient);
        }

        /// <summary>
        /// Tests unicast data from server to single client.
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void ServerUnicastDataToSingleClientTest()
        {
            ServerSendDataToClientsTest(1, false);
        }

        /// <summary>
        /// Tests unicast data from server to multiple clients.
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void ServerUnicastDataToMultipleClientsTest()
        {
            ServerSendDataToClientsTest(_multipleClientsCount, false);
        }

        /// <summary>
        /// Tests broadcast data from server to all clients.
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void ServerBroadcastDataToClientsTest()
        {
            ServerSendDataToClientsTest(_multipleClientsCount, true);
        }

        [Fact]
        public void ClientDisconnectTest()
        {
            // start server and clients
            CommunicatorServer communicatorServer = new();
            CommunicatorClient[] communicatorsClient = 
                NetworkTestGlobals.GetCommunicatorsClient(
                    _multipleClientsCount);
            NetworkTestGlobals.StartServerAndClients(
                communicatorServer, communicatorsClient, _clientId);

            // create notification handlers
            TestNotificationHandler notificationHandlerServer = new();
            TestNotificationHandler[] notificationHandlersClient =
                NetworkTestGlobals.GetTestNotificationHandlers(
                    _multipleClientsCount);

            // subscribe on client and server
            NetworkTestGlobals.SubscribeOnServerAndClient(
                communicatorServer, communicatorsClient,
                notificationHandlerServer,
                notificationHandlersClient,
                _module, _modulePriority);

            // disconnect client 0
            communicatorsClient[0].Stop();
            
            // broadcast data from server to all clients
            string data = NetworkTestGlobals.RandomString(10);
            communicatorServer.Send(data, _module, null);

            // assert module is notified on server that client
            // has left
            notificationHandlerServer.WaitForEvent();
            Assert.Equal("OnClientLeft",
                    notificationHandlerServer.GetLastEvent());
            Assert.Equal(_clientId + 0,
                notificationHandlerServer.GetLastEventClientId());

            // assert data is received by all other clients
            for (var i = 1; i < communicatorsClient.Length; i++)
            {
                notificationHandlersClient[i].WaitForEvent();
                Assert.Equal("OnDataReceived",
                    notificationHandlersClient[i].GetLastEvent());
                Assert.Equal(data, notificationHandlersClient[i].
                    GetLastEventData());
            }

            // stop server and client
            NetworkTestGlobals.StopServerAndClients(
                communicatorServer, communicatorsClient);
        }

        /// <summary>
        /// Tests all error catch cases in communicator
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void CommunicatorErrorCatchTest()
        {
            CommunicatorServer communicatorServer = new();
            communicatorServer.Start();
            communicatorServer.AddClient(null, null);
            communicatorServer.RemoveClient(null);
            communicatorServer.Send("Data", "Module", "Client Id1");
        }
    }
}
