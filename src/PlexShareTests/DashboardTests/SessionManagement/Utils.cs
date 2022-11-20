/*
 * Name : Saurabh Kumar
 * Roll : 111901046
 * FileNmae: Utils.cs
 * This file is have data of users so that we can use user for inserting while testing
 */
using Dashboard;

namespace PlexShareTests.DashboardTests.SessionManagement
{
    public class Utils
    {
        public static List<UserData> testUsers;

        public static List<UserData> GetUsers()
        {
            testUsers = new List<UserData>();
            testUsers.Add(new UserData("Jake", 1));
            testUsers.Add(new UserData("Simon", 2));
            testUsers.Add(new UserData("Michael", 3));

            return testUsers;
        }

        public static List<UserData> GetUsersSet2()
        {
            testUsers = new List<UserData>();
            testUsers.Add(new UserData("Elio", 1));
            testUsers.Add(new UserData("Simon", 2));
            testUsers.Add(new UserData("Benji", 3));

            return testUsers;
        }

        public static SessionData GetSessionData()
        {
            var sessionData = new SessionData();

            sessionData.AddUser(new UserData("Jake", 1));
            sessionData.AddUser(new UserData("Simon", 2));
            sessionData.AddUser(new UserData("Michael", 3));

            return sessionData;
        }
    }
}
