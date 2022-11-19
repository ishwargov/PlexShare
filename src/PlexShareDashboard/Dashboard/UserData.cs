/*
 * Name : Saurabh Kumar
 * Roll : 111901046
 * File Name: UserData.cs
 * This file contains the SessionData class used to store the list of users in the session.
 */

using System.Collections.Generic;

namespace Dashboard
{

    ///     This class is used to store the data about the
    ///     current session

    public class UserData
    {
        public int userID;

        public string username;

        public string? userEmail;

        public string? userPhotoUrl;



        public UserData()
        {

        }

        //parametrized constructor
        public UserData( string clientName, int clientID, string? clientEmail = null , string? clientPhotoUrl = null )
        {
            userID = clientID;
            username = clientName;
            userEmail = clientEmail;
            userPhotoUrl = clientPhotoUrl;
        }
        /// <summary>
        /// This is used to compare two users
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(UserData other)
        {
            if (other == null)
                return false;

            return userID.Equals(other.userID) &&
                   (
                       ReferenceEquals(username, other.username) ||
                       username != null &&
                       username.Equals(other.username)
                   );
        }

    }
}