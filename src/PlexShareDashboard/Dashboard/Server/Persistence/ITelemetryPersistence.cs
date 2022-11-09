using PlexShareDashboard.Dashboard.Server.Telemetry;

namespace Dashboard.Server.Persistence
{
    public interface ITelemetryPersistence
    {
        public bool Save(SessionAnalytics sessionAnalyticsData);
    }
}