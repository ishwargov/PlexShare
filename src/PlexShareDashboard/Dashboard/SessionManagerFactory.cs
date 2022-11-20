/*************************************
 * Name : Saurabh Kumar
 * Roll : 111901046
 * Module : Dashboard
 * File Name: SessionManagerFactory
 * This file contain the implemetation of SessionManager Factory
 *************************************/
using Dashboard.Server.SessionManagement;
using PlexShareDashboard.Dashboard.Client.SessionManagement;
using PlexShareNetwork.Communication;
using System;

namespace PlexShare.Dashboard
{
    public class SessionManagerFactory
    {
        private static readonly Lazy<ClientSessionManager> s_clientSessionManager = new(() => new ClientSessionManager());

        private static readonly Lazy<ServerSessionManager> s_serverSessionManager = new(() => new ServerSessionManager());

        /// <summary>
        ///     This method will create a Client sided server
        /// </summary>
        /// <returns> client session manager that will live till the end of the program </returns>
        public static ClientSessionManager GetClientSessionManager()
        {
            return s_clientSessionManager.Value;
        }


        // constructor for testing
        public static ClientSessionManager GetClientSessionManager(ICommunicator communicator)
        {
            return new ClientSessionManager(communicator);
        }


        //     This method will server a Client sided server
        //     manager that will live till the end of the program
        public static ServerSessionManager GetServerSessionManager()
        {
            return s_serverSessionManager.Value;
        }

        /// <summary>
        ///  This method will create a Server session 
        /// </summary>
        /// <param name="communicator"></param>
        /// <returns>return server session manager</returns>
        public static ServerSessionManager GetServerSessionManager(ICommunicator communicator)
        {
            return new ServerSessionManager(communicator);
        }

    }
}
