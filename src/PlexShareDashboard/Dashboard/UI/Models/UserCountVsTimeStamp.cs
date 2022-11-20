/// <author>Rupesh Kumar</author>
/// <summary>
///     Class model to store the usercount and timestamp values 
/// </summary>


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
