/// <author>Mohammad Umar Sultan</author>
/// <created>16/10/2022</created>
/// <summary>
/// This file contains the ICommunicator interface.
/// </summary>

namespace Networking
{
	public interface ICommunicator
	{

		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		///  
		/// </returns>
		string Start();


		/// <summary>
		/// 
		/// </summary>
		/// <returns> void </returns>
		void Stop();


		/// <summary>
		/// 
		/// </summary>
		/// <returns> void </returns>
		void AddClient();

		/// <summary>
		/// 
		/// </summary>
		/// <returns> void </returns>
		void RemoveClient();

		/// <summary>
		/// Method to send data over the network.
		/// </summary>
		/// <returns> void </returns>
		void Send();

		/// <summary>
		/// Other modules can subscribe using this method to be notified on receiving data over the network.
		/// </summary>
		/// <returns> void </returns>
		void Subscribe();
	}
}
