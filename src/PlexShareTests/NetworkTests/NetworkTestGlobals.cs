/// <author> Mohammad Umar Sultan </author>
/// <created> 16/10/2022 </created>
/// <summary>
/// This file contains all the common functions used by network tests
/// </summary>

using PlexShareNetwork.Communication;
using PlexShareNetwork.Queues;
using PlexShareNetwork.Sockets;
using System.Net.Sockets;
using System.Text;

namespace PlexShareNetwork
{
    public static class NetworkTestGlobals
    {
        public const string dashboardName = "Dashboard";

        // Priorities of each module (true is for high priority)
        public const bool dashboardPriority = true;

        // Used to generate random strings
        private static readonly Random random = new();

        /// <summary>
        /// Returns a randomly generated alphanumeric string
        /// </summary>
        /// <param name="length"> Length of the string. </param>
        /// <returns> Randomly generated string. </returns>
        public static string RandomString(int length)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).
            Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Generates "count" number of packets and returns the packets
        /// </summary>
        /// <param name="dataSize"> Size of data in packets. </param>
        /// <param name="destination"> Destination of packets. </param>
        /// <param name="module"> Module of packet. </param>
        /// <param name="count"> Count of packets to generate. </param>
        /// <returns> Randomly generated string. </returns>
        public static Packet[] GeneratePackets(int dataSize, 
            string? destination, string module, int count)
        {
            Packet[] packets = new Packet[count];
            for (var i = 0; i < count; i++)
            {
                string data = RandomString(dataSize);
                packets[i] = new Packet(data, destination, module);
            }
            return packets;
        }

        /// <summary>
        /// Sends the given packets through the given socket.
        /// </summary>
        /// <param name="packets"> Array of packets. </param>
        /// <param name="socket"> Socket to send packets. </param>
        /// <param name="count"> Count of packets. </param>
        /// <returns> void </returns>
        public static void SendPackets(Packet[] packets,
            TcpClient socket)
        {
            for (var i = 0; i < packets.Length; i++)
            {
                string packetString = 
                    PacketString.PacketToPacketString(packets[i]);
                byte[] bytes = Encoding.ASCII.GetBytes(packetString);
                socket.Client.Send(bytes);
            }
        }

        /// <summary>
        /// Enqueues the given packets into the given sending queue.
        /// </summary>
        /// <param name="packets"> Array of packets. </param>
        /// <param name="sendingQueue"> The sending queue. </param>
        /// <param name="count"> Count of packets. </param>
        /// <returns> void </returns>
        public static void EnqueuePackets(Packet[] packets, 
            SendingQueue queue)
        {
            for (var i = 0; i < packets.Length; i++)
            {
                queue.Enqueue(packets[i]);
            }
        }

        /// <summary>
        /// Asserts that the given send packets are received in
        /// the receiving queue.
        /// </summary>
        /// <param name="sendPackets"> Array of packets. </param>
        /// <param name="receivingQueue"> Receiving queue. </param>
        /// <param name="count"> Count of packets. </param>
        /// <returns> void </returns>
        public static void PacketsReceiveAssert(Packet[] sendPackets,
            ReceivingQueue receivingQueue, int count)
        {
            while (receivingQueue.Size() < count)
            {
                Thread.Sleep(100);
            }
            Assert.True(receivingQueue.Size() == count);

            for (var i = 0; i < count; i++)
            {
                Packet receivedPacket = receivingQueue.Dequeue();
                AssertPacketEquality(sendPackets[i], receivedPacket);
            }
        }

        /// <summary>
        /// Asserts the 2 given packets are equal.
        /// </summary>
        /// <param name="packet1"> The first packet. </param>
        /// <param name="packet2"> The second packet. </param>
        /// <returns> void </returns>
        public static void AssertPacketEquality(
            Packet packet1, Packet packet2)
        {
            Assert.Equal(
                packet1.serializedData, packet2.serializedData);
            Assert.Equal(
                packet1.destination, packet2.destination);
            Assert.Equal(
                packet1.moduleOfPacket, packet2.moduleOfPacket);
        }

        /// <summary>
        /// Creates and returns an array of CommunicatorClient
        /// </summary>
        /// <param name="count">
        /// Number of CommunicatorClient to be created.
        /// </param>
        /// <returns>
        /// The array of created CommunicatorClient.
        /// </returns>
        public static CommunicatorClient[] GetCommunicatorsClient(
            int count)
        {
            CommunicatorClient[] communicatorsClient = 
                new CommunicatorClient[count];
            for (int i = 0; i < count; i++)
            {
                communicatorsClient[i] = new CommunicatorClient();
            }
            return communicatorsClient;
        }

        /// <summary>
        /// Creates and returns an array of TestNotificationHandler
        /// </summary>
        /// <param name="count">
        /// Number of TestNotificationHandler to be created.
        /// </param>
        /// <returns>
        /// The array of created TestNotificationHandler.
        /// </returns>
        public static TestNotificationHandler[] 
            GetTestNotificationHandlers(int count)
        {
            TestNotificationHandler[] notificaitonHandlers = 
                new TestNotificationHandler[count];
            for (int i = 0; i < count; i++)
            {
                notificaitonHandlers[i] = new();
            }
            return notificaitonHandlers;
        }

        /// <summary>
        /// Subscribes the given module using the given server and
        /// client notification handlers to the given server array
        /// of clients.
        /// </summary>
        /// <param name="communicatorServer"> The server communicator.
        /// </param>
        /// <param name="communicatorsClient">
        /// The array of client communicators.
        /// </param>
        /// <param name="notificationHandlerServer">
        /// The notification handler to subscribe on server.
        /// </param>
        /// <param name="notificationHandlersClient">
        /// The array of notification handlers to subscribe on clients.
        /// </param>
        /// <param name="module"> The module to subscribe. </param>
        /// <param name="priority"> Priority of the module. </param>
        /// <returns> void </returns>
        public static void SubscribeOnServerAndClient(ICommunicator 
            communicatorServer, ICommunicator[] communicatorsClient,
            TestNotificationHandler notificationHandlerServer,
            TestNotificationHandler[] notificationHandlersClient,
            string module, bool priority)
        {
            communicatorServer.Subscribe(
                module, notificationHandlerServer, priority);
            for (var i = 0; i < communicatorsClient.Length; i++)
            {
                communicatorsClient[i].Subscribe(
                    module, notificationHandlersClient[i], priority);
            }
        }

        /// <summary>
        /// Starts the given server and the given array of clients
        /// </summary>
        /// <param name="communicatorServer"> The server communicator.
        /// </param>
        /// <param name="communicatorsClient">
        /// The array of client communicators.
        /// </param>
        /// <returns> void </returns>
        public static void StartServerAndClients(ICommunicator 
            communicatorServer, ICommunicator[] communicatorsClient,
            string clientId)
        {
            string serverIPAndPort = communicatorServer.Start();
            string[] IPAndPort = serverIPAndPort.Split(":");
            string ip = IPAndPort[0];
            string port = IPAndPort[1];

            // first subscribe the module on the server so that it can
            // be notified with the socket object when client joins
            TestNotificationHandler notificationHandlerServer = new();
            communicatorServer.Subscribe(
                "_", notificationHandlerServer, true);

            for (var i = 0; i < communicatorsClient.Length; i++)
            {
                string communicatorClientReturn = 
                    communicatorsClient[i].Start(ip, port);
                Assert.Equal("success", communicatorClientReturn);

                // assert that the module is notified on the server
                // that a new client has joined
                notificationHandlerServer.WaitForEvent();
                Assert.Equal("OnClientJoined", 
                    notificationHandlerServer.GetLastEvent());

                // assert that the socket is connected to the client
                TcpClient socket =
                    notificationHandlerServer.GetLastEventSocket();
                Assert.True(socket.Connected);
                
                // add this client on the server
                communicatorServer.AddClient(clientId + i, socket);
            }
        }

        /// <summary>
        /// Stops the given server and the given array of clients
        /// </summary>
        /// <param name="communicatorServer"> The server communicator.
        /// </param>
        /// <param name="communicatorsClient">
        /// The array of client communicators.
        /// </param>
        /// <returns> void </returns>
        public static void StopServerAndClients(ICommunicator
            communicatorServer, ICommunicator[] communicatorsClient)
        {
            foreach (ICommunicator communicatorClient in
                communicatorsClient)
            {
                communicatorClient.Stop();
            }
            communicatorServer.Stop();
        }
    }

	/// <summary>
	/// An test implementation of the INotificationHandler.
	/// </summary>
	public class TestNotificationHandler : INotificationHandler
	{
        // lists to store the events and the respective data,
        // socket, clientId arguments
        private readonly List <string> events = new();
        private readonly List<string> datas = new();
        private readonly List<TcpClient> sockets = new();
        private readonly List<string> clientIds = new();

        // to remember the count of the last even, so that we can
        // know when a new even occurs
        public int lastEventCount = 0;

        /// <summary>
        /// Called by the Communicator to notify subscribed module
        /// when data is received
        /// </summary>
        public void OnDataReceived(string data)
		{
            events.Add("OnDataReceived");
            datas.Add(data);
        }

        /// <summary>
        /// Called by the Communicator to notify subscribed module
        /// when a new client joins
        /// </summary>
        public void OnClientJoined(TcpClient socket)
        {
            events.Add("OnClientJoined");
            sockets.Add(socket);
        }

        /// <summary>
        /// Called by the Communicator to notify subscribed module
        /// a client leaves
        /// </summary>
        public void OnClientLeft(string clientId)
		{
            events.Add("OnClientLeft");
            clientIds.Add(clientId);
		}

        /// <summary>
        /// Waits for a notification event to occur
        /// </summary>
        public void WaitForEvent()
		{
            while (events.Count == lastEventCount)
            {
                Thread.Sleep(100);
            }
            lastEventCount = events.Count;
        }

        /// <summary>
        /// Returns the last event, to be called after an event
        /// has occured
        /// </summary>
        public string GetLastEvent()
        {
            return events.Last();
        }

        /// <summary>
        /// Returns the data of the last event, to be called
        /// after a "OnDataReceived" event has occured
        /// </summary>
        public string GetLastEventData()
        {
            return datas.Last();
        }

        /// <summary>
        /// Returns the socket of the last event, to be called
        /// after a "OnClientJoined" event has occured
        /// </summary>
        public TcpClient GetLastEventSocket()
        {
            return sockets.Last();
        }

        /// <summary>
        /// Returns the clientId of the last event, to be called
        /// after a "OnClientLeft" event has occured
        /// </summary>
        public string GetLastEventClientId()
        {
            return clientIds.Last();
        }
    }
}
