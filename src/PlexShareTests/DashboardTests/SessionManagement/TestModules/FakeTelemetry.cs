using Dashboard;
using PlexShareDashboard.Dashboard.Server.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareDashboard.Dashboard.Server.SessionManagement;
using PlexShare.Dashboard.Server.SessionManagement;
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

        public void SaveAnalytics(String allMessages)
        {

        }

        public SessionAnalytics GetTelemetryAnalytics(String allMessages)
        {
            return sessionAnalytics;
        }

        public void OnAnalyticsChanged(SessionData newSession)
        {
            analyticsChanged = true;
        }
    }
}
