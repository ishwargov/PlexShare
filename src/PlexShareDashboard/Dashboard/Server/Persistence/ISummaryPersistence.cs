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
        /// <summary>
        ///     saves the summary of the session into a summary file
        /// </summary>
        /// <param name="message"> takes message string that need to be saved </param>
        /// <returns> return true if succesfully saved else return false </returns>
        public bool SaveSummary(string message);
    }
}