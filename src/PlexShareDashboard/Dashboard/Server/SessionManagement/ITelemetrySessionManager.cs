using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareDashboard.Dashboard.Server.SessionManagement
{
    public interface ITelemetrySessionManager
    {
        //     Subscribes to changes in the session object
      public  void Subscribe(ITelemetryNotifications listener);
    }
}
