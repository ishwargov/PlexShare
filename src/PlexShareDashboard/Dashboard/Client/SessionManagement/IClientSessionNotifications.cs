/*
 * Name : Saurabh Kumar
 * Roll : 111901046
 * File Name :IClientSessionNotifications
 */
using Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// This file contains the interface to listen to changes in  Client session manager's session data.

namespace PlexShare.Dashboard.Client.SessionManagement
{   
    /// <summary>
    ///  Interface to notify about changes in the client side
    ///     session data .
    /// </summary>
    public interface IClientSessionNotifications
    {  
        /// <summary>
        /// Handles the changes in the SessionData object
        /// </summary>
        /// <param name="session"></param>
        void OnClientSessionChanged(SessionData session);
    }
}
