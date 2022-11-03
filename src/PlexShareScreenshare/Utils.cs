/// <author>Mayank Singla</author>
/// <summary>
/// Defines the static "Utils" class which contains utility functions and constants
/// </summary>

namespace PlexShareScreenshare
{
    public static class Utils
    {
        /// <summary>
        /// The string representing the module identifier for screen share
        /// </summary>
        public const string ModuleIdentifier = "ScreenShare";

        /// <summary>
        /// The timeout value in "milliseconds" defining the timeout for
        /// the timer in SharedClientScreen which represents the maximum time
        /// to wait for the arrival of the packet with the CONFIRMATION header
        /// </summary>
        public const double Timeout = 5000;
    }
}
