/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains the factory for the communicator which used factory design pattern.
/// </summary>

using PlexShareNetwork.Communication;
using System;

namespace PlexShareNetwork
{
	public static class CommunicationFactory
	{
		private static readonly CommunicatorClient _clientCommunicator = new();
		private static readonly CommunicatorServer _serverCommunicator = new();

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
				return _clientCommunicator;
			}
			return _serverCommunicator;
		}
	}
}
