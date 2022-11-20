/************************************************************
 * Filename    = FakeServerSessionManager.cs
 *
 * Author      = Jasir
 *
 * Product     = PlexShare
 * 
 * Project     = UX Team
 *
 * Description = Fake server session manager for testing the HomePageViewModel
 * 
 ************************************************************/
using Dashboard;
using Dashboard.Server.SessionManagement;
using PlexShareDashboard.Dashboard.Server.SessionManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareTests.UXTests.HomePageTests
{
    internal class FakeServerSessionManager : IUXServerSessionManager
    {
        private MeetingCredentials? _meetingCredentials;
        public MeetingCredentials GetPortsAndIPAddress()
        {
            _meetingCredentials = new MeetingCredentials("192.168.10.11", 12330);
            return _meetingCredentials;
        }

        public event NotifyEndMeet? MeetingEnded;
    }
}