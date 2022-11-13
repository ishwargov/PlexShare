using Dashboard;
using PlexShareDashboard.Dashboard.Server.Telemetry;
using PlexShareContent.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareDashboard.Dashboard.Server.SessionManagement;
using PlexShareDashboard.Dashboard.Server.SessionManagement;
namespace PlexShareTests.DashboardTests.SessionManagement.TestModules
{
    public class FakeTelemetry : ITelemetry, ITelemetryNotifications
    {
        public bool analyticsChanged;
        public SessionAnalytics sessionAnalytics;
        public FakeTelemetry(ITelemetrySessionManager sessionManager)
        {
            sessionManager.Subscribe(this);
            analyticsChanged = false;
            sessionAnalytics = new SessionAnalytics();
        }

        public void SaveAnalytics(PlexShareContent.DataModels.ChatThread[] allMessages)
        {

        }

        public SessionAnalytics GetTelemetryAnalytics(PlexShareContent.DataModels.ChatThread[] allMessages)
        {
            return sessionAnalytics;
        }

        public void OnAnalyticsChanged(SessionData newSession)
        {
            analyticsChanged = true;
        }
    }
}
