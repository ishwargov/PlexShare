/// <author>Hrishi Raaj Singh Chauhan</author>
/// <created>31/10/2022</created>
/// <summary>
///     It contains the public interface required by Summary Module to save Summary data.
/// </summary> 
///
using PlexShareDashboard.Dashboard.Server.Telemetry;
namespace Dashboard.Server.Persistence
{
    //SummaryPersistence Interface
    public interface ISummaryPersistence
    {
        public bool SaveSummary(string message);
    }
}