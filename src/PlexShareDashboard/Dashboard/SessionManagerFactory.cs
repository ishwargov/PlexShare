using Dashboard.Server.SessionManagement;
using PlexShareDashboard.Dashboard.Client.SessionManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShare.Dashboard
{
    public class SessionManagerFactory
    {
        private static readonly Lazy<ClientSessionManager> s_clientSessionManager =
           new(() => new ClientSessionManager());

        private static readonly Lazy<ServerSessionManager> s_serverSessionManager =
            new(() => new ServerSessionManager());

        //     This method will create a Client sided server
        ///     manager that will live till the end of the program
        public static ClientSessionManager GetClientSessionManager()
        {
            return s_clientSessionManager.Value;
        }

        //to do :add constructor for testing

        //     This method will server a Client sided server
        //     manager that will live till the end of the program
        public static ServerSessionManager GetServerSessionManager()
        {
            return s_serverSessionManager.Value;
        }

        //to do :add constructors for testing

    }
}
