/// <author>Rupesh Kumar</author>
/// <summary>
/// This file implements the logic to show the temetric data of the session. This is implementing the ITelemetry and ITelemetryNotifications.
/// </summary>


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
using Dashboard.Server.Persistence;
using Dashboard;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace PlexShareDashboard.Dashboard.Server.Telemetry
{
    public class Telemetry : ITelemetry, ITelemetryNotifications
    {
        //getting the sessionmanager and persistence instance using the factories
        private readonly ITelemetrySessionManager serverSessionManager = SessionManagerFactory.GetServerSessionManager();
        private readonly TelemetryPersistence persistence = PersistenceFactory.GetTelemetryPersistenceInstance();
        private readonly int thresholdTime = 30;




        //defining the dictionaries  to store the telemteric data 
        public Dictionary<DateTime, int> userCountVsEachTimeStamp = new Dictionary<DateTime, int>();

        //stores the each user entry time in the meeting 
        public Dictionary<UserData,DateTime> eachUserEnterTimeInMeeting  = new Dictionary<UserData,DateTime>();

        //stores each users exit time from the meeting 
        public Dictionary<UserData, DateTime> eachUserExitTime = new Dictionary<UserData, DateTime>();

        //stores chatcount for each userid 
        public Dictionary<int, int> userIdVsChatCount = new Dictionary<int, int>();

        //stores the list of insincere members 
        public List<int> listOfInSincereMembers = new List<int>();

        //stores the start time of session 
        private DateTime sessionStartTime;



        //adding dictionary to store the userid and email id 
        public Dictionary<int, string> userIdVsEmailId = new Dictionary<int, string>();

        //Dictionary to store the emailidvsusername 
        public Dictionary<string, string> emailIdVsUserName = new Dictionary<string, string>();

        //Dictionary to store the value of the username vs chat count
        Dictionary<string, int> userNameVsChatCount = new Dictionary<string, int>();


       


        //constructor for telemetry module 
        public Telemetry()
        {
            sessionStartTime = DateTime.Now;

            //we have to subscribe to the ITelemetryNotifications 
            serverSessionManager.Subscribe(this);
            
        }


        /// <summary>
        ///     function to find the username vs chat count to show on the UX.
        /// </summary>
        public void UpdateUserNameVsChatCount()
        {
            userNameVsChatCount.Clear();
            //using the for loop for this purpose 
            foreach (var currUserChatCount in userIdVsChatCount)
            {
                //we have to update the value of the chat count correspoding to the username for this purpose 
                string currEmailId = userIdVsEmailId[currUserChatCount.Key];
                string currUserName = emailIdVsUserName[currEmailId];
                if (userNameVsChatCount.ContainsKey(currUserName) == false)
                {
                    userNameVsChatCount[currUserName] = 0 + currUserChatCount.Value;

                }
                else
                { 
                    userNameVsChatCount[currUserName] = userNameVsChatCount[currUserName] + currUserChatCount.Value;
                
                }
            
            }

            //say everything went fine 
            return;
        }


        /// <summary>
        ///     function to fetch the telemetry analytics and then give it back to the session manager.This function will be called whenever the user refreshes the dashboard. This function will calculate the telemetry based on the current data.
        /// </summary>
         /// <params name="allMessages"> Array of ChatThread objects which contains information about messages of each thread </params>

        public SessionAnalytics GetTelemetryAnalytics(PlexShareContent.DataModels.ChatThread[] allChatMessages)
        {
            DateTime currTime = DateTime.Now;

            //updating the userid vs chat count given this all chat messages 
            GetUserIdVsChatCount(allChatMessages);

            //Calling the function to update the username vs chatcount value for this purpose 
            UpdateUserNameVsChatCount();

            //function to calculate the insincere members 
            GetListOfInsincereMembers(currTime);


            var currTotalChatCount = 0;
            var currTotalUser = 0;


            //using the for loop to find these values 
            foreach (var eachUser in userIdVsChatCount)
            {
                currTotalChatCount = currTotalChatCount + eachUser.Value;
                currTotalUser = currTotalUser + 1;
            }



            //Now we have to update the session Analytics and send this session analytics to the UX/session manager
            SessionAnalytics currSessionAnalytics = new SessionAnalytics();
            currSessionAnalytics.chatCountForEachUser = userIdVsChatCount;
            currSessionAnalytics.listOfInSincereMembers = listOfInSincereMembers;
            currSessionAnalytics.userCountVsTimeStamp = userCountVsEachTimeStamp;
            currSessionAnalytics.userNameVsChatCount = userNameVsChatCount;
            
            //calculating the session summary 
            SessionSummary sessionSummary = new SessionSummary();
            sessionSummary.userCount = currTotalUser;
            sessionSummary.chatCount = currTotalChatCount;
            sessionSummary.score = currTotalChatCount * currTotalUser;
            
            currSessionAnalytics.sessionSummary = sessionSummary;

            Trace.WriteLine("[Telemetry Submodule] Get Telemetry Analytics function called. Successfully send the updated the telemetric data to session Manager");
            //say everything went fine 
            return currSessionAnalytics;
        }


        /// <summary>
        ///     This function is called just before the session ends and this will call the persistence to save the final telemtric data in the server/faculty laptop
        /// </summary>
        /// <params name="allMessages"> Array of ChatThread objects which contains information about messages of each thread </params>

        public void SaveAnalytics(PlexShareContent.DataModels.ChatThread[] allChatMessages)
        {
            DateTime currDateTime = DateTime.Now;


            //calculating the anaytics by calling the functions below 
            GetUserIdVsChatCount(allChatMessages);
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


            //calculating the final session analytics just before the meeting 
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
            Trace.WriteLine("[Telemetry Submodule] SaveAnalytics function called to save the telemetric data by the persistence submodule.");

            //say everything went fine 
            return;
        }



        /// <summary>
        ///    this function calculates the userid vs chat count that that user has sent in the meeting 
        /// </summary>
        /// <params name="allMessages"> Array of ChatThread objects which contains information about messages of each thread </params>
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
                    //update the chat count using the senders id from chatthread 
                    if (userIdVsChatCount.ContainsKey(currMessage.SenderID)) userIdVsChatCount[currMessage.SenderID]++;
                    else
                        userIdVsChatCount.Add(currMessage.SenderID, 1);
                }
            }


            //say everything went fine 
            return;
        }



        /// <summary>
        ///     function to calculate the insincere members when the meeting ends and the session manager tells to save the details and the insincere members list will only be calculated then only
        /// </summary>
        /// <params name="currTime"> curr time at which this function called  </params>
        public void GetListOfInsincereMembers(DateTime currTime)
        {
            //clearing the list to recalculate the insincere members whenever the 
            listOfInSincereMembers.Clear();


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





        /// <summary>
        ///     this function updates the userid vs emailid dictionary every time when the session data changes 
        /// </summary>
        /// <params name="newSession"> new session data received from the session manager </params>
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


        /// <summary>
        ///     this function updates the emailid vs username dictionary every time when the session data changes 
        /// </summary>
        /// <params name="newSession"> new session data received from the session manager </params>
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



        /// <summary>
        ///     this function will be called whenever the session analytics will be changed at the server side session manager using publisher subscriber model 
        /// </summary>
        /// <params name="newSession"> new session data received from the session manager </params>
        public void OnAnalyticsChanged(SessionData newSession)
        {
            var currTime = DateTime.Now;

            //we have to update the telemetric analytics by calling the following functions 
            UpdateUserIdVsEamilId(newSession);
            UpdateEmailIdVsUserName(newSession);
            CalculateUserCountVsTimeStamp(newSession, currTime);
            CalculateArrivalExitTimeOfUser(newSession, currTime);
            GetListOfInsincereMembers(currTime);

            Trace.WriteLine("[Telemetry Submodule] OnAnalytics function get called, successfully updated the telemetric data on the server.");
            //say everything went fine 
            return;

        }


        /// <summary>
        ///function defined for testing purpose so that we can overload the function 
        /// </summary>
        /// <params name="newSession"> new session data received from the session manager </params>
        ///  /// <params name="currTime"> current time </params>
        public void OnAnalyticsChanged(SessionData newSession, DateTime currTime)
        {
            //var currTime = DateTime.Now;
            //we have to recalculate and  update the telemetric analytics
            CalculateUserCountVsTimeStamp(newSession, currTime);
            CalculateArrivalExitTimeOfUser(newSession, currTime);
            GetListOfInsincereMembers(currTime);

            return;

        }

        /// <summary>
        ///function to update calculateUserCountVsTimeStamp 
        /// </summary>
        /// <params name="newSession"> new session data received from the session manager </params>
        ///  /// <params name="currTime"> current time </params>
        public void CalculateUserCountVsTimeStamp(SessionData newSession, DateTime currTime)
        {
            userCountVsEachTimeStamp[currTime] = newSession.users.Count;

            //say everything went fine 
            return;
        
        }


        /// <summary>
        ///function to calculate the arrival and exit time of the users 
        /// </summary>
        /// <params name="newSession"> new session data received from the session manager </params>
        ///  /// <params name="currTime"> current time </params>

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

    }
}
