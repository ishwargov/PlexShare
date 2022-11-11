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
        /// Updated list of the subscribers
        /// </param>
        public void OnSubscribersChanged(List<SharedClientScreen> subscribers);
    }
}
