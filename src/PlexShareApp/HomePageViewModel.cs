/************************************************************
 * Filename    = HomaPageViewModel.cs
 *
 * Author      = Jasir
 *
 * Product     = PlexShare
 * 
 * Project     = UX Team
 *
 * Description = All the bussiness logic for the HomePage
 * 
 ************************************************************/
using Dashboard;
using PlexShare.Dashboard;
using PlexShare.Dashboard.Client.SessionManagement;
using PlexShareDashboard.Dashboard.Client.SessionManagement;
using PlexShareDashboard.Dashboard.Server.SessionManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PlexShareApp
{
    public class HomePageViewModel : IClientSessionNotifications
    {
        IUXServerSessionManager serverSessionManager;
        IUXClientSessionManager clientSessionManager;
        public int sessionID;
        public HomePageViewModel()
        {
            serverSessionManager = SessionManagerFactory.GetServerSessionManager();
            clientSessionManager = SessionManagerFactory.GetClientSessionManager();
            clientSessionManager.SubscribeSession(this);
        }

        public HomePageViewModel(IUXClientSessionManager clientSessionManager, IUXServerSessionManager serverSessionManager)
        {
            this.serverSessionManager = serverSessionManager;
            this.clientSessionManager = clientSessionManager;
        }

        /// <summary>
        /// Downloading the public url in the temo folder.
        /// </summary>
        /// <param name="url">Public URL</param>
        /// <param name="userEmail">Email ID of the user</param>
        /// <returns>Absolute path in the current machine</returns>
        public string DownloadImage(string url, string userEmail)
        {
            Trace.WriteLine("[UX] ", url);
            // Imagename would be  the roll number of the user
            string imageName = "";
            int len_email = userEmail.Length;
            for (int i = 0; i < len_email; i++)
            {
                if (userEmail[i] == '@')
                    break;
                imageName += userEmail[i];
            }
            string? dir = "./Resources/";

            // Global Environment path for the temp directory
            dir = Environment.GetEnvironmentVariable("temp", EnvironmentVariableTarget.User);
            string absolute_path = System.IO.Path.Combine(dir, imageName);
            try
            {
                if (url.Length == 0)
                    throw new Exception("URL Empty");
                if (File.Exists(absolute_path))
                {
                    return absolute_path;
                }
                using (WebClient webClient = new())
                {
                    webClient.DownloadFile(url, absolute_path);
                }
                if (File.Exists(absolute_path) == false)
                    throw new Exception("File Not found");
            }
            catch (Exception e)
            {
                // If URL is not found
                absolute_path = "./Resources/AuthScreenImg.jpg";
                Trace.WriteLine("[UX] Download Image Exception : ", e.Message);
            }

            return absolute_path;
        }

        /// <summary>
        /// Validate username by checking if its not empty
        /// </summary>
        /// <param name="name">username</param>
        /// <returns>Boolean</returns>
        bool ValidateUserName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            return true;
        }


        /// <summary>
        /// Checks if the IP address is valid or not
        /// </summary>
        /// <param name="ip">IP address in a string format</param>
        /// <returns>true if valid else false</returns>
        private bool ValidateIP(string ip)
        {
            if (ip.Length == 0)
                return false;
            string[] ipTokens = ip.Split('.');
            if (ipTokens.Length != 4)
                return false;
            foreach (string token in ipTokens)
            {
                if (token.Length == 0)
                    return false;
                int tokenValue = Int32.Parse(token);
                //System.Diagnostics.Debug.WriteLine(token_value.ToString
                if (tokenValue < 0 || tokenValue > 255)
                    return false;
            }
            var byteValues = ip.Split('.');
            // IPV4 contains 4 bytes separated by .
            if (byteValues.Length != 4) return false;
            // We have 4 bytes in a 
            // for each part(elements of byteValues list), we check whether the string 
            // can be successfully converted into a byte or not.
            return byteValues.All(r => byte.TryParse(r, out var tempForParsing));
        }

        /// <summary>
        /// Validating IP address
        /// </summary>
        /// <param name="ip">IP address</param>
        /// <returns>True if Valid IP</returns>
        bool ValidateIpAddress(string ip)
        {
            // Server Session IP Address
            if (ip == "-1")
                return true;
            if (string.IsNullOrEmpty(ip) || !ValidateIP(ip))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Funciton to validate port. PORT number should not be less than 1024 as it authorized users only
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        bool ValidatePort(string port)
        {
            if (string.IsNullOrEmpty(port) || port.Length > 18 || Int64.Parse(port) <= 1024 || Int64.Parse(port) > 65535)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Uses the dashboard's function to assess if the credentials entered is okay
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns>List<string> { ip, port, isValidUserName, isValidIpAddress, isValidPort, isServer, verified}</string></returns>
        public List<string> VerifyCredentials(string name, string ip, string port, string email, string url)
        {
            Trace.WriteLine("[UX] Enetering HomeScreen Now");
            // Validating the arguments passed
            bool isValidUserName = ValidateUserName(name), isServer = ip == "-1" ? true : false;
            bool isValidIpAddress = true, isValidPort = true;
            if (isServer == false)
            {
                isValidIpAddress = ValidateIpAddress(ip);
                isValidPort = ValidatePort(port);
            }
            bool isVerified = false;
            if (isValidUserName && isValidIpAddress && isValidPort)
            {
                if (isServer)
                {
                    Trace.WriteLine("[UX] Instaniating a server");
                    // Getting IP and PORT from the Server session manager
                    MeetingCredentials meetingCredentials = serverSessionManager.GetPortsAndIPAddress();
                    isVerified = clientSessionManager.AddClient(meetingCredentials.ipAddress, meetingCredentials.port, name, email, url);
                    ip = meetingCredentials.ipAddress;
                    port = meetingCredentials.port.ToString();
                }
                else
                {
                    Trace.WriteLine("[UX] Instaniating a client");
                    isVerified = clientSessionManager.AddClient(ip, int.Parse(port), name, email, url);
                }
            }
            // Return as List of strings
            List<string> result = new List<string>();
            result.Add(ip);
            result.Add(port);
            result.Add(isValidUserName.ToString());
            result.Add(isValidIpAddress.ToString());
            result.Add(isValidPort.ToString());
            result.Add(isServer.ToString());
            result.Add(isVerified.ToString());
            Thread.Sleep(2000);
            result.Add(clientSessionManager.GetSessionData().sessionId.ToString());
            Trace.WriteLine("[UX] The client verification returned : " + isVerified);
            return result;
        }
         
       

        public void OnClientSessionChanged(SessionData session)
        {
            sessionID = session.sessionId;
        }
    }
}