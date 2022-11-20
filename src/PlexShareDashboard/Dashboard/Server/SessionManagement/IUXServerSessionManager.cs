/****************************************************
 * Name : Saurabh Kumar
 * Roll : 111901046
 * Module : Dashboard
 * File Name :  IUXServerSessionManager
 * This file contains the interface for UX to access Client session manager's methods and fields.
 ****************************************************/
using Dashboard;
using Dashboard.Server.SessionManagement;

namespace PlexShareDashboard.Dashboard.Server.SessionManagement
{
    public interface IUXServerSessionManager
    {

        /// <summary>
        /// Returns the credentials required to
        ///     Join or start the meeting
        /// </summary>
        /// <returns>Meeting Credentials</returns>
        public MeetingCredentials GetPortsAndIPAddress();



        /// <summary>
        ///     Event to notify the UX Server about the end of the meeting.
        /// </summary>
        public event NotifyEndMeet MeetingEnded;
    }
}