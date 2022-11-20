/// <author>Rupesh Kumar</author>
/// <summary>
/// Class model to store the information for each user.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class User
    {
        //storing the details about the user 
        public int UserId { get; set; }
       
        public string UserName { get; set; }
        
        //stores whether user is presenting the screen or not 
        public string Status { get; set; }
        public string UserProfilePicPath { get; set; }
       

        //constructor for the User 
        public User(int UserId , string username, string Statuss, string userProfilePicPath)
        {
            this.UserId = UserId;
            this.UserName = username;
            this.Status = Statuss;
            this.UserProfilePicPath = userProfilePicPath;
        }
    }


}
