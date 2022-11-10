//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using PlexShareDashboard;
//using Dashboard.Client.SessionManagement;
//using Dashboard.Server.SessionManagement;
//namespace PlexShareApp
//{
//    internal class HomePageViewModel
//    {

//        private readonly IUXClientSessionManager _model;

//        public AuthViewModel()
//        {
//            _clientmodel = SessionManagerFactory.GetClientSessionManager();
//            _servemodel = SessionManagerFactory.GetServerSessionManager();
//        }

//        /// <summary>
//        ///     Constructor for testing purpose
//        /// </summary>
//        /// <param name="model"> Instance of type IUXClientSessionManager. </param>
//        public AuthViewModel(IUXClientSessionManager model)
//        {
//            _clientmodel = model;
//        }
//        public AuthViewModel(IUXServerSessionManager model)
//        {
//            _servermodel = model;
//        }

//        /// <summary>
//        ///     Sends the credentials entered by user to join a room to the respective method implemented by Session Manager
//        /// </summary>
//        /// <param name="ip"> IP Address of the server that started the meeting. </param>
//        /// <param name="port"> port number. </param>
//        /// <param name="username"> Name of the user. </param>
//        /// <returns> Boolean denoting the success or failure of whether the login attempt is valid </returns>

//        public void CreateSession(string username)
//        {
//            MeetingCredentials _meetingcredential = _servemodel.GetPortsAndIPAddress();
//            SendForAuth(_meetingcredential.ip, _meetingcredential.port, username);
//        }

//        public bool SendForAuth(string ip, int port, string username)
//        {
//            try
//            {
//                var response = _model.AddClient(ip, port, username);
//                return response;
//            }
//            catch (Exception _)
//            {
//                return false;
//            }
//        }
//    }
//}
