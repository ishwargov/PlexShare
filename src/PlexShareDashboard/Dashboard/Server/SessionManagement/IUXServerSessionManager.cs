
using Dashboard;
using Dashboard.Server.SessionManagement;
/// This file contains the interface for UX to access Client session manager's methods and fields.
namespace PlexShareDashboard.Dashboard.Server.SessionManagement
{
    public interface IUXServerSessionManager
    {

        //     Returns the credentials required to
        //     Join or start the meeting

        public MeetingCredentials GetPortsAndIPAddress();


        ///     Event to notify the UX Server about the end of the meeting.

        public event NotifyEndMeet MeetingEnded;
    }
}