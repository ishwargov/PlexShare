using Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// This file contains the interface for Telemetry to listen to changes in session data.

namespace PlexShareDashboard.Dashboard.Server.SessionManagement
{
    public interface ITelemetryNotifications
    {
        //     Handles the change in the Global session (SessionData Object)
        void OnAnalyticsChanged(SessionData newSession);
    }
}
