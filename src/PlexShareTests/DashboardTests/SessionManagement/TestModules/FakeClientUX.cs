using Dashboard;
using PlexShare.Dashboard.Client.SessionManagement;
using PlexShareDashboard.Dashboard.Client.SessionManagement;
using PlexShareDashboard.Dashboard.Server.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareTests.DashboardTests.SessionManagement.TestModules
{
    
        public class FakeClientUX : IClientSessionNotifications
        {
            public bool meetingEnded;
            public SessionAnalytics sessionAnalytics;
            public SessionData sessionData;
            public string sessionSummary;

        public FakeClientUX(IUXClientSessionManager sessionManager)
        {
            meetingEnded = false;
            sessionManager.MeetingEnded += () => OnMeetingEnded();
            sessionManager.AnalyticsCreated += sessionAnalytics => OnAnalyticsChanged(sessionAnalytics);
            sessionManager.SummaryCreated += summary => OnSummaryCreated(summary);
            sessionManager.SubscribeSession(this);
            sessionData = null;
        }

        public void OnClientSessionChanged(SessionData session)
        {
            sessionData = new SessionData();
            sessionData = session;
        }

        public void OnAnalyticsChanged(SessionAnalytics analytics)
        {
            sessionAnalytics = analytics;
        }

        public void OnMeetingEnded()
        {
            meetingEnded = true;
        }

        public void OnSummaryCreated(string summary)
        {
            sessionSummary = summary;
        }
    }
}

