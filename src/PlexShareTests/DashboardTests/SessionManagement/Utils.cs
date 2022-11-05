using Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
