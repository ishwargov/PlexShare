// defining the datastructure for the Meeting Credential

namespace Dashboard
{
    public class MeetingCredentials
    {
        public string ipAddress;
        public int port;

       
        ///     Instances of this class will store the
        ///     credentials required to join/start
        
       
        public MeetingCredentials(string address, int portNumber)
        {
            ipAddress = address;
            port = portNumber;
        }
    }
}