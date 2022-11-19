/// <author> Hrishi Raaj Singh Chauhan </author>
/// <summary>
/// It contains the public interface provided to Telemetry module to save the session analytics.
/// </summary>

using PlexShareDashboard.Dashboard.Server.Telemetry;

namespace Dashboard.Server.Persistence
{
    // Interface for the Telemetry Module to save the analytics data.
    public interface ITelemetryPersistence
    {
        /// <summary>
        /// Save the session Analytics of the session
        /// </summary>
        /// <param name="sessionAnalyticsData">takes SessionAnalytics as input</param>
        /// <returns>true if saved sucessfuly</returns>
        public bool Save(SessionAnalytics sessionAnalyticsData);
    }
}