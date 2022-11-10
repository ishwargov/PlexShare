using PlexShareDashboard.Dashboard.Server.SessionManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareTests.DashboardTests.SessionManagement.TestModules
{
    public class FakeServerUX
    {
        public bool meetingEnded;

        public FakeServerUX(IUXServerSessionManager sessionManager)
        {
            meetingEnded = false;
            sessionManager.MeetingEnded += () => OnMeetingEnded();
        }

        public void OnMeetingEnded()
        {
            meetingEnded = true;
        }
    }
}
