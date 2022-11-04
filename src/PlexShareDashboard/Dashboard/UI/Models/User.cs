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
        
        public string Status { get; set; }
       

        //constructor for the User 
        public User(int UserId , string username, string Statuss)
        {
            this.UserId = UserId;
            this.UserName = username;
            this.Status = Statuss;
        }
    }


}
