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
        //defining the variables for the client session manager 
        //SessionData stores the values of the session data which consists of the list of users and some important function
        //and sessionAnlaytics stores the telemetric data that we will get from the getAnalytics function 
        private ClientSessionManager clientSessionManager;
        private SessionData sessionData;
        private SessionAnalytics sessionAnalytics;



        //this is the view model for the dashboard in this we will be fetaching the details from the models and then storing it in the viewmodel and then we will be binding to the view of the application 
        //ObservableCollection  for storing the list of pariticipants and their status of screensharing
        public ObservableCollection<User> ParticipantsList { get; set; }


        public ChartValues<int> UserCountList { get; set; }
        public ObservableCollection<string> TimeStampsList { get; set; }

        public ChartValues<int> ChatCountListForUserId { get; set; }
        public ChartValues<int> ChatCountListForUserName { get; set; }

        public ObservableCollection<string> UserIdList { get; set; }

        public ObservableCollection<string> UserNameList { get; set; }

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
        public Bitmap ReceivedImage
        {
            get;  set;
        }

        public string DisplayedImage
        {
            get { return "./Assets/FillTool.png"; }
        }




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


        public DashboardViewModel()
        {
           

            sessionData = new SessionData();
            ParticipantsList = new ObservableCollection<User>();
        
            



            UserCountList = new ChartValues<int>();
            TimeStampsList = new ObservableCollection<string>();


      


            ChatCountListForUserId = new ChartValues<int>();
            ChatCountListForUserName = new ChartValues<int>();
            UserIdList = new ObservableCollection<string>();

            UserNameList = new ObservableCollection<string>();
         

            AttentiveUsersSetter = 100;
            NonAttentiveUsersSetter = 0;

            TotalParticipantsCountSetter = 1;
            MaxTotalParticipantsCountSetter = 1;
            TotalMessageCountSetter = 0;
            EngagementRateSetter = "0";
            SessionModeSetter = "LabMode";
            SessionScoreSetter = 0;
            

            clientSessionManager = SessionManagerFactory.GetClientSessionManager();
            clientSessionManager.SubscribeSession(this);

            Trace.WriteLine("Initializing the dashboard viewmodel");
            ButtonContentSetter = "Switch Mode";

            SummaryContentSetter = "Refresh To get the updated summary";




            ////defining the sessionanalytics to store the information about the sessionanalytics 
            sessionAnalytics = new SessionAnalytics();
            sessionAnalytics.chatCountForEachUser = new Dictionary<int, int>();
            sessionAnalytics.listOfInSincereMembers = new List<int>();
            sessionAnalytics.userCountVsTimeStamp = new Dictionary<DateTime, int>();
            sessionAnalytics.sessionSummary = new SessionSummary();

          
            clientSessionManager.AnalyticsCreated += (latestAnalytics) => OnAnalyticsChanged(latestAnalytics);
            clientSessionManager.SummaryCreated += (latestSummary) => OnSummaryChanged(latestSummary);

        }


     


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



        //function to update the viewModel whenever required 
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




            return;

        }



        //function to access the private members 
        public ClientSessionManager GetClientSessionManager()
        {
            return clientSessionManager;
        }

        public SessionAnalytics GetSessionAnalytics()
        {
            return sessionAnalytics;
        }

        public SessionData GetSessionData()
        {
            return sessionData;
        
        }




        //function to update the button content 
        public void UpdateButtonContent(UserData currUser)
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

        //function to change the session mode 
        public void SwitchSessionMode()
        {

            //getting the current user because this action is only allowed for the faculty 
            UserData currUser = clientSessionManager.GetUser();


            if (currUser.userID == 1)
            {
                //calling the toggle function to toggle the session for this particular meeting
                clientSessionManager.ToggleSessionMode();

            }
            else
            {
                //show alert here for this purpose on the UX 
            }

            //say everything went fine 
            return;

        }



        //function to initiate leavemeeting procedure 
        public void LeaveMeetingProcedure()
        {
            //getting the current user as LeaveMeetingProcedure is only allowed for the server 
            UserData currUser = clientSessionManager.GetUser();

            if (currUser.userID == 1)
            {
                //this user is host hence it will end the meet  
                //calling the end meet procedure 
                clientSessionManager.EndMeet();
            }
            else
            {
                //the user will be just removed by session manager 
                //this is normal user hence we have to call the remove client 
                clientSessionManager.RemoveClient();

            }

        }



        //function to update the ParticipantsList of viewmodel 
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


        //function to fetch the hour and the minute format 
        public string GetHourAndMinute(DateTime currDateTime)
        {
            string currHour = currDateTime.ToString("HH");
            string currSecond = currDateTime.ToString("mm");
            string finalTimeStamp = currHour + ":" + currSecond;

            //say everything went fine 
            return finalTimeStamp;

        }



        //function to update usercountvstimestamp observable collection to update the view 
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



        //function to update the useridvschatcounts 
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


        //function to calculate the number of attentive and non attentive users in the meeting 
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



        public void SetMaxTotalParticipantsCount()
        {
            if (TotalParticipantsCountSetter > MaxTotalParticipantsCountSetter)
            {
                MaxTotalParticipantsCountSetter = TotalParticipantsCountSetter;
            }
        }



        //function to listen to any of the session data changed subscribed to the IClientSessionNotifications
        public void OnClientSessionChanged(SessionData newSessionData)
        {

            if (newSessionData != null)
            {
                //we have to update the participants list and SessionMode
                UpdateParticipantsList(newSessionData.users);
                UserData currUser = clientSessionManager.GetUser();

                SetLeaveButtonAccordingToUser();
                SessionModeSetter = newSessionData.sessionMode;
                UpdateButtonContent(currUser);

                TotalParticipantsCountSetter = ParticipantsList.Count;
                SetMaxTotalParticipantsCount();

            }


            return;
        }


        //overloading function to test it 
        public void OnClientSessionChanged(SessionData newSessionData, int testingGateway)
        {

            if (newSessionData != null)
            {
                //we have to update the participants list and SessionMode
                UpdateParticipantsList(newSessionData.users);
                UserData currUser = clientSessionManager.GetUser();

                SessionModeSetter = newSessionData.sessionMode;
                UpdateButtonContent(currUser);

                TotalParticipantsCountSetter = ParticipantsList.Count;
                SetMaxTotalParticipantsCount();

            }


            return;
        }


        //defining the function to update the summary of the current session till now 
        public void OnSummaryChanged(string latestSummary)
        {
            if (latestSummary == null)
            {
                //say everything went fine 
                return;
            }

            //updating the summary for this session 
            SummaryContentSetter = latestSummary;

            //say everything went fine 
            return;
        }



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



        //implementing the onanalytics changed
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

            //say everything went fine 
            return;
        }




        //############################################################################## 
        //Function to calculate the engagement rate 
        public void CalculateEngagementRate(Dictionary<string, int> currUserNameVsChatCount)
        {
            int activeMembers = currUserNameVsChatCount.Count;

            float EngagementRate = (float)(activeMembers*100) / MaxTotalParticipantsCountSetter;
            EngagementRateSetter = EngagementRate.ToString("0") + "%";

            //say everything went fine 
            return;
        }



        //the following function notifies the view whenever the property changes on the viewmodel 
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }




        public event PropertyChangedEventHandler PropertyChanged;

    }


}
