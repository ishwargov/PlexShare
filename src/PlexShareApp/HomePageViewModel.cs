using Dashboard;
using PlexShare.Dashboard;
using PlexShare.Dashboard.Client.SessionManagement;
using PlexShareDashboard.Dashboard.Client.SessionManagement;
using PlexShareDashboard.Dashboard.Server.SessionManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlexShareApp
{
    public class HomePageViewModel : IClientSessionNotifications
    {
    
        bool isServer;
        IUXServerSessionManager serverSessionManager;
        IUXClientSessionManager clientSessionManager;
        public int sessionID;


        public HomePageViewModel()
        {
            serverSessionManager = SessionManagerFactory.GetServerSessionManager();
            clientSessionManager = SessionManagerFactory.GetClientSessionManager();
            clientSessionManager.SubscribeSession(this);
        }

        /// <summary>
        /// Uses the dashboard's function to assess if the credentials entered is okay
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public List<string> VerifyCredentials(string name, string ip, string port, string email, string url)
        {
            Trace.WriteLine("[UX] Enetering HomeScreen Now");
            bool verified = false;
            
            if (ip == "-1")
            {
                Trace.WriteLine("[UX] Instaniating a server");
                MeetingCredentials meetingCredentials = serverSessionManager.GetPortsAndIPAddress();
                verified = clientSessionManager.AddClient(meetingCredentials.ipAddress, meetingCredentials.port, name, email, url);
                ip = meetingCredentials.ipAddress;
                port = meetingCredentials.port.ToString();
                isServer = true;
            }
            else
            {
                Trace.WriteLine("[UX] Instaniating a client");
                verified = clientSessionManager.AddClient(ip, int.Parse(port), name, email, url);
                isServer = false;
            }

            List<string> result = new List<string>();
            result.Add(verified.ToString());
            result.Add(ip);
            result.Add(port);
            Thread.Sleep(1000);
            result.Add(clientSessionManager.GetSessionData().sessionId.ToString());
            //result.Add(sessionID.ToString());
            Trace.WriteLine("[UX] The client verification returned : " + verified);
            return result;
        }
        public void OnClientSessionChanged(SessionData currentSession)
        {
            sessionID = currentSession.sessionId;
        }
    }

}


