using PlexShareDashboard.Dashboard.Server.Telemetry;
namespace Dashboard.Server.Persistence
{
    public interface ISummaryPersistence
    {
        public bool SaveSummary(string message);
    }
}