/// <author>Rupesh Kumar</author>
/// <summary>
/// this is the interface for telemetry. This can be used to call the telemetry to all other dependent modoules or submodules. 
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PlexShareDashboard.Dashboard.Server.Telemetry
{

    //this is the interface for others to use the telemetry submodule 
    public interface ITelemetry
    {
        //function to save the analytics when the meeting ends 
        public void SaveAnalytics(PlexShareContent.DataModels.ChatThread[] allChatMessages);

        //function to get the telemetry analytics on demand 
        public SessionAnalytics GetTelemetryAnalytics(PlexShareContent.DataModels.ChatThread[] allChatMessages);

    }
}
