using Client.Models;
//using LiveCharts;
//using LiveCharts.Defaults;
//using PlexShareDashboard.Dashboard.UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Markup;
using PlexShareDashboard.Dashboard.UI.Models;
using PlexShareDashboard.Dashboard.Server.Telemetry;
using PlexShareDashboard.Dashboard;
using PlexShare.Dashboard;
using PlexShareDashboard.Dashboard.Client.SessionManagement;
using PlexShare.Dashboard.Client.SessionManagement;
using Dashboard;

namespace PlexShareDashboard.Dashboard.UI.ViewModel
{
    public class DashboardViewModel : INotifyPropertyChanged, IClientSessionNotifications
    {
        //defining the variables for the client session manager 
        //SessionData stores the values of the session data which consists of the list of users and some important function
        //and sessionAnlaytics stores the telemetric data that we will get from the getAnalytics function 
        private  ClientSessionManager clientSessionManager;
        private  SessionData sessionData;
        private  SessionAnalytics sessionAnalytics;



        //this is the view model for the dashboard in this we will be fetaching the details from the models and then storing it in the viewmodel and then we will be binding to the view of the application 
        //ObservableCollection  for storing the list of pariticipants and their status of screensharing
        public ObservableCollection<User> participantsList { get; set; }
      

       //ObservableCollection for storing usercount at every time stamp 
        public ObservableCollection<UserCountVsTimeStamp> userCountVsTimeStamps { get; set; }

        //ObservableCollection for storing the number of chat count for each user 
        public ObservableCollection<UserIdVsChatCount> userIdVsChatCounts { get; set; }

        //storing the attentive and non attentive users in the meeting 
        private int attentiveUsers { get; set; }
        private int nonAttentiveUsers { get; set; }

        private int totalMessageCount { get; set; }

        private int totalParticipantsCount { get; set; }

        private string engagementRate { get; set; }

        private int sessionScore { get; set; }

        private string sessionMode { get; set; }


        /// <summary>
        /// Total number of messages sent in chat during the session
        /// </summary>
        public int TotalMessageCount
        {
            get { return this.totalMessageCount; }
            set
            {
                if (this.totalMessageCount != value)
                {
                    this.totalMessageCount = value;
                    OnPropertyChanged("TotalMessageCount");
                }
            }
        }

        public int TotalParticipantsCount
        {
            get { return totalParticipantsCount; }
            set
            {
                if (totalParticipantsCount != value)
                {
                    totalParticipantsCount = value;
                    OnPropertyChanged("TotalParticipantsCount");
                }
            }
        }


        public string EngagementRate
        {
            get { return engagementRate; }
            set
            {
                if (engagementRate != value)
                {
                    engagementRate = value;
                    OnPropertyChanged("EngagementRate");
                }
            }
        }

        public int AttentiveUsers
        {
            get { return attentiveUsers; }
            set
            {
                if (attentiveUsers != value)
                {
                    attentiveUsers = value;
                    OnPropertyChanged("AttentiveUsers");
                }
            }
        }

        public int NonAttentiveUsers
        {
            get { return nonAttentiveUsers; }
            set
            {
                if (nonAttentiveUsers != value)
                {
                    nonAttentiveUsers = value;
                    OnPropertyChanged("NonAttentiveUsers");
                }
            }
        }

        public int SessionScore
        {
            get { return sessionScore; }
            set
            {
                if (sessionScore != value)
                {
                    sessionScore = value;
                    OnPropertyChanged("SessionScore");
                }
            }
        }


        public string SessionMode
        {
            get { return sessionMode; }
            set
            {
                if (sessionMode != value)
                {
                    sessionMode = value;
                    OnPropertyChanged("SessionMode");
                }
            }
        }
        //constructor for view model 
        public DashboardViewModel()
        {
            
            
            //initialising participantsList 
            participantsList = new ObservableCollection<User>();
            User user1 = new User(1, "Rupesh Kumar", "Presenting");
            User user2 = new User(2, "Shubham Raj", "Presenting");
            User user3 = new User(3, "Hrishi Raaj", "Presenting");
            User user4 = new User(4, "Saurabh kumar", "Not Presenting");
            User user5 = new User(5, "Aditya Agarwal", "Not Presenting");
            participantsList.Add(user1);
            participantsList.Add(user2);
            participantsList.Add(user3);
            participantsList.Add(user4);
            participantsList.Add(user5);



            //initialising userCountVsTimeStamps
            userCountVsTimeStamps = new ObservableCollection<UserCountVsTimeStamp>();
            userCountVsTimeStamps.Add(new UserCountVsTimeStamp(10, new DateTime(2019, 05, 09, 9, 20, 0)));
            userCountVsTimeStamps.Add(new UserCountVsTimeStamp(20, new DateTime(2019, 05, 09, 9, 25, 0)));
            userCountVsTimeStamps.Add(new UserCountVsTimeStamp(30, new DateTime(2019, 05, 09, 9, 35, 0)));
            userCountVsTimeStamps.Add(new UserCountVsTimeStamp(40, new DateTime(2019, 05, 09, 9, 45, 0)));




            //initialising the uservschatcount collection 
            userIdVsChatCounts = new ObservableCollection<UserIdVsChatCount>();

            userIdVsChatCounts.Add(new UserIdVsChatCount(1, 10));
            userIdVsChatCounts.Add(new UserIdVsChatCount(2, 12));
            userIdVsChatCounts.Add(new UserIdVsChatCount(3, 13));
            userIdVsChatCounts.Add(new UserIdVsChatCount(4, 4));



            AttentiveUsers = 60;
            NonAttentiveUsers = 100 - AttentiveUsers;

            TotalParticipantsCount = 140;
            TotalMessageCount = 104;
            EngagementRate = "94.2";
            TotalParticipantsCount = 200;
            SessionMode = "LabMode";



            //############################################################################################

            clientSessionManager = SessionManagerFactory.GetClientSessionManager();
            //we also have to subscribe to the IClientSessionNotifications if any session data changes 
            clientSessionManager.SubscribeSession(this);

            ////defining the sessionanalytics to store the information about the sessionanalytics 
            sessionAnalytics = new SessionAnalytics();
            sessionAnalytics.chatCountForEachUser = new Dictionary<int, int>();
            sessionAnalytics.listOfInSincereMembers = new List<int>();
            sessionAnalytics.userCountVsTimeStamp = new Dictionary<DateTime, int>();
            sessionAnalytics.sessionSummary = new SessionSummary();

            ////this function will be called whenever the summary and the telemetry data wil be ready 
            //clientSessionManager.SummaryCreated += (latestSummary) => OnSummaryChanged(latestSummary);
            clientSessionManager.AnalyticsCreated += (latestAnalytics) => OnAnalyticsChanged(latestAnalytics);

        }

        //function to update the viewModel whenever required 
        public void UpdateDashboardViewModel()
        {
            //########################################################
            //we have to fetech the analytics 
            //clientSessionManager.GetAnalytics();
            //########################################################

            //Convert the sessionAnalytics properly to show to the graph 

            //userCountVsTimeStamps.Clear();
            userCountVsTimeStamps.Add(new UserCountVsTimeStamp(50,new DateTime(2019, 05, 09, 9, 15, 0)));

            //userIdVsChatCounts.Clear();

            userIdVsChatCounts.Add(new UserIdVsChatCount(5, 16 ));



            //once got the sessionAnalytics 
            //update the value of all the observable collections.

            //update the total message count

            //update total paritcipant count 
            TotalParticipantsCount = 201;
            TotalMessageCount = 120;
            EngagementRate = "95%";
            NonAttentiveUsers = 50;
            AttentiveUsers = 50;
            //TotalParticipantsCount = 200;

            //update the engagement rate 

            //update attentive and non attentive users 


            return;
    
        }




        //function to update the participantsList of viewmodel 
        public void UpdateParticipantsList(List<UserData> users)
        {
            participantsList.Clear();

            //using the for loop to push the updated list of the users into participants list 
            foreach (var currUser in users)
            {
                int currUserId = currUser.userID;
                string currUserName = currUser.username;
                string currUserStatus = "Presenting";
                User newUser = new User(currUserId, currUserName, currUserStatus);

                participantsList.Add(newUser);
            
            }

            return;
        }


        //function to update usercountvstimestamp observable collection to update the view 
        public void UpdateUserCountVsTimeStamp(Dictionary<DateTime, int> currUserCountVsTimeStamp)
        { 
            //we have to update the observable collection userCountVsTimeStamp 
            userCountVsTimeStamps.Clear();

            //using the for loop for this purpose 
            foreach (var currElement in currUserCountVsTimeStamp)
            {
                int currUserCount = currElement.Value;
                DateTime currTimeStamp = currElement.Key;

                UserCountVsTimeStamp newUserCountVsTimeStampElement = new UserCountVsTimeStamp(currUserCount, currTimeStamp);

                userCountVsTimeStamps.Add(newUserCountVsTimeStampElement);
            
            }

            //say everything went fine 
            return;

        
        }

        //function to update the useridvschatcounts 
        public void UpdateUserIdVsChatCount(Dictionary<int, int> chatCountForEachUser)
        {
            userIdVsChatCounts.Clear();
            int chatCount = 0;

            //using the for loop for this purpose 
            foreach(var currUserChatCount in chatCountForEachUser)
            {
                var currUserid = currUserChatCount.Key;
                var currChatCount = currUserChatCount.Value;
                
                UserIdVsChatCount currUserIdChatCount = new UserIdVsChatCount(currUserid, currChatCount);

                userIdVsChatCounts.Add(currUserIdChatCount);

                chatCount = chatCount + currChatCount;
            }


            //updating the total chatcount 
            TotalMessageCount = chatCount;


            //say everything went fine 
            return;
        
        }


        //function to calculate the number of attentive and non attentive users in the meeting 
        public void CalculatePercentageOfAttentiveAndNonAttentiveUsers(int currNonAttentiveUsers, int currAttentiveUsers)
        {
            int nonAttentivePercentage = (currNonAttentiveUsers) / TotalParticipantsCount * 100;
            int attentivePercentage = 100 - nonAttentivePercentage;

            //updating the percentages
            NonAttentiveUsers = nonAttentivePercentage;
            AttentiveUsers = attentivePercentage;

            //say everything went fine 
            return;
        }


       


//#####################################################################################

        //function to listen to any of the session data changed subscribed to the IClientSessionNotifications
        public void OnClientSessionChanged(SessionData newSessionData)
        {
            
            if (newSessionData != null)
            {
                //we have to update the participants list and sessionMode
                UpdateParticipantsList(newSessionData.users);
                SessionMode = newSessionData.sessionMode;
                TotalParticipantsCount = participantsList.Count;

            }


            return;
        }



        //##################################################################################
        //implementing the onanalytics changed
        public void OnAnalyticsChanged(SessionAnalytics latestAnalytics)
        {
            //update the analytics of this viewModel
            sessionAnalytics = latestAnalytics;
            var chatCount = sessionAnalytics.sessionSummary.chatCount;
            var userCount = sessionAnalytics.sessionSummary.userCount;
            sessionAnalytics.sessionSummary.score = chatCount * userCount;
            var currScore = sessionAnalytics.sessionSummary.score;


            SessionScore = currScore;

            //we have to update all the lists so that we can show to the dahsboard
            UpdateUserCountVsTimeStamp(sessionAnalytics.userCountVsTimeStamp);
            UpdateUserIdVsChatCount(sessionAnalytics.chatCountForEachUser);
            CalculateEngagementRate(sessionAnalytics.chatCountForEachUser);


            int currNonAttentiveUsers = sessionAnalytics.listOfInSincereMembers.Count;
            int currAttentiveUsers = TotalParticipantsCount - currNonAttentiveUsers;

            CalculatePercentageOfAttentiveAndNonAttentiveUsers(currNonAttentiveUsers, currAttentiveUsers);
            
            //say everything went fine 
            return;
        }


        //############################################################################## 
        //Function to calculate the engagement rate 
        public void CalculateEngagementRate(Dictionary<int, int> currChatCountForEachUser)
        {
            int activeMembers = currChatCountForEachUser.Count;

            float engagementRate = (float)(activeMembers / TotalParticipantsCount) * 100;
            EngagementRate =  engagementRate.ToString("0") + "%";
            

            //say everything went fine 
            return;
        }






        //public event PropertyChangedEventHandler? PropertyChanged;
        //the following function notifies the view whenever the property changes on the viewmodel 
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

       
        

        public event PropertyChangedEventHandler PropertyChanged;

    }


}
