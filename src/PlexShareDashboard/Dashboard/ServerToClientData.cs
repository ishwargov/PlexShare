using Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareDashboard.Dashboard.Server.Telemetry;

namespace PlexShare.Dashboard
{
    public class ServerToClientData
    {
        public UserData _user;

        public string eventType;
        public SessionAnalytics sessionAnalytics;
        public SessionData sessionData;
        public SummaryData summaryData;

        //     Parametric constructor to initialize the fields
        public ServerToClientData(string eventName, SessionData sessionDataToSend, SummaryData summaryDataToSend,
            SessionAnalytics sessionAnalyticsToSend, UserData user)
        {
            // SessionAnalytics sessionAnalyticsToSend
            eventType = eventName;
            _user = user;
            sessionData = sessionDataToSend;
            summaryData = summaryDataToSend;
            sessionAnalytics = sessionAnalyticsToSend;
        }

        //     Default constructor for serialization
        public ServerToClientData()
        {
        }

        //     Method to access the UserData object
        public UserData GetUser()
        {
            return _user;
        }
    }
}
