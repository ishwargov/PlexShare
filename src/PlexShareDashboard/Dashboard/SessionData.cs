/****************************************
 * Name : Saurabh Kumar
 * Roll : 111901046
 * Module : Dashboard
 * File Name : SessionData.cs
 * This file contains the SessionData class used to store the list of users in the session.
 *****************************************/


using System;
using System.Collections.Generic;

namespace Dashboard
{
    /// <summary>
    ///     This class is used to store the data about the
    ///      current session
    /// </summary>

    public class SessionData
    {
        // the List of users in the meeting 
        public List<UserData> users;

        // default SessionMode is LabMode
        public string sessionMode;

        //used to store the session id
        public int sessionId;

        /// <summary>
        /// Constructor to initialise and empty list of users, generate Random no. for Session Id and initialise SessionMode as LabMode
        /// </summary>
        public SessionData()
        {
            Random rnd = new Random();
            if (users == null) users = new List<UserData>();
            sessionMode = "LabMode";
            sessionId = rnd.Next();
        }

        /// <summary>
        ///  Adds a user to the list of users in the session
        /// </summary>
        /// <param name="user"> An instance of the UserData class </param>
        public void AddUser(UserData user)
        {
            users.Add(user);
        }

        /// <summary>
        /// This function is used toggle the session Mode
        /// </summary>
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

        /// <summary>
        /// Removes the user from the user list in the sessionData.
        /// </summary>
        /// <param name="userID">The UserID of the user who is to be removed </param>
        /// <param name="username"> An optional paramter indicating the name of the user. </param>
        /// <returns></returns>        

        public UserData RemoveUserFromSession(int userID, string? username = null)
        {
            // Check if the user is in the list and if so, then remove it and return true
            for (var i = 0; i < users.Count; ++i)
                if (users[i].userID.Equals(userID))
                    lock (this)
                    {
                        UserData removedUser = new(users[i].username, users[i].userID, users[i].userEmail, users[i].userPhotoUrl);
                        users.RemoveAt(i);
                        return removedUser;
                    }

            return null;
        }




    }
}