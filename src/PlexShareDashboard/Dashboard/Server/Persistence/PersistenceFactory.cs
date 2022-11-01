/// <author>Hrishi Raaj Singh Chauhan</author>
/// <created>21/10/2022</created>
/// <summary>
///     It contains the static PersistenceFactory Class, It is the the entry point
///     of Persisitence Module. We first need to create instances of SummaryPersistence
///     and TelemetryPersistence to call different functions corresponding their interfaces
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
        public static ResponseEntity lastSaveResponse;

        /// <summary>
        ///     It is constructor for the Persistence factory and enables to create only
        ///     one instance in a single run.
        /// </summary>
        static PersistenceFactory()
        {
            if (_summaryPersisitence == null) _summaryPersisitence = new SummaryPersistence();

            if (_telemetryPersisitence == null) _telemetryPersisitence = new TelemetryPersistence();
        }

        /// <summary>Helps to create instance of ISummaryPersistence </summary>
        /// <returns> return ITelemetryPersistence Interface which enables to use functions of class inheriting the same </returns>
        public static SummaryPersistence GetSummaryPersistenceInstance()
        {
            return _summaryPersisitence;
        }

        /// <summary>Helps to create instance of ITelemetryPersistence </summary>
        /// <returns> return ITelemetryPersistence Interface which enables to use functions of class inheriting the same </returns>
        public static TelemetryPersistence GetTelemetryPersistenceInstance()
        {
            return _telemetryPersisitence;
        }
    }
}