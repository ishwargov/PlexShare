using Dashboard;
using Dashboard.Server.Persistence;
using PlexShareDashboard.Dashboard.Server.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
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

        //checking the test case for singleton design pattern 
        [Fact]
        public void SingletonFactory_Test_Check()
        {
            ITelemetry telemetryInstance1 = TelemetryFactory.GetTelemetryInstance();
            ITelemetry telemetryInstance2 = TelemetryFactory.GetTelemetryInstance();

            Assert.Equal(telemetryInstance1, telemetryInstance2);
            //Assert.True(telemetryInstance1.Equals(telemetryInstance2));
        
        }

        [Fact]
        //writing the first test to checking the function CalculateUserCountVsTimeStamp 
        public void CalculateUserCountVsTimeStamp_ShouldGiveUserCountAtTimeStamp_Simple_Test()
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
        [Fact]
        //Complex test case to check the calculate user count vs time stamp function 
        public void CalculateUserCountVsTimeStamp_ShouldGiveUserCountAtTimeStamp_Complex_Test()
        {
            //Arrange ==> defining the users 
            UserData user1 = new UserData("Rupesh Kumar", 1);
            UserData user2 = new UserData("Shubham Raj", 2);
            UserData user3 = new UserData("Saurabh Kumar", 3);
            UserData user4 = new UserData("Aditya Agarwal", 4);
            UserData user5 = new UserData("Hrishi Raaj", 5);
            UserData user6 = new UserData("user6", 6);
            UserData user7 = new UserData("user7", 7);
            UserData user8 = new UserData("user8", 8);

            //defining the session data 
            SessionData sessionData = new SessionData();
            //var currTime = new DateTime(2021, 11, 23, 1, 0, 0);
            DateTime currDateTime1 = new DateTime(2021, 11, 23, 1, 0, 0);
            DateTime currDateTime2 = new DateTime(2021, 11, 23, 1, 1, 0);
            DateTime currDateTime3 = new DateTime(2021, 11, 23, 1, 2, 0);

            sessionData.AddUser(user1);
            sessionData.AddUser(user2);
            sessionData.AddUser(user3);

            var telemetryInstance = TelemetryFactory.GetTelemetryInstance();
            telemetryInstance.CalculateUserCountVsTimeStamp(sessionData, currDateTime1);
            int result1 = telemetryInstance.userCountVsEachTimeStamp[currDateTime1];

            sessionData.AddUser(user4);
            sessionData.AddUser(user5);

            telemetryInstance.CalculateUserCountVsTimeStamp(sessionData, currDateTime2);
            int result2 = telemetryInstance.userCountVsEachTimeStamp[currDateTime2];


            sessionData.AddUser(user6);
            sessionData.AddUser(user7);
            sessionData.AddUser(user8);

            telemetryInstance.CalculateUserCountVsTimeStamp(sessionData, currDateTime3);
            int result3 = telemetryInstance.userCountVsEachTimeStamp[currDateTime3];
            
            
            bool check1 = false;
            bool check2 = false;
            bool check3 = false;

            if (result1 == 3)
            {
                //Trace.WriteLine("Got True for check1");
                check1 = true;
            }

            if (result2 == 5)
            {
                check2 = true;
            }

            if (result3 == 8)
            {
                check3 = true;
            }

            Assert.True(check1 && check2 && check3);

            //say everything went fine 
            return;
        }




        //Testing for calculation of entry time of the users 
        [Fact]
        public void CalculateArrivalExitTimeOfUser_Test_Entry_Time_Calculation()
        {
            //Arrange ==> this is to check the calculation of the entry time 
            UserData user1 = new UserData("Rupesh Kumar", 1);
            UserData user2 = new UserData("Shubham Raj", 2);
            UserData user3 = new UserData("Saurabh Kumar", 3);
            UserData user4 = new UserData("Aditya Agarwal", 4);
            UserData user5 = new UserData("Hrishi Raaj", 5);

            DateTime currDateTime1 = new DateTime(2021, 11, 23, 1, 0, 0);
            DateTime currDateTime2 = new DateTime(2021, 11, 23, 1, 1, 0);
            DateTime currDateTime3 = new DateTime(2021, 11, 23, 1, 2, 0);
            DateTime currDateTime4 = new DateTime(2021, 11, 23, 1, 3, 0);

            SessionData sessionData = new SessionData();

            //Act ==> now we have to add the data to the session data and then call the function 
            DateTime dateTime1 = new DateTime(2021, 11, 23, 1, 0, 0);
            DateTime dateTime2 = new DateTime(2021, 11, 23, 1, 1, 0);
            DateTime dateTime3 = new DateTime(2021, 11, 23, 1, 2, 0);
            DateTime dateTime4 = new DateTime(2021, 11, 23, 1, 3, 0);


            sessionData.AddUser(user1);
            sessionData.AddUser(user2);

            //calling the function to calculate the entry time whenever the session data changes 
            var telemetryInstance = TelemetryFactory.GetTelemetryInstance();
            telemetryInstance.CalculateArrivalExitTimeOfUser(sessionData, currDateTime1);
            //DateTime dateTime1 = new DateTime()
            sessionData.AddUser(user3);

            telemetryInstance.CalculateArrivalExitTimeOfUser(sessionData, currDateTime2);

            sessionData.AddUser(user4);
            telemetryInstance.CalculateArrivalExitTimeOfUser(sessionData, currDateTime4);

            sessionData.AddUser(user5);
            telemetryInstance.CalculateArrivalExitTimeOfUser(sessionData, currDateTime3);


            var check1 = false;
            var check2 = false;
            var check3 = false;
            var check4 = false;

            //ASSERT 
            if (telemetryInstance.eachUserEnterTimeInMeeting[user1] == dateTime1 && telemetryInstance.eachUserEnterTimeInMeeting[user2] == dateTime1)
            {
                check1 = true;
            }

            if (telemetryInstance.eachUserEnterTimeInMeeting[user3] == dateTime2)
            {
                check2 = true;
            }

            if (telemetryInstance.eachUserEnterTimeInMeeting[user4] == dateTime4)
            {
                check3 = true;
            }

            if (telemetryInstance.eachUserEnterTimeInMeeting[user5] == dateTime3)
            {
                check4 = true;
            }


            Assert.True(check1 && check2 && check3 && check4);

            //say everything went fine 
            return;
        }




        [Fact]
        //function to test the exit time of the users when no user exits 
        public void CalculateArrivalExitTimeOfUser_Test_Exit_Time_Calculation_When_No_User_Exits()
        {
            UserData user1 = new UserData("Rupesh Kumar", 1);
            UserData user2 = new UserData("Shubham Raj", 2);
            UserData user3 = new UserData("Saurabh Kumar", 3);


            DateTime currDateTime1 = new DateTime(2021, 11, 23, 1, 0, 0);
            DateTime currDateTime2 = new DateTime(2021, 11, 23, 1, 1, 0);
            DateTime currDateTime3 = new DateTime(2021, 11, 23, 1, 2, 0);


            SessionData sessionData = new SessionData();
            sessionData.AddUser(user1);
            sessionData.AddUser(user2);
            sessionData.AddUser(user3);

            var telemetryInstance = TelemetryFactory.GetTelemetryInstance();
            telemetryInstance.CalculateArrivalExitTimeOfUser(sessionData, currDateTime1);

            //Act
            bool check1 = false;
            if (telemetryInstance.eachUserExitTime.Count() == 0)
            {
                check1 = true;
            }

            //Assert 
            Assert.True(check1);


            //say everything went fine 
            return;

        }


        //function to check the exit time 
        [Fact]
        public void CalculateArrivalExitTimeOfUser_Test_Exit_Time_Calculation_Complex_Test()
        {
            UserData user1 = new UserData("Rupesh Kumar", 1);
            UserData user2 = new UserData("Hrishi Raaj Singh Chauhan", 2);

            SessionData sessionData = new SessionData();
            sessionData.AddUser(user1);
            DateTime currTime1 = new DateTime(2021, 11, 23, 1, 0, 0);

            var telemetryInstance = TelemetryFactory.GetTelemetryInstance();
            telemetryInstance.CalculateArrivalExitTimeOfUser(sessionData, currTime1);

            //let us say that the user1 leaves the meeting 
            //then the session would be empty 
            SessionData sessionData2 = new SessionData();
            sessionData2.AddUser(user2);
            DateTime currTime2 = new DateTime(2021, 11, 23, 1, 1, 0);

            telemetryInstance.CalculateArrivalExitTimeOfUser(sessionData2, currTime2);

            bool check1 = false;

            if (telemetryInstance.eachUserExitTime[user1] == currTime2)
            {
                check1 = true;
            }


            Assert.True(check1);

            //say everything went fine 
            return;

        }
    }
}
