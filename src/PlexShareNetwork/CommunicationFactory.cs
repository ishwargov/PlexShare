/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains the factory for the communicator which used factory design pattern.
/// </summary>

using System;

namespace Networking
{
	public static class CommunicationFactory
	{
		private static readonly Lazy<ICommunicator> _clientCommunicator = new(() => new CommunicatorClient());
		private static readonly Lazy<ICommunicator> _serverCommunicator = new(() => new CommunicatorServer());

		/// <summary>
		/// 
		/// </summary>
		public static ICommunicator GetCommunicator(bool isClient = true, bool isTesting = false)
		{
			if (isTesting)
			{
				if (isClient)
				{
					return new CommunicatorClient();
				}
				return new CommunicatorServer();
			}
			if (isClient)
			{
				return _clientCommunicator.Value;
			}
			return _serverCommunicator.Value;
		}
	}
}
