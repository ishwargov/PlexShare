/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains all the global constants used by testing files
/// </summary>

using System;
using System.Linq;

namespace Networking
{
    public static class NetworkingGlobals
    {
        public const string dashboardName = "Dashboard", whiteboardName = "Whiteboard", screenshareName = "Screenshare";

        // Priorities of each module (true is for high priority)
        public const bool dashboardPriority = true, whiteboardPriority = false, screensharePriority = true;

        // Used to generate random strings
        private static Random random = new Random();

        /// <summary>
        /// Returns a randomly generated alphanumeric string
        /// </summary>
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
