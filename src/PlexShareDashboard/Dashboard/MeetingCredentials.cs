/***************************************
 * Name : Saurabh Kumar
 * Roll : 111901046
 * Module : Dashboard
 * FileName: MeetingCredentials.cs
 *  this file implemets the datastructure for the Meeting Credential
 ***************************************/

namespace Dashboard
{
    public class MeetingCredentials
    {
        public string ipAddress;
        public int port;

        /// <summary>
        ///  Instances of this class will store the
        /// credentials required to join/start the meeting
        /// </summary>
        /// <param name="address"></param>
        /// <param name="portNumber"></param>

        public MeetingCredentials(string address, int portNumber)
        {
            ipAddress = address;
            port = portNumber;
        }
    }
}