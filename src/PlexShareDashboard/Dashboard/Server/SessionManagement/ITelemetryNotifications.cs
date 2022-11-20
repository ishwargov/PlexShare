/**********************************************
 * Name : Saurabh Kumar
 * Roll : 111901046
 * Module : Dashboard
 * File Name : ITelemetryNotifications.cs
 * This file contains the interface for Telemetry to listen to changes in session data.
 **********************************************/
using Dashboard;

namespace PlexShareDashboard.Dashboard.Server.SessionManagement
{
    public interface ITelemetryNotifications
    {
        /// <summary>
        /// Handles the change in the SessionData Object
        /// </summary>
        /// <param name="newSession"></param>   
        void OnAnalyticsChanged(SessionData newSession);
    }
}
