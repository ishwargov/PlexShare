/// <author> Mohammad Umar Sultan </author>
/// <created> 16/10/2022 </created>
/// <summary>
/// This file contains the factory for the communicator which used factory design pattern.
/// </summary>

using PlexShareNetwork.Communication;
using System;

namespace PlexShareNetwork
{
	public static class CommunicationFactory
	{
		private static readonly CommunicatorClient _communicatorClient = new();
		private static readonly CommunicatorServer _communicatorServer = new();

        /// <summary>
        /// Factory function to get the communicator.
        /// </summary>
        /// <param name="isClientSide"> Boolean telling is it client side or server side. </param>
        /// <returns> The communicator singleton instance. </returns>
        public static ICommunicator GetCommunicator(bool isClientSide = true)
		{
			if (isClientSide)
			{
				return _communicatorClient;
			}
			return _communicatorServer;
		}
	}
}
