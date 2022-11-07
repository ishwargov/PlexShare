/// <author> Anish Bhagavatula and Mohammad Umar Sultan </author>
/// <summary>
/// This file contains all the global constants used by testing files
/// </summary>

using PlexShareNetwork.Queues;
using PlexShareNetwork.Serialization;
using PlexShareNetwork.Sockets;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

        private static readonly Serializer _serializer = new();

        /// <summary>
        /// Returns a randomly generated alphanumeric string
        /// </summary>
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
    }

	/// <summary>
	/// An implementation of the notification handler.
	/// </summary>
	public class TestNotificationHandler : INotificationHandler
	{
        public List<Tuple<string, string?, TcpClient?>> eventAndEventArgument = new();
        public Dictionary<string, Tuple<string?, TcpClient?>> eventToEventArgumentMap = new();
        public int lastEventCount = 0;

        public void OnDataReceived(string data)
		{
            eventAndEventArgument.Add(new("OnDataReceived", data, null));
        }

        public void OnClientJoined(TcpClient socket)
        {
            eventAndEventArgument.Add(new("OnClientJoined", null, socket));
        }

        public void OnClientLeft(string clientId)
		{
            eventAndEventArgument.Add(new("OnClientLeft", clientId, null));
		}

		public void WaitForEvent()
		{
            while (eventAndEventArgument.Count == lastEventCount)
            {
                Thread.Sleep(100);
            }
            lastEventCount = eventAndEventArgument.Count;
        }

        public string GetLastEvent()
        {
            return eventAndEventArgument.Last().Item1;
        }

        public string? GetLastEventData()
        {
            return eventAndEventArgument.Last().Item2;
        }

        public TcpClient? GetLastEventSocket()
        {
            return eventAndEventArgument.Last().Item3;
        }

        public string? GetLastEventClientId()
        {
            return eventAndEventArgument.Last().Item2;
        }
    }
}
