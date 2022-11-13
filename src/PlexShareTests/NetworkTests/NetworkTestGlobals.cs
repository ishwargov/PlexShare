/// <author> Mohammad Umar Sultan </author>
/// <summary>
/// This file contains all the common functions used by network tests
/// </summary>

using PlexShareNetwork.Communication;
using PlexShareNetwork.Queues;
using PlexShareNetwork.Serialization;
using PlexShareNetwork.Sockets;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Transactions;
using System.Windows.Markup;
using Xunit;

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
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static Packet[] GeneratePackets(int dataSize, string? destination, string module, int count)
        {
            Packet[] packets = new Packet[count];
            for (var i = 0; i < count; i++)
            {
                packets[i] = new(RandomString(dataSize), destination, module);
            }
            return packets;
        }

        public static void SendPackets(Packet[] sendPackets, TcpClient socket, int count)
        {
            for (var i = 0; i < count; i++)
            {
                string sendString = SendString.PacketToSendString(sendPackets[i]);
                byte[] bytes = Encoding.ASCII.GetBytes(sendString);
                socket.Client.Send(bytes);
            }
        }

        public static void SendPackets(Packet[] sendPackets, SendingQueue sendingQueue, int count)
        {
            for (var i = 0; i < count; i++)
            {
                sendingQueue.Enqueue(sendPackets[i]);
            }
        }

        public static void PacketsReceiveAssert(Packet[] sendPackets, ReceivingQueue receivingQueue, int count)
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

        public static void AssertPacketEquality(Packet packet1, Packet packet2)
        {
            Assert.Equal(packet1.serializedData, packet2.serializedData);
            Assert.Equal(packet1.destination, packet2.destination);
            Assert.Equal(packet1.moduleOfPacket, packet2.moduleOfPacket);
        }

        public static CommunicatorClient[] GetCommunicatorsClient(int count)
        {
            CommunicatorClient[] communicatorsClient = new CommunicatorClient[count];
            for (int i = 0; i < count; i++)
            {
                communicatorsClient[i] = new CommunicatorClient();
            }
            return communicatorsClient;
        }

        public static TestNotificationHandler[] GetTestNotificationHandlers(int count)
        {
            TestNotificationHandler[] notificaitonHandlers = new TestNotificationHandler[count];
            for (int i = 0; i < count; i++)
            {
                notificaitonHandlers[i] = new TestNotificationHandler();
            }
            return notificaitonHandlers;
        }

        public static void SubscribeOnServerAndClient(ICommunicator communicatorServer, ICommunicator[] communicatorsClient,
                TestNotificationHandler testNotificationHandlerServer, TestNotificationHandler[] testNotificationHandlersClient)
        {
            communicatorServer.Subscribe("Test Module", testNotificationHandlerServer, true);
            for (var i = 0; i < communicatorsClient.Length; i++)
            {
                communicatorsClient[i].Subscribe("Test Module", testNotificationHandlersClient[i], true);
            }
        }

        public static void StartServerAndClients(ICommunicator communicatorServer, ICommunicator[] communicatorsClient)
        {
            string serverIPAndPort = communicatorServer.Start();
            string[] IPAndPort = serverIPAndPort.Split(":");

            // first subscribe the module on the server so that it can be notified with the socket object when client joins
            TestNotificationHandler testNotificationHandlerServer = new();
            communicatorServer.Subscribe("_", testNotificationHandlerServer, true);

            for (var i = 0; i < communicatorsClient.Length; i++)
            {
                string communicatorClientReturn = communicatorsClient[i].Start(IPAndPort[0], IPAndPort[1]);
                Assert.Equal("success", communicatorClientReturn);

                testNotificationHandlerServer.WaitForEvent();
                Assert.Equal("OnClientJoined", testNotificationHandlerServer.GetLastEvent());
                Assert.True(testNotificationHandlerServer.GetLastEventSocket().Connected);
                communicatorServer.AddClient("Client ID" + i, testNotificationHandlerServer.GetLastEventSocket());
            }
        }

        public static void StopServerAndClients(ICommunicator communicatorServer, ICommunicator[] communicatorsClient)
        {
            foreach (ICommunicator communicatorClient in communicatorsClient)
            {
                communicatorClient.Stop();
            }
            communicatorServer.Stop();
        }
    }

	/// <summary>
	/// An implementation of the notification handler.
	/// </summary>
	public class TestNotificationHandler : INotificationHandler
	{
        private readonly List <string> events = new();
        private readonly List<string> datas = new();
        private readonly List<TcpClient> sockets = new();
        private readonly List<string> clientIds = new();

        public List<Tuple<string, string?, TcpClient?>> eventAndEventArgument = new();
        public int lastEventCount = 0;

        public void OnDataReceived(string data)
		{
            events.Add("OnDataReceived");
            datas.Add(data);
        }

        public void OnClientJoined(TcpClient socket)
        {
            events.Add("OnClientJoined");
            sockets.Add(socket);
        }

        public void OnClientLeft(string clientId)
		{
            events.Add("OnClientLeft");
            clientIds.Add(clientId);
		}

		public void WaitForEvent()
		{
            while (events.Count == lastEventCount)
            {
                Thread.Sleep(100);
            }
            lastEventCount = events.Count;
        }

        public string GetLastEvent()
        {
            return events.Last();
        }

        public string? GetLastEventData()
        {
            return datas.Last();
        }

        public TcpClient? GetLastEventSocket()
        {
            return sockets.Last();
        }

        public string? GetLastEventClientId()
        {
            return clientIds.Last();
        }
    }
}
