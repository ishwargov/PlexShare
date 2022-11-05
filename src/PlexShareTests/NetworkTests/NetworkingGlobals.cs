/// <author> Anish Bhagavatula and Mohammad Umar Sultan </author>
/// <summary>
/// This file contains all the global constants used by testing files
/// </summary>

using PlexShareNetwork.Communication;
using PlexShareNetwork.Queues;
using System;
using System.Dynamic;
using System.Linq;
using System.Threading;

namespace PlexShareNetwork
{
	public static class NetworkingGlobals
	{
		public const string dashboardName = "Dashboard", whiteboardName = "Whiteboard", screenshareName = "Screenshare";

		// Priorities of each module (true is for high priority)
		public const bool dashboardPriority = true, whiteboardPriority = false, screensharePriority = true;

		public static ICommunicator NewClientCommunicator => CommunicationFactory.GetCommunicator(true, true);

		public static ICommunicator NewServerCommunicator => CommunicationFactory.GetCommunicator(false, true);

		// Used to generate random strings
		private static readonly Random random = new();

		/// <summary>
		/// Returns a randomly generated alphanumeric string
		/// </summary>
		public static string RandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
			.Select(s => s[random.Next(s.Length)]).ToArray());
		}

        public static void AssertPacketEquality(Packet packet1, Packet packet2)
        {
            Assert.Equal(packet1.serializedData, packet2.serializedData);
            Assert.Equal(packet1.destination, packet2.destination);
            Assert.Equal(packet1.moduleOfPacket, packet2.moduleOfPacket);
        }
	}

	/// <summary>
	/// The 3 notification events which could occur.
	/// </summary>
	public enum NotificationEvents
	{
		OnDataReceived,
		OnClientJoined,
		OnClientLeft
	}

	/// <summary>
	///  A fake implementation of the notification handler.
	/// </summary>
	public class FakeNotificationHandler : INotificationHandler
	{
		private readonly AutoResetEvent _autoResetEvent = new(false);
		private int _timeOutCount;
		public dynamic Data = new ExpandoObject();
		public dynamic Event = new ExpandoObject();

		public void OnDataReceived(string data)
		{
			Event = NotificationEvents.OnDataReceived;
			Data = data;

			if (_timeOutCount-- > 0)
			{
				return;
			}
			_autoResetEvent.Set();
		}

		public void OnClientJoined<T>(T socketObject)
		{
			Event = NotificationEvents.OnClientJoined;
			Data = socketObject;
			if (_timeOutCount-- > 0)
			{
				return;
			}
			_autoResetEvent.Set();
		}

		public void OnClientLeft(string clientId)
		{
			Event = NotificationEvents.OnClientLeft;
			Data = clientId;
			if (_timeOutCount-- > 0)
			{
				return;
			}
			_autoResetEvent.Set();
		}

		public void Wait(double timeOut = 15)
		{
			// wait for a maximum of timeOut seconds
			var signalReceived = _autoResetEvent.WaitOne(TimeSpan.FromSeconds(timeOut));
			if (signalReceived)
			{
				return;
			}
			// If the wait has timed out, increase the number of timeouts
			// this allows us to ignore messages that were previously timed out.
			_timeOutCount++;
			_autoResetEvent.Reset();
			throw new TimeoutException("Wait failed due to timeout!");
		}

		/// <summary>
		///     Reset Data and Event to null and also reset the AutoResetEvent
		/// </summary>
		public void Reset()
		{
			Event = null;
			Data = null;
			_autoResetEvent.Reset();
		}
	}


	public class Machine
	{
		public readonly FakeNotificationHandler SsHandler;
		public readonly FakeNotificationHandler WbHandler;

		protected Machine()
		{
			WbHandler = new FakeNotificationHandler();
			SsHandler = new FakeNotificationHandler();
		}

		public ICommunicator Communicator { get; set; }

		public void Subscribe()
		{
			Communicator.Subscribe(NetworkingGlobals.whiteboardName, WbHandler, NetworkingGlobals.whiteboardPriority);
			Communicator.Subscribe(NetworkingGlobals.screenshareName, SsHandler, NetworkingGlobals.screensharePriority);
		}

		public void Reset()
		{
			WbHandler.Reset();
			SsHandler.Reset();
		}
	}

	public class FakeClientA : Machine
	{
		public const string Id = "A";
		public FakeClientA()
		{
			Communicator = NetworkingGlobals.NewClientCommunicator;
		}
	}

	public class FakeClientB : Machine
	{
		public const string Id = "B";

		public FakeClientB()
		{
			Communicator = NetworkingGlobals.NewClientCommunicator;
		}
	}

	public class FakeServer : Machine
	{
		public FakeServer()
		{
			Communicator = NetworkingGlobals.NewServerCommunicator;
		}
	}
}
