using Dashboard;
using PlexShare.Dashboard.Client.SessionManagement;
using System;


namespace PlexShareDashboard.Dashboard.Client.SessionManagement
{

    // this is the interface for UX to access Client session manager's methods and fields.
    public interface IUXClientSessionManager
    {
        //this method is used to add client to the meeting.It will take the IP address , port number and name of user.
        //it returns true if user suceessfully added else false
        bool AddClient(string ipAddress, int ports, string username, string userEmail, string photoUrl);

        //It is used to change the session mode from Lab Mode to Exam mode and vice-versa
        void ToggleSessionMode();

        //It is used to remove the user from the meetinh by deleting their data from the session
        void RemoveClient();

        //It will end the meeting for all, creating and storing the summary and analytics
        void EndMeet();

        //It would retrieve the summary of the chats that were send from start of the meet till the function was called to the client
        void GetSummary();

        //it is used to subscribe for any changes in the Session object
        void SubscribeSession(IClientSessionNotifications listener);

        //it will gather analytics of the users and messages
        void GetAnalytics();

        //get the user data object from the client session manager
        UserData GetUser();

        //event for notifying summary creation
        public event NotifySummaryCreated SummaryCreated;

        //event for notifying the end of meeting to the client UX
        public event NotifyEndMeet MeetingEnded;

        //event for notyfying the creation of analytics to the client UX
        public event NotifyAnalyticsCreated AnalyticsCreated;

        //event for notifying the change of session mode to the client UX
        public event NotifySessionModeChanged SessionModeChanged;
    }
}