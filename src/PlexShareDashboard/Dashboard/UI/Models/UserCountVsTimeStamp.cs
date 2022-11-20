/// <author>Rupesh Kumar</author>
/// <summary>
///     Class model to store the usercount and timestamp values 
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareDashboard.Dashboard.UI.Models
{
    public class UserCountVsTimeStamp
    {
        public int UserCount { get; set; }
        public int TimeStamp { get; set; }


        //constructor for this class 
        public UserCountVsTimeStamp(int currUserCount, int currTimeStamp)
        { 
            UserCount = currUserCount;
            TimeStamp = currTimeStamp;  
        
        }
    }
}
