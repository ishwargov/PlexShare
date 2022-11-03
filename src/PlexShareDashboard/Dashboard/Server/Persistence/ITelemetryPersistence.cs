/// <author>Hrishi Raaj</author>
/// <created>31/10/2022</created>
/// <summary>
///     It contains the public interface required by Telemetry Module to SAVE different Analytics data
/// </summary> 

using PlexShareDashboard.Dashboard.Server.Telemetry;

//Persistence
namespace Dashboard.Server.Persistence
{
    public interface ITelemetryPersistence
    {
        public ResponseEntity Save(SessionAnalytics sessionAnalyticsData);
    }
}