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
		private static readonly Lazy<IConnunicator> _clientCommunicator = new(() => new ClientCommunicator());
		private static readonly Lazy<IConnunicator> _serverCommunicator = new(() => new ServerCommunicator());

		/// <summary>
		/// 
		/// </summary>
		public static IConnunicator GetCommunicator(bool isClient = true, bool isTesting = false)
		{
			if (isTesting)
			{
				if (isClient)
				{
					return new ClientCommunicator();
				}
				return new ServerCommunicator();
			}
			if (isClient)
			{
				return _clientCommunicator.Value;
			}
			return _serverCommunicator.Value;
		}
	}
}
