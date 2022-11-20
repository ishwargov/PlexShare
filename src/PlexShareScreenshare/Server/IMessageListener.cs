/// <author>Mayank Singla</author>
/// <summary>
/// Defines the interface "IMessageListener" which will be implemented by
/// the server view model and used by the server data model to notify view model.
/// </summary>

using System.Collections.Generic;

namespace PlexShareScreenshare.Server
{
    /// <summary>
    /// Interface to be implemented by the server view model and used by
    /// the server data model to notify the view model.
    /// </summary>
    public interface IMessageListener
    {
        /// <summary>
        /// Notifies that subscribers list has been changed.
        /// This will happen when a client either starts or stops screen sharing.
        /// </summary>
        /// <param name="subscribers">
        /// Updated list of the subscribers.
        /// </param>
        public void OnSubscribersChanged(List<SharedClientScreen> subscribers);

        /// <summary>
        /// Notifies that a client has started screen sharing.
        /// </summary>
        /// <param name="clientId">
        /// Id of the client who started screen sharing.
        /// </param>
        /// <param name="clientName">
        /// Name of the client who started screen sharing.
        /// </param>
        public void OnScreenshareStart(string clientId, string clientName);

        /// <summary>
        /// Notifies that a client has stopped screen sharing.
        /// </summary>
        /// <param name="clientId">
        /// Id of the client who stopped screen sharing.
        /// </param>
        /// <param name="clientName">
        /// Name of the client who stopped screen sharing.
        /// </param>
        public void OnScreenshareStop(string clientId, string clientName);
    }
}
