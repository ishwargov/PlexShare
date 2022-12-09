/************************************************************
 * Filename    = FakeClientSessionMAnager.cs
 *
 * Author      = Jasir
 *
 * Product     = PlexShare
 * 
 * Project     = UX Team
 *
 * Description = Fake Client session manager for testing the HomePageViewModel
 * 
 ************************************************************/
using Dashboard;
using PlexShare.Dashboard.Client.SessionManagement;
using PlexShareDashboard.Dashboard.Client.SessionManagement;

namespace PlexShareTests.UX.HomePageTests
{
    public class FakeClientSessionManager : IUXClientSessionManager
    {
        public bool AddClient(string ipAddress, int ports, string username, string userEmail, string photoUrl)
        {
            if (string.IsNullOrWhiteSpace(ipAddress) || string.IsNullOrWhiteSpace(username) ||
                ipAddress.Contains(" "))
                return false;
            return true;
        }

        public void RemoveClient()
        {
        }

        public void EndMeet()
        {
        }

        public void GetSummary()
        {
        }

        public void GetAnalytics()
        {
        }

        public UserData GetUser()
        {
            throw new NotImplementedException();
        }


        public void ToggleSessionMode()
        {
            throw new NotImplementedException();
        }

        public void SubscribeSession(IClientSessionNotifications listener)
        {
            throw new NotImplementedException();
        }

        UserData IUXClientSessionManager.GetUser()
        {
            throw new NotImplementedException();
        }

        public SessionData GetSessionData()
        {
            return new SessionData();
        }

        // Event for notifying summary creation 
        public event NotifySummaryCreated? SummaryCreated;

        // Event for notifying the end of the meeting on the client side
        public event NotifyEndMeet? MeetingEnded;

        // Event for notifying the creation of anlalytics to the client UX.
        public event NotifyAnalyticsCreated? AnalyticsCreated;
        public event NotifySessionModeChanged? SessionModeChanged;
    }
}