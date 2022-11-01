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
        public int userId { get; set; }
       
        public string userName { get; set; }
        
        public string status { get; set; }
       

        //constructor for the User 
        public User(int userId , string username, string statuss)
        {
            this.userId = userId;
            this.userName = username;
            this.status = statuss;
        }
    }


}
