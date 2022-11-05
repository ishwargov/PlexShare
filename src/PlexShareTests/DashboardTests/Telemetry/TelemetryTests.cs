using Dashboard;
using Dashboard.Server.Persistence;
using PlexShareDashboard.Dashboard.Server.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace PlexShareTests.DashboardTests.Telemetry
{
    public class TelemetryTests
    {
        //in  this we write the unit test for the telemetry submodule 
        //defining  the persistence factory for this purpose 
        private readonly TelemetryPersistence persistenceInstance = new TelemetryPersistence();
        [Fact]
        //writing the first test to checking the function CalculateUserCountVsTimeStamp 
        public void CalculateUserCountVsTimeStamp_ShouldGiveUserCountAtTimeStamp()
        { 
            //Arrange 
            //first we have to arrange or do the setup 
            UserData user1 = new UserData("Rupesh", 1);

            //defining the new session 
            SessionData sessionData1 = new SessionData();
            //adding the new user to the session data to hardcode the users list 
            sessionData1.AddUser(user1);

            //ACT ==> now we have to bring the telemetry module in action to check whether it is working fine or not

            DateTime currDateTime = DateTime.Now;
            //defining the instance for the telemetry 
            var telemetryInstance = TelemetryFactory.GetTelemetryInstance();

            //calling the function to find the user count vs the time stamp 
            telemetryInstance.CalculateUserCountVsTimeStamp(sessionData1, currDateTime);
            int result1 = telemetryInstance.userCountVsEachTimeStamp[currDateTime];

            //ASSERT ==> now we have to check whether the output is the correct or not 
            var check1 = false;
            int userCount = sessionData1.users.Count();
            if (result1 == userCount)
            {
                check1 = true;
            }

            Assert.True(check1);

            //say everything went fine 
            return;


        }

    }
}
