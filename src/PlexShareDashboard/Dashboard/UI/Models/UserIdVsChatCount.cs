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
        public int UserId { get; set; }
        public int ChatCount { get; set; }

        //constructor this 
        public UserIdVsChatCount(int currUserId, int currChatCount)
        { 
            UserId = currUserId;
            ChatCount = currChatCount;  
        }
    }
}
