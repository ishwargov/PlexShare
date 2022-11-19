///<author>Hrishi Raaj Singh Chauhan</author>
/// Here we are having a static persistence factory, It will create the instance of SummaryPersistence and TelemetryPersistence to access their functionalities
using PlexShareDashboard.Dashboard.Server.Telemetry;
namespace Dashboard.Server.Persistence
{
    //Persistence Factory
    /// <summary>
    /// Create instance of the SummaryPersistence and TelemetryPersistence
    /// </summary>
    public static class PersistenceFactory
    {
        private static readonly SummaryPersistence _summaryPersisitence;
        private static readonly TelemetryPersistence _telemetryPersisitence;
        /// <summary>
        /// Constructor of the factory which will create only a single instance of the SummaryPersistence and TelemetryPersistence
        /// </summary>
        static PersistenceFactory()
        {
            if (_summaryPersisitence == null) _summaryPersisitence = new SummaryPersistence();

            if (_telemetryPersisitence == null) _telemetryPersisitence = new TelemetryPersistence();
        }
        /// <summary>
        /// create a new instance of the SummaryPersistence if not already instantiated.
        /// </summary>
        /// <returns>returns the instance of the SummaryPersistence</returns>
        public static SummaryPersistence GetSummaryPersistenceInstance()
        {
            return _summaryPersisitence;
        }
        /// <summary>
        /// create a new instance of the TelemetryPersistence if not already instantiated.
        /// </summary>
        /// <returns>returns the instance of the TelemetryPersistence</returns>
        public static TelemetryPersistence GetTelemetryPersistenceInstance()
        {
            return _telemetryPersisitence;
        }
    }
}