/*
 * Name : Saurabh Kumar
 * Roll : 111901046
 *  File Name :  ITelemetrySessionManager
 *  This file contains the interface for Telemetry to subscribe to Session manager.
 */
using PlexShareDashboard.Dashboard.Server.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

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
