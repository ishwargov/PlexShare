using Client.Models;
using PlexShare.Dashboard.Server.SessionManagement;
using PlexShare.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareDashboard;
//using Dashboard.Server.Persistence;
//using PlexShareDashboard.Dashboard.Server.Persistent;
using Dashboard.Server.Persistence;
using Dashboard;

namespace PlexShareDashboard.Dashboard.Server.Telemetry
{
    public class Telemetry : ITelemetry, ITelemetryNotifications
    {
        //getting the sessionmanager and persistence instance using the corresponding factory
        private readonly ITelemetrySessionManager serverSessionManager = SessionManagerFactory.GetServerSessionManager();
        private readonly TelemetryPersistence persistence = PersistenceFactory.GetTelemetryPersistenceInstance();
        private readonly int thresholdTime = 10;


        //defining the variables to store the telemteric data 
        public Dictionary<DateTime, int> userCountVsEachTimeStamp = new Dictionary<DateTime, int>();
        public Dictionary<UserData,DateTime> eachUserEnterTimeInMeeting  = new Dictionary<UserData,DateTime>();
        public Dictionary<UserData, DateTime> eachUserExitTime = new Dictionary<UserData, DateTime>();
        public Dictionary<int, int> userIdVsChatCount = new Dictionary<int, int>();
        public List<int> listOfInSincereMembers = new List<int>();

        //constructor for telemetry module 
        
        public Telemetry()
        {
            //we have to subscribe to the ITelemetryNotifications 
            serverSessionManager.Subscribe(this);
            
        }


        public SessionAnalytics GetTelemetryAnalytics(string allChatMessages)
        {

            GetUserIdVsChatCount(allChatMessages);
            GetListOfInsincereMembers();

            var currTotalChatCount = 0;
            var currTotalUser = 0;


            //using the for loop to find these values 
            foreach (var eachUser in userIdVsChatCount)
            {
                currTotalChatCount = currTotalChatCount + eachUser.Value;
                currTotalUser = currTotalUser + 1;
            }



            //here goes the implementation 
            SessionAnalytics currSessionAnalytics = new SessionAnalytics();
            currSessionAnalytics.chatCountForEachUser = userIdVsChatCount;
            currSessionAnalytics.listOfInSincereMembers = listOfInSincereMembers;
            currSessionAnalytics.userCountVsTimeStamp = userCountVsEachTimeStamp;
            currSessionAnalytics.sessionSummary.chatCount = currTotalChatCount;
            currSessionAnalytics.sessionSummary.userCount = currTotalUser;

            return currSessionAnalytics;
        }

        //function fetch the details from the chatcontext and then giving it to persistent to save the analytics on the server 
        public void SaveAnalytics(string allChatMessages)
        {
            GetUserIdVsChatCount(allChatMessages);
            GetListOfInsincereMembers();
            var currTotalUser = 0;
            var currTotalChatCount = 0;

            //using the for loop to find these values 
            foreach(var eachUser in userIdVsChatCount)
            {
                currTotalChatCount = currTotalChatCount + eachUser.Value;
                currTotalUser = currTotalUser + 1;
            }


            var finalSessionAnalyticsToSave = new SessionAnalytics();
            finalSessionAnalyticsToSave.chatCountForEachUser = userIdVsChatCount;
            finalSessionAnalyticsToSave.listOfInSincereMembers = listOfInSincereMembers;
            finalSessionAnalyticsToSave.userCountVsTimeStamp = userCountVsEachTimeStamp;
            finalSessionAnalyticsToSave.sessionSummary.chatCount = currTotalChatCount;
            finalSessionAnalyticsToSave.sessionSummary.userCount = currTotalUser;


            //calling the persistent module to save these analytics 
            persistence.Save(finalSessionAnalyticsToSave);


            //say everything went fine 
            return;
        }

        public void GetUserIdVsChatCount(string allMesssages)
        {
            //we have to implement when we start integrating with the chat module 

            //say everything went fine 
            return;
        }

        public void GetListOfInsincereMembers()
        {
            //using the for loop to find who all users are insincere 
            foreach (var currElelement in eachUserEnterTimeInMeeting)
            {
                var currUserData = currElelement.Key;

                //if this user is present in the exit array then this has left 
                if (eachUserExitTime.ContainsKey(currUserData) && eachUserExitTime[currUserData].Subtract(currElelement.Value).TotalMinutes < thresholdTime)
                {
                    //then this is considered as the insincere member hence we have to add this to the list 
                    listOfInSincereMembers.Add(currUserData.userID);
                }
            }
            //say everything went fine 
            return;
        }

        public void OnAnalyticsChanged(SessionData newSession)
        {
            var currTime = DateTime.Now;
            //we have to recalculate and  update the telemetric analytics
            CalculateUserCountVsTimeStamp(newSession, currTime);
            CalculateArrivalExitTimeOfUser(newSession, currTime);

            return;

        }


        //function to update calculateUserCountVsTimeStamp 
        public void CalculateUserCountVsTimeStamp(SessionData newSession, DateTime currTime)
        {
            userCountVsEachTimeStamp[currTime] = newSession.users.Count;
        
        }


        //function to calculate the arrival and exit time of the users 
        public void CalculateArrivalExitTimeOfUser(SessionData newSession, DateTime currTime)
        {
            foreach (var currUser in newSession.users)
            {
                if (eachUserEnterTimeInMeeting.ContainsKey(currUser) == false)
                    eachUserEnterTimeInMeeting[currUser] = currTime;



            }
            //checking for the left users 
            foreach (var currUser in eachUserEnterTimeInMeeting)
            { 
                if (newSession.users.Contains(currUser.Key) == false && eachUserEnterTimeInMeeting.ContainsKey(currUser.Key) == false)
                    eachUserEnterTimeInMeeting[currUser.Key] = currTime;
            
            }

            //say everything went fine 
            return;
        }




        //function to get the useridvschatcount 
        //public 
    }
}
