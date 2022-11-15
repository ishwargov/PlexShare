using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareDashboard.Dashboard.Server.Telemetry
{
    public class SessionAnalytics
    {
        public Dictionary<int, int> chatCountForEachUser;
        public Dictionary<string, int> userNameVsChatCount;

        public List<int> listOfInSincereMembers;

        public Dictionary<DateTime, int> userCountVsTimeStamp;

        public SessionSummary sessionSummary;
    }
}
