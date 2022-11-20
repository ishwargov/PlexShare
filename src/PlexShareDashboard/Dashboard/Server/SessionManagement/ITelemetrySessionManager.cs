/******************************************
 * Name : Saurabh Kumar
 * Roll : 111901046
 * Module : Dashboard
 * File Name :  ITelemetrySessionManager
 * This file contains the interface for Telemetry to subscribe to Session manager.
 ******************************************/

namespace PlexShareDashboard.Dashboard.Server.SessionManagement
{
    public interface ITelemetrySessionManager
    {
        /// <summary>
        ///  Subscribes to changes in the session object
        /// </summary>
        /// <param name="listener"></param>    
        public void Subscribe(ITelemetryNotifications listener);
    }
}
