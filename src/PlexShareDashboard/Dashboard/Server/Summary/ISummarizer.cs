using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PlexShareDashboard.Dashboard.Server.Telemetry;

namespace PlexShareDashboard.Dashboard.Server.Summary
{
    public interface ISummarizer
    {
        string GetSummary(ChatThread[] chats);
        bool SaveSummary(ChatThread[] chats);

    }
}
