/**********************************************
 * Name : Saurabh Kumar
 * Roll : 111901046
 * Module : Dashboard
 * File Name :  IUXClientSessionManager 
 *********************************************/
using Dashboard;
using PlexShare.Dashboard.Client.SessionManagement;


namespace PlexShareDashboard.Dashboard.Client.SessionManagement
{

    // this is the interface for UX to access Client session manager's methods and fields.
    public interface IUXClientSessionManager
    {
        /// <summary>
        /// this method is used to add client to the meeting.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="ports"></param>
        /// <param name="username"></param>
        /// <param name="userEmail"></param>
        /// <param name="photoUrl"></param>
        /// <returns>returns true if user suceessfully added else false</returns>
        bool AddClient(string ipAddress, int ports, string username, string userEmail, string photoUrl);

        /// <summary>
        /// It is used to change the session mode from Lab Mode to Exam mode and vice-versa
        /// </summary>
        void ToggleSessionMode();

        /// <summary>
        /// It is used to remove the user from the meeting by deleting their data from the session
        /// </summary>
        void RemoveClient();

        /// <summary>
        /// It will end the meeting for all, creating and storing the summary and analytics
        /// </summary>
        void EndMeet();

        /// <summary>
        /// It would retrieve the summary of the chats that were send from start of the meet till the function was called to the client
        /// </summary>
        void GetSummary();

        /// <summary>
        /// it is used to subscribe for any changes in the Session object
        /// </summary>
        /// <param name="listener"></param>
        void SubscribeSession(IClientSessionNotifications listener);

        /// <summary>
        /// it will gather analytics of the users and messages
        /// </summary>
        void GetAnalytics();

        /// <summary>
        /// get the user data object from the client session manager
        /// </summary>
        /// <returns></returns>
        UserData GetUser();

        /// <summary>
        /// get the sessionData object from the client manager
        /// </summary>
        /// <returns></returns>
        SessionData GetSessionData();

        /// <summary>
        /// event for notifying summary creation
        /// </summary>
        public event NotifySummaryCreated SummaryCreated;

        /// <summary>
        /// event for notifying the end of meeting to the client UX
        /// </summary>
        public event NotifyEndMeet MeetingEnded;

        /// <summary>
        /// event for notyfying the creation of analytics to the client UX
        /// </summary>
        public event NotifyAnalyticsCreated AnalyticsCreated;

        /// <summary>
        /// event for notifying the change of session mode to the client UX
        /// </summary>
        public event NotifySessionModeChanged SessionModeChanged;
    }
}