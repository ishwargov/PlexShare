
/// This file contains the SessionData class used to store the list of users in the session.

using System;
using System.Collections.Generic;

namespace Dashboard
{

    ///     This class is used to store the data about the
    ///     current session

    public class SessionData
    {
        

        // the List of users in the meeting 
        public List<UserData> users;

        // default SessionMode is LabMode
        public string sessionMode;

        //used to store the session id
        public int sessionId;
        



        ///     Constructor to initialise and empty list of users
        public SessionData()
        {
            Random rnd = new Random();
            if (users == null) users = new List<UserData>();
            if (users == null) sessionMode = "LabMode";
            if (users == null) sessionId = rnd.Next();
        }

        ///     Adds a user to the list of users in the session

        /// An instance of the UserData class 
        public void AddUser(UserData user)
        {
            users.Add(user);
        }

        public void ToggleMode()
        {
            if (sessionMode == "LabMode")
            {
                sessionMode = "ExamMode";
            }
            else
            {
                sessionMode = "LabMode";
            }
        }


        ///     Overrides the ToString() method to pring the sessionData object for testing, debugging and logging.

        ///Returns a string which contains the data of each user separated by a newline character  
        public override string ToString()
        {
            var output = "";
            for (var i = 0; i < users.Count; ++i)
            {
                output += users[i].ToString();
                output += "\n";
            }

            return output;
        }


        ///     Removes the user from the user list in the sessionData.

        ///  The UserID of the user who is to be removed 
        ///   An optional paramter indicating the name of the user.  
        /// 
        public UserData RemoveUserFromSession(int userID, string? username = null)
        {
            // Check if the user is in the list and if so, then remove it and return true
            for (var i = 0; i < users.Count; ++i)
                if (users[i].userID.Equals(userID))
                    lock (this)
                    {
#pragma warning disable CS8604 // Possible null reference argument.
                        UserData removedUser = new(users[i].username, users[i].userID);
#pragma warning restore CS8604 // Possible null reference argument.
                        users.RemoveAt(i);
                        return removedUser;
                    }

            return null;
        }


        

    }
}