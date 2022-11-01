using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareDashboard.Dashboard.UI.Models
{
    public class UserCountVsTimeStamp
    {
        public int userCount { get; set; }
        public DateTime timeStamp { get; set; }


        //constructor for this class 
        public UserCountVsTimeStamp(int currUserCount, DateTime currTimeStamp)
        { 
            userCount = currUserCount;
            timeStamp = currTimeStamp;  
        
        }
    }
}
