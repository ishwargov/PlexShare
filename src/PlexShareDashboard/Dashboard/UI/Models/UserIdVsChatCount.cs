using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareDashboard.Dashboard.UI.Models
{

    // to store the value of chatcount corresponding to each userid
    public class UserIdVsChatCount
    {
        public int userId { get; set; }
        public int chatCount { get; set; }

        //constructor this 
        public UserIdVsChatCount(int currUserId, int currChatCount)
        { 
            userId = currUserId;
            chatCount = currChatCount;  
        }
    }
}
