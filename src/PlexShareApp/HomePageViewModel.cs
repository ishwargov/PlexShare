//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using PlexShareDashboard;
//using PlexShareDashboard.Dashboard.Client.SessionManagement;
//using PlexShareDashboard.Dashboard.Server.SessionManagement;
//namespace PlexShareApp
//{
//    internal class HomePageViewModel
//    {

//        private readonly IUXClientSessionManager _clientmodel;
//        private readonly IUXServerSessionManager _sessionmodel;

//        public AuthViewModel()
//        {
           
//        }

//        public AuthViewModel(IUXClientSessionManager model)
//        {
//            _clientmodel = model;
//        }
//        public AuthViewModel(IUXServerSessionManager model)
//        {
//            _servermodel = model;
//        }

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
