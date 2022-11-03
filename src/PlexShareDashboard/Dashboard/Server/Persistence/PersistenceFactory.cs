/// <author>Hrishi Raaj Singh Chauhan</author>
/// <created>31/10/2022</created>
/// <summary>
///     It contains the static PersistenceFactory Class.
/// </summary>
//using PlexShareDashboard.Dashboard.Server.Persistence;
using PlexShareDashboard.Dashboard.Server.Telemetry;
namespace Dashboard.Server.Persistence
{
    //Persisence Factory
    /// <summary>
    ///     create instances of Summamry or Telemetry Persistence Class respectively
    /// </summary>
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