/// <author> Mohammad Umar Sultan </author>
/// <created> 16/10/2022 </created>
/// <summary>
/// This file contains the factory for the communicator which uses
/// factory design pattern.
/// </summary>

using PlexShareNetwork.Communication;
using System;
using System.Diagnostics;

namespace PlexShareNetwork
{
    public static class CommunicationFactory
    {
        private static readonly CommunicatorClient _communicatorClient =
            new();
        private static readonly CommunicatorServer _communicatorServer =
            new();

        /// <summary>
        /// Factory function to get the communicator.
        /// </summary>
        /// <param name="isClientSide">
        /// Boolean telling is it client side or server side.
        /// </param>
        /// <returns> The communicator singleton instance. </returns>
        public static ICommunicator GetCommunicator(
            bool isClientSide = true)
        {
            Trace.WriteLine("[Networking] " +
                "CommunicationFactory.GetCommunicator() function " +
                "called with isClientSide: " + isClientSide.ToString());
            if (isClientSide)
            {
                return _communicatorClient;
            }
            return _communicatorServer;
        }
    }
}
