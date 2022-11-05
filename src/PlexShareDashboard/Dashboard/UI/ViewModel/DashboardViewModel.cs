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
using System.Diagnostics;
using LiveCharts.Wpf;
using LiveCharts;

namespace PlexShareDashboard.Dashboard.UI.ViewModel
{
    public class DashboardViewModel : INotifyPropertyChanged, IClientSessionNotifications
    {
        //defining the variables for the client session manager 
        //SessionData stores the values of the session data which consists of the list of users and some important function
        //and sessionAnlaytics stores the telemetric data that we will get from the getAnalytics function 
        private ClientSessionManager clientSessionManager;
        private SessionData sessionData;
        private SessionAnalytics sessionAnalytics;



        //this is the view model for the dashboard in this we will be fetaching the details from the models and then storing it in the viewmodel and then we will be binding to the view of the application 
        //ObservableCollection  for storing the list of pariticipants and their status of screensharing
        public ObservableCollection<User> ParticipantsList { get; set; }


        //ObservableCollection for storing usercount at every time stamp 
        //public ObservableCollection<UserCountVsTimeStamp> UserCountVsTimeStamps { get; set; }
        //public ObservableCollection<int> UserCountList { get; set; }
        public ChartValues<int> UserCountList { get; set; }
        public ObservableCollection<string> TimeStampsList { get; set; }

        //ObservableCollection for storing the number of chat count for each user 
        public ObservableCollection<UserIdVsChatCount> UserIdVsChatCounts { get; set; }
        public ChartValues<int> ChatCountList { get; set; }
        public ObservableCollection<string> UserIdList { get; set; }
        //debug.assert 
        //checkbills & free 
        //Trace  
        //storing the attentive and non attentive users in the meeting 
        private int AttentiveUsers { get; set; }
        private int NonAttentiveUsers { get; set; }

        private int TotalMessageCount { get; set; }

        private int TotalParticipantsCount { get; set; }

        private string EngagementRate { get; set; }

        private int SessionScore { get; set; }

        private string SessionMode { get; set; }

        //variable for storing the button content to be shown according to the user 
        private string ButtonContent { get; set; }


        /// <summary>
        /// Total number of messages sent in chat during the session
        /// </summary>
        public int TotalMessageCountSetter
        {
            get { return this.TotalMessageCount; }
            set
            {
                if (this.TotalMessageCount != value)
                {
                    this.TotalMessageCount = value;
                    OnPropertyChanged("TotalMessageCountSetter");
                }
            }
        }

        public int TotalParticipantsCountSetter
        {
            get { return TotalParticipantsCount; }
            set
            {
                if (TotalParticipantsCount != value)
                {
                    TotalParticipantsCount = value;
                    OnPropertyChanged("TotalParticipantsCountSetter");
                }
            }
        }


        public string EngagementRateSetter
        {
            get { return EngagementRate; }
            set
            {
                if (EngagementRate != value)
                {
                    EngagementRate = value;
                    OnPropertyChanged("EngagementRateSetter");
                }
            }
        }

        public int AttentiveUsersSetter
        {
            get { return AttentiveUsers; }
            set
            {
                if (AttentiveUsers != value)
                {
                    AttentiveUsers = value;
                    OnPropertyChanged("AttentiveUsersSetter");
                }
            }
        }

        public int NonAttentiveUsersSetter
        {
            get { return NonAttentiveUsers; }
            set
            {
                if (NonAttentiveUsers != value)
                {
                    NonAttentiveUsers = value;
                    OnPropertyChanged("NonAttentiveUsersSetter");
                }
            }
        }

        public int SessionScoreSetter
        {
            get { return SessionScore; }
            set
            {
                if (SessionScore != value)
                {
                    SessionScore = value;
                    OnPropertyChanged("SessionScoreSetter");
                }
            }
        }


        public string SessionModeSetter
        {
            get { return SessionMode; }
            set
            {
                if (SessionMode != value)
                {
                    SessionMode = value;
                    OnPropertyChanged("SessionModeSetter");
                }
            }
        }


        public string ButtonContentSetter
        {
            get { return ButtonContent; }
            set
            {
                if (ButtonContent != value)
                {
                    ButtonContent = value;
                    OnPropertyChanged("ButtonContentSetter");
                }
            }
        }


        //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
        //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
        //constructor for view model 
        public DashboardViewModel()
        {


            //initialising ParticipantsList 
            ParticipantsList = new ObservableCollection<User>();
            User user1 = new User(1, "Rupesh Kumar", "Presenting");
            User user2 = new User(2, "Shubham Raj", "Presenting");
            User user3 = new User(3, "Hrishi Raaj", "Presenting");
            User user4 = new User(4, "Saurabh kumar", "Not Presenting");
            User user5 = new User(5, "Aditya Agarwal", "Not Presenting");
            ParticipantsList.Add(user1);
            ParticipantsList.Add(user2);
            ParticipantsList.Add(user3);
            ParticipantsList.Add(user4);
            ParticipantsList.Add(user5);


            UserCountList = new ChartValues<int>();
            TimeStampsList = new ObservableCollection<string>();


            ////initialising UserCountVsTimeStamps
            //UserCountVsTimeStamps = new ObservableCollection<UserCountVsTimeStamp>();
            //UserCountVsTimeStamps.Add(new UserCountVsTimeStamp(10, 15));
            //UserCountVsTimeStamps.Add(new UserCountVsTimeStamp(20, 20));
            //UserCountVsTimeStamps.Add(new UserCountVsTimeStamp(30, 25));
            //UserCountVsTimeStamps.Add(new UserCountVsTimeStamp(40, 30));
            UserCountList.Add(10);
            UserCountList.Add(20);
            UserCountList.Add(30);
            UserCountList.Add(40);

            TimeStampsList.Add("15");
            TimeStampsList.Add("20");
            TimeStampsList.Add("25");
            TimeStampsList.Add("35");



            ChatCountList = new ChartValues<int>();
            UserIdList = new ObservableCollection<string>();

            //initialising the uservschatcount collection 
            UserIdVsChatCounts = new ObservableCollection<UserIdVsChatCount>();

            UserIdVsChatCounts.Add(new UserIdVsChatCount(1, 10));
            UserIdVsChatCounts.Add(new UserIdVsChatCount(2, 12));
            UserIdVsChatCounts.Add(new UserIdVsChatCount(3, 13));
            UserIdVsChatCounts.Add(new UserIdVsChatCount(4, 4));
            ChatCountList.Add(10);
            ChatCountList.Add(12);
            ChatCountList.Add(13);
            ChatCountList.Add(4);

            UserIdList.Add("1");
            UserIdList.Add("2");
            UserIdList.Add("3");
            UserIdList.Add("4");

            AttentiveUsersSetter = 60;
            NonAttentiveUsersSetter = 100 - AttentiveUsersSetter;

            TotalParticipantsCountSetter = 140;
            TotalMessageCountSetter = 104;
            EngagementRateSetter = "94.2";
            TotalParticipantsCountSetter = 200;
            SessionModeSetter = "LabMode";
            SessionScoreSetter = 40;

            clientSessionManager = SessionManagerFactory.GetClientSessionManager();
            //we also have to subscribe to the IClientSessionNotifications if any session data changes 
            clientSessionManager.SubscribeSession(this);

            Trace.WriteLine("Initializing the dashboard viewmodel");

            UserData currUser = clientSessionManager.GetUser();
            //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
            //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
            UserData currU = new UserData("Rupesh", 1);
            currUser = currU;
            //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
            //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
            UpdateButtonContent(currUser);

           



            //############################################################################################

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


            //TODO WHILE INTEGRATION 
            //clientSessionManager.GetAnalytics();




            //########################################################

            

            UserIdVsChatCounts.Add(new UserIdVsChatCount(5, 16));

            //adding the new stuff into the usercount list and the participant list 
            UserCountList.Add(50);
            //TimeStampsList.Add("35");
            //string currDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            DateTime currDateTime = DateTime.Now;
            string currHour = currDateTime.ToString("HH");
            string currSecond = currDateTime.ToString("mm");

            string finalTimeStamp = currHour  + ":" + currSecond;
            TimeStampsList.Add(finalTimeStamp);



            //once got the sessionAnalytics 
            //update the value of all the observable collections.

            //update the total message count

            //update total paritcipant count 
            TotalParticipantsCountSetter = 201;
            TotalMessageCountSetter = 120;
            EngagementRateSetter = "95%";
            NonAttentiveUsersSetter = 50;
            AttentiveUsersSetter = 50;
            SessionScoreSetter = 100;
            //TotalParticipantsCount = 200;

            //update the engagement rate 

            //update attentive and non attentive users 


            return;

        }


        //function to update the button content 
        private void UpdateButtonContent(UserData currUser)
        {
            //UserData currU = new UserData("Rupesh", 1);
            //currUser = currU;
            if (currUser == null)
            {
                ButtonContentSetter = "Meeting Not Started";
            }
            else
            {
                if (SessionModeSetter == "LabMode")
                {
                    ButtonContentSetter = "Switch To ExamMode";

                }
                else
                {
                    ButtonContentSetter = "Switch To LabMode";
                }
                //this is host hence we have to show the button content according to the host 
            }
           

            //say everything went fine 
            return;

        }

        //function to change the session mode 
        public void SwitchSessionMode()
        {
            UserData currUser = clientSessionManager.GetUser();


            //code for testing 
            //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
            //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
            //UserData currU = new UserData("Rupesh", 1);
            UserData currU = new UserData("Rupesh", 2);
            currUser = currU;
            //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
            //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$


            if (currUser.userID == 1)
            {
                //this user is host hence it can switch the mode 


                //TODO WHILE INTEGRATION
                //clientSessionManager.ToggleSessionMode();



                //code for testing purpose 
                //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
                //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
                if (SessionModeSetter == "LabMode")
                {
                    SessionModeSetter = "ExamMode";


                }
                else
                {
                    SessionModeSetter = "LabMode";
                }
                UpdateButtonContent(currUser);
                //UpdateButtonContent(currUser);

                //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
                //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$

                //buttonValue = "Switch Mode"
            }
            else
            {
                //buttonValue = SessionMode;
            }

            //say everything went fine 
            return;

        }

        //function to initiate leavemeeting procedure 
        public void LeaveMeetingProcedure()
        {
            UserData currUser = clientSessionManager.GetUser();


            //TODO during integration 



            //if (currUser.userID == 1)
            //{
            //    //this user is host hence it will end the meet  
            //    //clientSessionManager.EndMeet();
            //    //buttonValue = "Switch Mode"
            //}
            //else
            //{
            //    //buttonValue = SessionMode;
            //    //the user will be just removed by session manager 
            //    //clientSessionManager.RemoveClient();
            //}

        }



        //function to update the ParticipantsList of viewmodel 
        public void UpdateParticipantsList(List<UserData> users)
        {
            ParticipantsList.Clear();

            //using the for loop to push the updated list of the users into participants list 
            foreach (var currUser in users)
            {
                int currUserId = currUser.userID;
                string currUserName = currUser.username;
                string currUserStatus = "Presenting";
                User newUser = new User(currUserId, currUserName, currUserStatus);

                ParticipantsList.Add(newUser);

            }

            return;
        }


        //function to fetch the hour and the minute format 
        private string GetHourAndMinute(DateTime currDateTime)
        {
            string currHour = currDateTime.ToString("HH");
            string currSecond = currDateTime.ToString("mm");

            string finalTimeStamp = currHour + ":" + currSecond;
            //TimeStampsList.Add(finalTimeStamp);

            //say everything went fine 
            return finalTimeStamp;

        }


        //function to update usercountvstimestamp observable collection to update the view 
        public void UpdateUserCountVsTimeStamp(Dictionary<DateTime, int> currUserCountVsTimeStamp)
        {
            //we have to update the observable collection userCountVsTimeStamp 
            //UserCountVsTimeStamps.Clear();

            //we have to clear the userscountList 
            UserCountList.Clear();
            TimeStampsList.Clear();


            //using the for loop for this purpose 
            foreach (var currElement in currUserCountVsTimeStamp)
            {
                int currUserCount = currElement.Value;
                DateTime currTimeStamp = currElement.Key;
                string finalTimeStampToShow = GetHourAndMinute(currTimeStamp);
                //TODO to convert the date time into the minutes and then append
                //int currTimeStampInt = 20;
                //UserCountVsTimeStamp newUserCountVsTimeStampElement = new UserCountVsTimeStamp(currUserCount, finalTimeStampToShow);

                //UserCountVsTimeStamps.Add(newUserCountVsTimeStampElement);





                //adding this new users count into the usercountlist 
                UserCountList.Add(currUserCount);

                //adding the new entry to the timestamp 
                TimeStampsList.Add(finalTimeStampToShow);

            }

            //say everything went fine 
            return;


        }

        //function to update the useridvschatcounts 
        public void UpdateUserIdVsChatCount(Dictionary<int, int> chatCountForEachUser)
        {
            UserIdVsChatCounts.Clear();

            //we have to clear the array of the userid list and 
            UserIdList.Clear();
            ChatCountList.Clear();



            int chatCount = 0;

            //using the for loop for this purpose 
            foreach (var currUserChatCount in chatCountForEachUser)
            {
                var currUserid = currUserChatCount.Key;
                var currChatCount = currUserChatCount.Value;

                UserIdVsChatCount currUserIdChatCount = new UserIdVsChatCount(currUserid, currChatCount);

                UserIdVsChatCounts.Add(currUserIdChatCount);

                //we have to add  the new element into the chart values 
                UserIdList.Add(currUserid.ToString());
                ChatCountList.Add(currChatCount);

                chatCount = chatCount + currChatCount;
            }


            //updating the total chatcount 
            TotalMessageCountSetter = chatCount;


            //say everything went fine 
            return;

        }


        //function to calculate the number of attentive and non attentive users in the meeting 
        public void CalculatePercentageOfAttentiveAndNonAttentiveUsers(int currNonAttentiveUsers, int currAttentiveUsers)
        {
            int nonAttentivePercentage = (currNonAttentiveUsers) / TotalParticipantsCountSetter * 100;
            int attentivePercentage = 100 - nonAttentivePercentage;

            //updating the percentages
            NonAttentiveUsersSetter = nonAttentivePercentage;
            AttentiveUsersSetter = attentivePercentage;

            //say everything went fine 
            return;
        }





        //#####################################################################################

        //function to listen to any of the session data changed subscribed to the IClientSessionNotifications
        public void OnClientSessionChanged(SessionData newSessionData)
        {

            if (newSessionData != null)
            {
                //we have to update the participants list and SessionMode
                UpdateParticipantsList(newSessionData.users);
                UserData currUser = clientSessionManager.GetUser();
                SessionModeSetter = newSessionData.sessionMode;
                UpdateButtonContent(currUser);
                TotalParticipantsCountSetter = ParticipantsList.Count;

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


            SessionScoreSetter = currScore;

            //we have to update all the lists so that we can show to the dahsboard
            UpdateUserCountVsTimeStamp(sessionAnalytics.userCountVsTimeStamp);
            UpdateUserIdVsChatCount(sessionAnalytics.chatCountForEachUser);
            CalculateEngagementRate(sessionAnalytics.chatCountForEachUser);


            int currNonAttentiveUsers = sessionAnalytics.listOfInSincereMembers.Count;
            int currAttentiveUsers = TotalParticipantsCountSetter - currNonAttentiveUsers;

            CalculatePercentageOfAttentiveAndNonAttentiveUsers(currNonAttentiveUsers, currAttentiveUsers);

            //say everything went fine 
            return;
        }


        //############################################################################## 
        //Function to calculate the engagement rate 
        public void CalculateEngagementRate(Dictionary<int, int> currChatCountForEachUser)
        {
            int activeMembers = currChatCountForEachUser.Count;

            float EngagementRate = (float)(activeMembers / TotalParticipantsCountSetter) * 100;
            EngagementRateSetter = EngagementRate.ToString("0") + "%";


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
