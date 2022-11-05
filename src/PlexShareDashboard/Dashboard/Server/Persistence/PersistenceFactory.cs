using PlexShareDashboard.Dashboard.Server.Telemetry;
namespace Dashboard.Server.Persistence
{
    public static class PersistenceFactory
    {
        private static readonly SummaryPersistence _summaryPersisitence;
        private static readonly TelemetryPersistence _telemetryPersisitence;

        static PersistenceFactory()
        {
            if (_summaryPersisitence == null) _summaryPersisitence = new SummaryPersistence();

            if (_telemetryPersisitence == null) _telemetryPersisitence = new TelemetryPersistence();
        }

        public static SummaryPersistence GetSummaryPersistenceInstance()
        {
            return _summaryPersisitence;
        }

        public static TelemetryPersistence GetTelemetryPersistenceInstance()
        {
            return _telemetryPersisitence;
        }
    }
}