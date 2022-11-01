using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShare.Dashboard.Server.SessionManagement
{
    internal interface ITelemetrySessionManager
    {
        //     Subscribes to changes in the session object
        void Subscribe(ITelemetryNotifications listener);
    }
}
