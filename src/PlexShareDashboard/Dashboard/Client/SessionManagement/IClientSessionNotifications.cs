using Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// This file contains the interface to listen to changes in  Client session manager's session data.

namespace PlexShare.Dashboard.Client.SessionManagement
{
    //     Interface to notify about changes in the client side
    //     session data (SessionData Object).
    public interface IClientSessionNotifications
    {
        //     Handles the changes in the SessionData object
        void OnClientSessionChanged(SessionData session);
    }
}
