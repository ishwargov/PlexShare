/// <author>Mayank Singla</author>
/// <summary>
/// Defines the interface "ITimerManager" which will be implemented by the
/// server model which will contain the timeout callback for screen sharing.
/// </summary>

using System.Timers;

namespace PlexShareScreenshare.Server
{
    /// <summary>
    /// Interface to be implemented by the server model which will contain the
    /// timeout callback for screen sharing.
    /// </summary>
    public interface ITimerManager
    {
        /// <summary>
        /// Callback which will be invoked when the timeout occurs for the
        /// CONFIRMATION packet not received by the client.
        /// </summary>
        /// <param name="source">
        /// Default argument passed by the "Timer" class
        /// </param>
        /// <param name="e">
        /// Default argument passed by the "Timer" class
        /// </param>
        /// <param name="id">
        /// The ID of the client for which the timeout occurred
        /// </param>
        public void OnTimeOut(object? source, ElapsedEventArgs e, string id);
    }
}
