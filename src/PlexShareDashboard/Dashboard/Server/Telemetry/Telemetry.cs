using Client.Models;
using PlexShareDashboard.Dashboard.Server.SessionManagement;
using PlexShare.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareDashboard;
using PlexShareContent.DataModels;
//using Dashboard.Server.Persistence;
//using PlexShareDashboard.Dashboard.Server.Persistent;
using Dashboard.Server.Persistence;
using Dashboard;
using System.Runtime.InteropServices;

namespace PlexShareDashboard.Dashboard.Server.Telemetry
{
    public class Telemetry : ITelemetry, ITelemetryNotifications
    {
        //getting the sessionmanager and persistence instance using the corresponding factory
        private readonly ITelemetrySessionManager serverSessionManager = SessionManagerFactory.GetServerSessionManager();
        private readonly TelemetryPersistence persistence = PersistenceFactory.GetTelemetryPersistenceInstance();
        private readonly int thresholdTime = 30;


        //defining the variables to store the telemteric data 
        public Dictionary<DateTime, int> userCountVsEachTimeStamp = new Dictionary<DateTime, int>();
        public Dictionary<UserData,DateTime> eachUserEnterTimeInMeeting  = new Dictionary<UserData,DateTime>();
        public Dictionary<UserData, DateTime> eachUserExitTime = new Dictionary<UserData, DateTime>();
        public Dictionary<int, int> userIdVsChatCount = new Dictionary<int, int>();
        public List<int> listOfInSincereMembers = new List<int>();

        private DateTime sessionStartTime;



        //adding dictionary to store the userid and email id 
        public Dictionary<int, string> userIdVsEmailId = new Dictionary<int, string>();

        //Dictionary to store the emailidvsusername 
        public Dictionary<string, string> emailIdVsUserName = new Dictionary<string, string>();

        //Dictionary to store the value of the username vs chat count
        Dictionary<string, int> userNameVsChatCount = new Dictionary<string, int>();


        //it will store the recent entry time of the user 
        Dictionary<string, DateTime> listOfCurrUserWithEntryTime = new Dictionary<string, DateTime>();


        //this will store the time for which the user was present throughout the session 
        Dictionary<string, int> eachUserMeetingDurationTime = new Dictionary<string, int>();


        //constructor for telemetry module 
        
        public Telemetry()
        {
            sessionStartTime = DateTime.Now;
            //we have to subscribe to the ITelemetryNotifications 
            serverSessionManager.Subscribe(this);
            
        }

        //function to find the username vs chat count to show on the UX 
        public void UpdateUserNameVsChatCount()
        {
            userNameVsChatCount.Clear();
            //using the for loop for this purpose 
            foreach (var currUserChatCount in userIdVsChatCount)
            {
                //we have to update the value of the chat count correspoding to the username for this purpose 
                string currEmailId = userIdVsEmailId[currUserChatCount.Key];
                string currUserName = emailIdVsUserName[currEmailId];
                userNameVsChatCount[currUserName] = currUserChatCount.Value;
            
            }

            //say everything went fine 
            return;
        }

        //function to fetch the telemetry analytics and then give it back to the session manager 
        public SessionAnalytics GetTelemetryAnalytics(PlexShareContent.DataModels.ChatThread[] allChatMessages)
        {
            DateTime currTime = DateTime.Now;
            GetUserIdVsChatCount(allChatMessages);

            //Calling the function to update the username vs chatcount value for this purpose 
            UpdateUserNameVsChatCount();


            GetListOfInsincereMembers(currTime);

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
            currSessionAnalytics.userNameVsChatCount = userNameVsChatCount;
            SessionSummary sessionSummary = new SessionSummary();
            sessionSummary.userCount = currTotalUser;
            sessionSummary.chatCount = currTotalChatCount;
            sessionSummary.score = currTotalChatCount * currTotalUser;
            
            currSessionAnalytics.sessionSummary = sessionSummary;

            return currSessionAnalytics;
        }



        //function fetch the details from the chatcontext and then giving it to persistent to save the analytics on the server 
        public void SaveAnalytics(PlexShareContent.DataModels.ChatThread[] allChatMessages)
        {
            DateTime currDateTime = DateTime.Now;
            GetUserIdVsChatCount(allChatMessages);

            //updating the username vs the chat count to be able to save in persistent for this purpose 
            UpdateUserNameVsChatCount();
            GetListOfInsincereMembers(currDateTime);
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
            finalSessionAnalyticsToSave.userNameVsChatCount = userNameVsChatCount;
            SessionSummary sessionSummary = new SessionSummary();
            sessionSummary.chatCount = currTotalChatCount;
            sessionSummary.userCount = currTotalUser;
            finalSessionAnalyticsToSave.sessionSummary = sessionSummary;


            //calling the persistent module to save these analytics 
            persistence.Save(finalSessionAnalyticsToSave);


            //say everything went fine 
            return;
        }

        public void GetUserIdVsChatCount(PlexShareContent.DataModels.ChatThread[] allMessages)
        {
            //we have to implement when we start integrating with the chat module 
            //we have to first clear this and then we have to again calculate the useridvschatcount 
            userIdVsChatCount.Clear();

            //using the for loop 
            foreach (var currThread in allMessages)
            {
                foreach (var currMessage in currThread.MessageList)
                {
                    //using the if else statement 
                    if (userIdVsChatCount.ContainsKey(currMessage.SenderID)) userIdVsChatCount[currMessage.SenderID]++;
                    else
                        userIdVsChatCount.Add(currMessage.SenderID, 1);
                }
            }


            //say everything went fine 
            return;
        }


        //function to calculate the insincere members when the meeting ends and the session manager tells to save the details and the insincere members list will only be calculated then only
        public void GetListOfInsincereMembers(DateTime currTime)
        {
            //clearing the list to recalculate the insincere members whenever the 
            listOfInSincereMembers.Clear();

            //we have to calculate the threshold time here to find the attentie and non attentive users for this purpose 

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



        public void UpdateUserIdVsEamilId(SessionData newSession)
        {
            //using the for loop for this purpose 
            foreach (var currUser in newSession.users)
            {
                //using the if else statement for this purpose 
                if (userIdVsEmailId.ContainsKey(currUser.userID) == false)
                {
                    //this means that this is new userid hence we have to store the email correspoding to this for this purpose 
                    userIdVsEmailId[currUser.userID] = currUser.userEmail;
                }
            }

            //say everything went fine 
            return;
        
        }

        public void UpdateEmailIdVsUserName(SessionData newSession)
        {
            foreach (var currUser in newSession.users)
            {
                //applying the if else statement whether this email id has already stored the username or not for this purpose 
                if (emailIdVsUserName.ContainsKey(currUser.userEmail) == false)
                {
                    //this email is not present in the dictionary hence we have to store this 
                    emailIdVsUserName[currUser.userEmail] = currUser.username;
                }
            }

            //say everything went fine 
            return;
        
        }



        //this function will be called whenever the session analytics will be changed at the server side session manager using publisher subscriber model 
        public void OnAnalyticsChanged(SessionData newSession)
        {
            var currTime = DateTime.Now;

            //we have to update the useidvsemail and emailvsusername 
            UpdateUserIdVsEamilId(newSession);
            UpdateEmailIdVsUserName(newSession);
            //we have to recalculate and  update the telemetric analytics
            CalculateUserCountVsTimeStamp(newSession, currTime);
            CalculateArrivalExitTimeOfUser(newSession, currTime);
            GetListOfInsincereMembers(currTime);

            return;

        }

        //function defined for testing purpose so that we can overload the function 
        public void OnAnalyticsChanged(SessionData newSession, DateTime currTime)
        {
            //var currTime = DateTime.Now;
            //we have to recalculate and  update the telemetric analytics
            CalculateUserCountVsTimeStamp(newSession, currTime);
            CalculateArrivalExitTimeOfUser(newSession, currTime);
            GetListOfInsincereMembers(currTime);

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
                //if the user is not present in the entertimemeeting dictionary then this is joining again 
                if (eachUserEnterTimeInMeeting.ContainsKey(currUser) == false)
                    eachUserEnterTimeInMeeting[currUser] = currTime;



            }
            //checking for the left users 
            foreach (var currUser in eachUserEnterTimeInMeeting)
            {
                if (newSession.users.Contains(currUser.Key) == false && eachUserExitTime.ContainsKey(currUser.Key) == false)
                    eachUserExitTime[currUser.Key] = currTime;

            }

            //say everything went fine 
            return;
        }

        //public void CalculateArrivalExitTimeOfUser(SessionData newSession, DateTime currTime)
        //{
        //    //using the for loop for this purpose 
        //    foreach (var currUser in newSession.users)
        //    {
        //        //if new user comes 
        //        if (listOfCurrUserWithEntryTime.ContainsKey(currUser.userEmail) == false)
        //        {
        //            //then we have to update the entry time in this
        //            listOfCurrUserWithEntryTime[currUser.userEmail] = currTime;

        //            //if there is no entry has been made in the duration dictionary then add this user with duration time 0 
        //            if (eachUserMeetingDurationTime.ContainsKey(currUser.userEmail) == false)
        //            {
        //                eachUserMeetingDurationTime[currUser.userEmail] = 0;

        //            }
        //            else
        //            { 
        //                //if entry is already there then we do not need to do anything 
        //            }
        //        }
        //    }

        //}



        //function to get the useridvschatcount 
        //public 
        //ading some comments 
        //adding some commetnsadfadsj
    }
}
