using PlexShareNetwork;
using PlexShareNetwork.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace PlexShareTests.DashboardTests.SessionManagement.TestModules
{
    public class FakeCommunicator : ICommunicator
    {
        public string meetAddress;
        public string transferredData;
        public int userCount;



        public FakeCommunicator(string address = null)
        {
            userCount = 0;
            
            if (address == null)
                meetAddress = "192.168.1.1:8080";
            else
                meetAddress = address;
        }

        public void AddClient(string clientId, TcpClient socketObject)
        {
            userCount++;
        }

        public void RemoveClient(string clientId)
        {
            userCount--;
        }

        public void Send(string data, string identifier)
        {

            transferredData = data;
        }

        public void Send(string data, string identifier, string destination)
        {
            transferredData = data;
            
        }

        public string Start(string serverIp = null, string serverPort = null)
        {
            if (serverIp == null && serverPort == null)
                return meetAddress;

            if (meetAddress == serverIp + ":" + serverPort)
                return "success";
            return "failure";
        }

        public void Stop()
        {
            userCount = 0;
        }

       /* public void Subscribe(string identifier, INotificationHandler handler, int priority)
        {
            throw new NotImplementedException();
        }
       */
        public void Subscribe(string moduleName, INotificationHandler notificationHandler, bool isHighPriority = false)
        {
            return;
        }
    }
}
