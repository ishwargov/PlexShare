/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains the class definition of CommunicatorClient.
/// </summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Networking
{
	internal class CommunicatorClient : ICommunicator
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		///  
		/// </returns>
		string ICommunicator.Start()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns> void </returns>
		void ICommunicator.Stop()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns> void </returns>
		void ICommunicator.AddClient()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns> void </returns>
		void ICommunicator.RemoveClient()
		{

		}

		/// <summary>
		/// Method to send data over the network.
		/// </summary>
		/// <returns> void </returns>
		void ICommunicator.Send()
		{

		}

		/// <summary>
		/// Other modules can subscribe using this method to be notified on receiving data over the network.
		/// </summary>
		/// <returns> void </returns>
		void ICommunicator.Subscribe()
		{

		}
	}
}