/// <author>Rupesh Kumar</author>
/// <summary>
/// This is dashboard View Model where all the logic resides and it connects the models and the view  
/// </summary>

using Client.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Windows;
using System.Data.Entity.Core.Metadata.Edm;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Net;
using System.IO;

namespace PlexShareDashboard.Dashboard.UI.ViewModel
{
    public class DashboardViewModel : INotifyPropertyChanged, IClientSessionNotifications
    {
       
        /// <summary>
        /// Defining the private attributes.
        /// 1. Client Session Manager = this will store the instance of clientSessionManager
        /// 2. sessionData = this is to store the sessionData value from sessionManager
        /// 3. sessionAnalytics = this stores the analytics of the session received from session manager.
        /// </summary>
        private ClientSessionManager clientSessionManager;
        private SessionData sessionData;
        private SessionAnalytics sessionAnalytics;



        
        /// <summary>
        /// Defining the observable collections and chartvalues. The names are self explanatory
        /// </summary>
        public ObservableCollection<User> ParticipantsList { get; set; }
        public ChartValues<int> UserCountList { get; set; }
        public ObservableCollection<string> TimeStampsList { get; set; }
        public ChartValues<int> ChatCountListForUserId { get; set; }
        public ChartValues<int> ChatCountListForUserName { get; set; }
        public ObservableCollection<string> UserIdList { get; set; }
        public ObservableCollection<string> UserNameList { get; set; }


        /// <summary>
        ///     defining the private attributes some of the values 
        /// </summary>
        private int AttentiveUsers { get; set; }
        private int NonAttentiveUsers { get; set; }
        private int TotalMessageCount { get; set; }
        private int TotalParticipantsCount { get; set; }
        private int MaxTotalParticipantsCount { get; set; }
        private string EngagementRate { get; set; }
        private int SessionScore { get; set; }
        private string SessionMode { get; set; }
        private string ButtonContent { get; set; }
        private string LeaveButtonContent { get; set; }
        private string SummaryContent { get; set; }
       



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


        /// <summary>
        /// Total number of participants in the session
        /// </summary>
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

        /// <summary>
        /// Max number of participants during the session 
        /// </summary>
        public int MaxTotalParticipantsCountSetter
        {
            get { return MaxTotalParticipantsCount; }
            set
            {
                if (MaxTotalParticipantsCount != value)
                {
                    MaxTotalParticipantsCount = value;
                    OnPropertyChanged("MaxTotalParticipantsCountSetter");
                }
            }
        }

        /// <summary>
        /// Stores the engagement rate  during the session 
        /// </summary>
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

        /// <summary>
        /// Stores the Attentive users   during the session 
        /// </summary>
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


        /// <summary>
        /// Stores the non attentive users during the session 
        /// </summary>
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


        /// <summary>
        /// Stores the session score of  the session 
        /// </summary>
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



        /// <summary>
        /// Stores the session mode of  the session 
        /// </summary>
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


        /// <summary>
        /// Stores the value of switch mode button value the session 
        /// </summary>
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


        /// <summary>
        /// Stores the text to show on the leave button  during the session 
        /// </summary>
        public string LeaveButtonContentSetter
        {
            get { return LeaveButtonContent; }
            set
            {
                if (LeaveButtonContent != value)
                {
                    LeaveButtonContent = value;
                    OnPropertyChanged("LeaveButtonContentSetter");
                }
            }
        }



        /// <summary>
        /// Stores the summary of the session 
        /// </summary>
        public string SummaryContentSetter
        {
            get { return SummaryContent; }
            set
            {
                if (SummaryContent != value)
                {
                    SummaryContent = value;
                    OnPropertyChanged("SummaryContentSetter");
                }
            }
        }


        /// <summary>
        /// this is constructor of dashboard view model 
        /// </summary>
        public DashboardViewModel()
        {
           

            sessionData = new SessionData();

            //initialising the observable collection and chartvalues
            ParticipantsList = new ObservableCollection<User>();
            UserCountList = new ChartValues<int>();
            TimeStampsList = new ObservableCollection<string>();
            ChatCountListForUserId = new ChartValues<int>();
            ChatCountListForUserName = new ChartValues<int>();
            UserIdList = new ObservableCollection<string>();
            UserNameList = new ObservableCollection<string>();
         

            //initialising the private attributes 
            AttentiveUsersSetter = 100;
            NonAttentiveUsersSetter = 0;
            TotalParticipantsCountSetter = 1;
            MaxTotalParticipantsCountSetter = 1;
            TotalMessageCountSetter = 0;
            EngagementRateSetter = "0";
            SessionModeSetter = "LabMode";
            SessionScoreSetter = 0;
            ButtonContentSetter = "Switch Mode";
            SummaryContentSetter = "Refresh To get the updated summary";

            
            //getting the instane of the client session manager 
            clientSessionManager = SessionManagerFactory.GetClientSessionManager();

            //subscribing to the session manager to get the updated session data whenever the session data changes 
            clientSessionManager.SubscribeSession(this);



            ////defining the sessionanalytics to store the information about the sessionanalytics 
            sessionAnalytics = new SessionAnalytics();
            sessionAnalytics.chatCountForEachUser = new Dictionary<int, int>();
            sessionAnalytics.listOfInSincereMembers = new List<int>();
            sessionAnalytics.userCountVsTimeStamp = new Dictionary<DateTime, int>();
            sessionAnalytics.sessionSummary = new SessionSummary();

            //these functions will be called whenever these will be invoked by the client session manager 
            clientSessionManager.AnalyticsCreated += (latestAnalytics) => OnAnalyticsChanged(latestAnalytics);
            clientSessionManager.SummaryCreated += (latestSummary) => OnSummaryChanged(latestSummary);



            Trace.WriteLine("[Dashboard ViewModel]Initializing the Dashboard ViewModel");
        }




        /// <summary>
        ///     Function to show the end meet to faculty and leave meet to user
        /// </summary>
        public void SetLeaveButtonAccordingToUser()
        {
            UserData currUser =  clientSessionManager.GetUser();

            if (currUser.userID == 1)
            {
                //this is host 
                LeaveButtonContentSetter = "End Meet";
                
            }
            else
            {
                //then this is normal user hence we have to show the leave meeting 
                LeaveButtonContentSetter = "Leave Meet";
                

            }

            //say everything went fine 
            return;
        
        }


        /// <summary>
        ///     Whenever the user clicks the refresh option this UpdateDashboardViewModel will be called 
        /// </summary>
        public void UpdateDashboardViewModel()
        {
          


            //calling the function to get the analytics 
            clientSessionManager.GetAnalytics();
            //calling the function to get the summary for this purpose 
            clientSessionManager.GetSummary();




            DateTime currDateTime = DateTime.Now;
            string currHour = currDateTime.ToString("HH");
            string currSecond = currDateTime.ToString("mm");

            string finalTimeStamp = currHour + ":" + currSecond;
            TimeStampsList.Add(finalTimeStamp);


            Trace.WriteLine("[Dashboard ViewModel] User has clicked the refresh button. Updating the dashboard view model");

            return;

        }


        /// <summary>
        ///     function to get the client session manager 
        /// </summary>
        public ClientSessionManager GetClientSessionManager()
        {
            return clientSessionManager;
        }

        /// <summary>
        ///     function to get the session analytics 
        /// </summary>
        public SessionAnalytics GetSessionAnalytics()
        {
            return sessionAnalytics;
        }


        /// <summary>
        ///     Function to get the session Data 
        /// </summary>
        public SessionData GetSessionData()
        {
            return sessionData;
        
        }



        /// <summary>
        ///     function to update the button content according to the current mode of the session 
        /// </summary>
        public void UpdateButtonContent()
        {
            
            if (SessionModeSetter == "LabMode")
            {
                ButtonContentSetter = "Switch To ExamMode";

            }
            else
            {
                ButtonContentSetter = "Switch To LabMode";
            }

            

            //say everything went fine 
            return;

        }


        /// <summary>
        ///     function to change the session mode 
        /// </summary>
        
        public void SwitchSessionMode()
        {

            //getting the current user because this action is only allowed for the faculty 
            UserData currUser = clientSessionManager.GetUser();


            if (currUser.userID == 1)
            {
                //calling the toggle function to toggle the session for this particular meeting
                clientSessionManager.ToggleSessionMode();
                Trace.WriteLine("[Dashboard ViewModel] The faculty has changed the session Mode of the meeting");

            }
            else
            {
                //show alert here for this purpose on the UX 
            }

            //say everything went fine 
            return;

        }


        /// <summary>
        ///function to initiate leavemeeting procedure      
        /// </summary>
        public void LeaveMeetingProcedure()
        {
            //getting the current user as LeaveMeetingProcedure is only allowed for the server 
            UserData currUser = clientSessionManager.GetUser();

            if (currUser.userID == 1)
            {
                //this user is host hence it will end the meet  
                //calling the end meet procedure 
                clientSessionManager.EndMeet();
                Trace.WriteLine("[Dashboard ViewModel] The Faculty has ended the meeting");
            }
            else
            {
                //the user will be just removed by session manager 
                //this is normal user hence we have to call the remove client 
                clientSessionManager.RemoveClient();
                Trace.WriteLine("[Dashboard ViewModel] The User has left the meeting");

            }

        }


        /// <summary>
                //function to update the ParticipantsList of viewmodel 
        /// </summary>
        public void UpdateParticipantsList(List<UserData> users)
        {
            //writing the code to update the observable collection for the participant list 
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                ParticipantsList.Clear();
            });
            

            //first we have to insert the instructor 
            //the instructor should be at the top 
            //using the for loop for this purpose 
            foreach (var currUser in users)
            {
                int currUserId = currUser.userID;
                if (currUserId == 1)
                {
                    //int currUserId = currUser.userID;
                    string currUserName = currUser.username + "  (Instructor)";
                    string currUserStatus = "Presenting";
                    string currProfilePath = currUser.userPhotoUrl;


                    User newUser = new User(currUserId, currUserName, currUserStatus, currProfilePath);


                    Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        ParticipantsList.Add(newUser);
                    });
                    break;
                }
            }



            //using the for loop to push the updated list of the users into participants list 
            foreach (var currUser in users)
            {
                int currUserId = currUser.userID;
                if (currUserId == 1)
                {
                    continue;
                }
                string currUserName = currUser.username;
                string currUserStatus = "Presenting";
                string currProfilePath = currUser.userPhotoUrl;
                

                User newUser = new User(currUserId, currUserName, currUserStatus, currProfilePath);
                

                Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    ParticipantsList.Add(newUser);
                });

            }

            return;
        }


        /// <summary>
        ///     function to fetch the hour and the minute in the proper  format 
        /// </summary>
        public string GetHourAndMinute(DateTime currDateTime)
        {
            string currHour = currDateTime.ToString("HH");
            string currSecond = currDateTime.ToString("mm");
            string finalTimeStamp = currHour + ":" + currSecond;

            //say everything went fine 
            return finalTimeStamp;

        }


        /// <summary>
        ///function to update usercountvstimestamp observable collection to update the view 
        /// </summary>
        public void UpdateUserCountVsTimeStamp(Dictionary<DateTime, int> currUserCountVsTimeStamp)
        {
            //we have to clear the userscountList 
            Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                UserCountList.Clear();
                TimeStampsList.Clear();
            });


            //using the for loop for this purpose 
            foreach (var currElement in currUserCountVsTimeStamp)
            {
                int currUserCount = currElement.Value;
                DateTime currTimeStamp = currElement.Key;
                string finalTimeStampToShow = GetHourAndMinute(currTimeStamp);

                Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    //adding this new users count into the usercountlist 
                    UserCountList.Add(currUserCount);

                    //adding the new entry to the timestamp 
                    TimeStampsList.Add(finalTimeStampToShow);
                });


            }

            //say everything went fine 
            return;


        }


        /// <summary>
        ///function to update the useridvschatcounts 
        /// </summary>
        public void UpdateUserIdVsChatCount(Dictionary<int, int> chatCountForEachUser)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                //we have to clear the array of the userid list and 
                UserIdList.Clear();
                ChatCountListForUserId.Clear();
            });



            int chatCount = 0;

            //using the for loop for this purpose 
            foreach (var currUserChatCount in chatCountForEachUser)
            {
                var currUserid = currUserChatCount.Key;
                var currChatCount = currUserChatCount.Value;

                Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    //we have to add  the new element into the chart values 
                    UserIdList.Add(currUserid.ToString());
                    ChatCountListForUserId.Add(currChatCount);
                });


                chatCount = chatCount + currChatCount;
            }


            //updating the total chatcount 
            TotalMessageCountSetter = chatCount;


            //say everything went fine 
            return;

        }

        /// <summary>
        ///function to calculate the number of attentive and non attentive users in the meeting     
        /// </summary>
        public void CalculatePercentageOfAttentiveAndNonAttentiveUsers(int currNonAttentiveUsers, int currAttentiveUsers)
        {
            int nonAttentivePercentage = ((currNonAttentiveUsers)*100) / TotalParticipantsCountSetter;
            int attentivePercentage = 100 - nonAttentivePercentage;

            //updating the percentages
            NonAttentiveUsersSetter = nonAttentivePercentage;
            AttentiveUsersSetter = attentivePercentage;

            //say everything went fine 
            return;
        }


        /// <summary>
        ///     function to calculate the max participants count till now in session
        /// </summary>
        public void SetMaxTotalParticipantsCount()
        {
            if (TotalParticipantsCountSetter > MaxTotalParticipantsCountSetter)
            {
                MaxTotalParticipantsCountSetter = TotalParticipantsCountSetter;
            }
        }


        /// <summary>
        ///function to listen to any of the session data changed subscribed to the IClientSessionNotifications
        /// </summary>
        public void OnClientSessionChanged(SessionData newSessionData)
        {

            if (newSessionData != null)
            {
                //we have to update the participants list and SessionMode
                UpdateParticipantsList(newSessionData.users);
                UserData currUser = clientSessionManager.GetUser();

                SetLeaveButtonAccordingToUser();
                SessionModeSetter = newSessionData.sessionMode;
                UpdateButtonContent();

                TotalParticipantsCountSetter = ParticipantsList.Count;
                SetMaxTotalParticipantsCount();

            }

            Trace.WriteLine("[Dashboard ViewModel] Got the Updated session data.");


            return;
        }

        /// <summary>
        ///overloading function to test the onclient session changed. 
        /// </summary>
        public void OnClientSessionChanged(SessionData newSessionData, int testingGateway)
        {

            if (newSessionData != null)
            {
                //we have to update the participants list and SessionMode
                UpdateParticipantsList(newSessionData.users);
                UserData currUser = clientSessionManager.GetUser();

                SessionModeSetter = newSessionData.sessionMode;
                UpdateButtonContent();

                TotalParticipantsCountSetter = ParticipantsList.Count;
                SetMaxTotalParticipantsCount();

            }


            return;
        }


        /// <summary>
        ///defining the function to update the summary of the current session till now 
        /// </summary>
        public void OnSummaryChanged(string latestSummary)
        {
            if (latestSummary == null)
            {
                //say everything went fine 
                return;
            }

            //updating the summary for this session 
            SummaryContentSetter = latestSummary;
            Trace.WriteLine("[Dashboard ViewModel] Updated the summary of the session");
            //say everything went fine 
            return;
        }


        /// <summary>
        ///     function to update the dictionary storing username vs chatcount
        /// </summary>
        public void UpdateUserNameVsChatCount(Dictionary<string, int> currUserNameVsChatCount)
        {

           
            Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                //we have to clear the array of the userid list and 
                UserNameList.Clear();
                ChatCountListForUserName.Clear();
            });


            int chatCount = 0;


            //using the for loop to add the username vs userid 
            foreach (var currUserChatCount in currUserNameVsChatCount)
            {
                var currUserName = currUserChatCount.Key;
                var currChatCount = currUserChatCount.Value;

                Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    //we have to add  the new element into the chart values 
                    UserNameList.Add(currUserName);
                    ChatCountListForUserName.Add(currChatCount);
                });

                chatCount = chatCount + currChatCount;

            }

            TotalMessageCountSetter = chatCount;

            //say everything went fine 
            return;
        }


        /// <summary>
        ///implementing the onanalytics changed. This function will be called every time when the user clicks the onAnalytics changed once the analytics is ready from the telemtry module. This function will be called via session manager.
        /// </summary>
        
        public void OnAnalyticsChanged(SessionAnalytics latestAnalytics)
        {
            if (latestAnalytics == null)
            {
                //say everything went fine 
                return;
            }


            //update the analytics of this viewModel
            sessionAnalytics = latestAnalytics;
            var chatCount = sessionAnalytics.sessionSummary.chatCount;
            var userCount = sessionAnalytics.sessionSummary.userCount;
            sessionAnalytics.sessionSummary.score = chatCount * userCount;
            var currScore = sessionAnalytics.sessionSummary.score;


            //change the session score only then when the current session score is less 
            if (currScore > SessionScoreSetter)
            {
                SessionScoreSetter = currScore;

            }

            //we have to update all the lists so that we can show to the dahsboard
            UpdateUserCountVsTimeStamp(sessionAnalytics.userCountVsTimeStamp);
            //this is am calculating to be able to be used in future.
            UpdateUserIdVsChatCount(sessionAnalytics.chatCountForEachUser);

            //calculating the engagement rate 
            CalculateEngagementRate(sessionAnalytics.userNameVsChatCount);

            //calling the function to update and show the username vs chat count 
            UpdateUserNameVsChatCount(sessionAnalytics.userNameVsChatCount);

            int currNonAttentiveUsers = sessionAnalytics.listOfInSincereMembers.Count;
            int currAttentiveUsers = TotalParticipantsCountSetter - currNonAttentiveUsers;

            CalculatePercentageOfAttentiveAndNonAttentiveUsers(currNonAttentiveUsers, currAttentiveUsers);

            Trace.WriteLine("[Dashboard ViewModel] Updated the telemtric data on the view model.");


            //say everything went fine 
            return;
        }




        /// <summary>
        ///Function to calculate the engagement rate 
        /// </summary>
        public void CalculateEngagementRate(Dictionary<string, int> currUserNameVsChatCount)
        {
            int activeMembers = currUserNameVsChatCount.Count;

            float EngagementRate = (float)(activeMembers*100) / MaxTotalParticipantsCountSetter;
            EngagementRateSetter = EngagementRate.ToString("0") + "%";

            //say everything went fine 
            return;
        }


        /// <summary>
        ///the following function notifies the view whenever the property changes on the viewmodel 
        /// </summary>
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }




        public event PropertyChangedEventHandler PropertyChanged;

    }


}
