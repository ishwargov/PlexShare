using PlexShareDashboard.Dashboard.UI.ViewModel;
using System;
using System.Collections.Generic;
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



    }
}
