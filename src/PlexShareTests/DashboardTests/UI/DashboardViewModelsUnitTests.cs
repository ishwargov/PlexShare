using Dashboard;
using PlexShareDashboard.Dashboard.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PlexShareTests.DashboardTests.UI
{
    public class DashboardViewModelsUnitTests
    {
        //here we will write the unit testing for the dashboard view model 
        //first doing the set up for testing purpose 
        private DashboardViewModel DashboardViewModelForTest = new();
        //test for doing the setup 
        [Fact]
        public void SetUpTest()
        { 
            Assert.NotNull(DashboardViewModelForTest);
            Assert.IsType<DashboardViewModel>(DashboardViewModelForTest);
            Assert.NotNull(DashboardViewModelForTest.GetClientSessionManager());
            Assert.NotNull(DashboardViewModelForTest.GetSessionData());
            Assert.NotNull(DashboardViewModelForTest.GetSessionAnalytics());
            Assert.NotNull(DashboardViewModelForTest.ParticipantsList);
            Assert.NotNull(DashboardViewModelForTest.UserCountList);
            Assert.NotNull(DashboardViewModelForTest.ChatCountList);
            Assert.NotNull(DashboardViewModelForTest.UserIdList);
            Assert.NotNull(DashboardViewModelForTest.TimeStampsList);

        }

        [Fact]
        //writing the testing for checking the initialization process 
        public void Initialization_Test_Of_Lists()
        {
            //we have to test the initialisation of the list  
            Assert.Empty(DashboardViewModelForTest.ParticipantsList);
            Assert.Empty(DashboardViewModelForTest.UserCountList);
            Assert.Empty(DashboardViewModelForTest.UserIdList);
            Assert.Empty(DashboardViewModelForTest.TimeStampsList);
            Assert.Empty(DashboardViewModelForTest.ChatCountList);

            //say everything went fine 
            return;
        }

        //testing function for testing the initialization of the variables 
        [Fact]
        public void Initialization_Test_Of_Variables()
        {
            Assert.Equal(100, DashboardViewModelForTest.AttentiveUsersSetter);
            Assert.Equal(0, DashboardViewModelForTest.NonAttentiveUsersSetter);
            Assert.Equal(1, DashboardViewModelForTest.TotalParticipantsCountSetter);
            Assert.Equal(0, DashboardViewModelForTest.TotalMessageCountSetter);
            Assert.Equal("0", DashboardViewModelForTest.EngagementRateSetter);
            Assert.Equal("LabMode", DashboardViewModelForTest.SessionModeSetter);
            Assert.Equal(0, DashboardViewModelForTest.SessionScoreSetter);
            Assert.Equal("Switch Mode", DashboardViewModelForTest.ButtonContentSetter);

            //say everything went fine 
            return;
        }

        [Fact]
        //function to test the on property change event whether it is triggering or not 
        public void OnPropertyChange_Event_Test()
        {
            string CheckCurrentProperty = DashboardViewModelForTest.EngagementRateSetter;
            DashboardViewModelForTest.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                CheckCurrentProperty = e.PropertyName;

            };

            DashboardViewModelForTest.OnPropertyChanged("EngagementRateSetter");

            Assert.NotNull(CheckCurrentProperty);
            Assert.Equal("EngagementRateSetter", CheckCurrentProperty);

            //say everything went fine 
            return;
        
        }


        //function to check the updatebutton setter 
        [Fact]
        public void UpdateButtonContent_Test_To_Update_According_To_Current_Mode()
        {
            //DashboardViewModelForTest.ButtonContentSetter = "LabMode";
            DashboardViewModelForTest.SessionModeSetter = "LabMode";

            UserData user1 = new UserData("Rupesh Kumar", 1);

            DashboardViewModelForTest.UpdateButtonContent(user1);

            Assert.Equal("Switch To ExamMode", DashboardViewModelForTest.ButtonContentSetter);

            //say everything went fine 
            return;
        }

        [Fact]
        //writing the function to test the update participant list 
        public void UpdateParticipantsList_Test()
        {
            List<UserData> newParticipantsList = new List<UserData>();
            UserData user1 = new UserData("Rupesh kumar", 1);
            UserData user2 = new UserData("Saurabh Kumar", 2);
            UserData user3 = new UserData("Hrishi Raaj", 3);

            newParticipantsList.Add(user1);
            newParticipantsList.Add(user2);
            newParticipantsList.Add(user3);

            DashboardViewModelForTest.UpdateParticipantsList(newParticipantsList);

            Assert.Equal("Rupesh kumar", DashboardViewModelForTest.ParticipantsList[0].UserName);
            Assert.Equal("Saurabh Kumar", DashboardViewModelForTest.ParticipantsList[1].UserName);
            Assert.Equal("Hrishi Raaj", DashboardViewModelForTest.ParticipantsList[2].UserName);
            Assert.Equal(3, DashboardViewModelForTest.ParticipantsList.Count);

            //say everything went fine 
            return;
        }
    }
}
