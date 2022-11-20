/// <author>Rupesh Kumar</author>
/// <summary>
/// class model to store the userid and chat count values 
/// </summary>


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
