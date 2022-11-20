using Dashboard;
using PlexShareDashboard.Dashboard.Server.Telemetry;
using PlexShareContent.DataModels;
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

        public void SaveAnalytics(ChatThread[] allMessages)
        {

        }

        public SessionAnalytics GetTelemetryAnalytics(ChatThread[] allMessages)
        {
            return sessionAnalytics;
        }

        public void OnAnalyticsChanged(SessionData newSession)
        {
            analyticsChanged = true;
        }
    }
}
