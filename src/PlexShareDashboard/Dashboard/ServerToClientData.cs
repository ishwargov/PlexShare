/******************************************
 * Name : Saurabh Kumar
 * Roll : 111901046
 * Module : Dashboard
 * File Name: ServerToClientData.cs
 * This file is for creating class for datamodel of ServerToClient data
 ******************************************/

using Dashboard;
using PlexShareDashboard.Dashboard.Server.Telemetry;

namespace PlexShareDashboard.Dashboard
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
