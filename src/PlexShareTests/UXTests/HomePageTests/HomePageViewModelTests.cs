/************************************************************
 * Filename    =HomePAgeViewModelTests.cs
 *
 * Author      = Jasir
 *
 * Product     = PlexShare
 * 
 * Project     = UX Team
 *
 * Description = Testing the viewmodel for different buttons and dowload image option
 * 
 ************************************************************/
using Xunit;
using PlexShareApp;
using PlexShareDashboard.Dashboard.Client.SessionManagement;
using PlexShare.Dashboard;
using PlexShareTests.UX.HomePageTests;
using PlexShareTests.UXTests.HomePageTests;

HomePageViewModelUnitTest tmp = new();
tmp.OnNewMeeting();

namespace PlexShareTests.UXTests.HomePageTests
{
    public class HomePageViewModelUnitTest
    {
        private HomePageViewModel _viewModel;

        public HomePageViewModelUnitTest()
        {
            _viewModel = new HomePageViewModel(new FakeClientSessionManager(), new FakeServerSessionManager());
        }

        [Fact]
        public void OnNewMeeting()
        {
            // VerifyCredentials returns - List<string> { ip, port, isValidUserName, isValidIpAddress, isValidPort, isServer, isVerified}

            // Empty Username Check
            var result = _viewModel.VerifyCredentials("", "-1", "0", "111901025@smail.iitpkd.ac.in", "");
            Assert.Equal("-1", result[0]);
            Assert.Equal("0", result[1]);
            Assert.Equal("False", result[2]);            // "" is a Invalid username
            Assert.Equal("True", result[3]);
            Assert.Equal("True", result[4]);
            Assert.Equal("True", result[5]);
            Assert.Equal("False", result[6]);           // Not verified

            // Not null username
            result = _viewModel.VerifyCredentials("Jasir", "-1", "0", "111901025@smail.iitpkd.ac.in", "");
            Assert.Equal("192.168.10.11", result[0]);   // This IP is received from FakeServerSessionManager
            Assert.Equal("12330", result[1]);            // PORT receved from FakeServerSessionManager
            Assert.Equal("True", result[2]);
            Assert.Equal("True", result[3]);
            Assert.Equal("True", result[4]);
            Assert.Equal("True", result[5]);
            Assert.Equal("True", result[6]);
        }

        [Fact]
        public void OnJoinMeeting()
        {
            // Empty Username Check, IP and PORT Testing
            var result = _viewModel.VerifyCredentials("", "....", "11111111", "111901025@smail.iitpkd.ac.in", "");
            Assert.Equal("....", result[0]);
            Assert.Equal("11111111", result[1]);
            Assert.Equal("False", result[2]);            // "" is a Invalid username
            Assert.Equal("False", result[3]);
            Assert.Equal("False", result[4]);
            Assert.Equal("False", result[5]);
            Assert.Equal("False", result[6]);           // Not verified

            // Invalid case testing for wrong IP and PORT
            result = _viewModel.VerifyCredentials("Jasir", "192.18.12.2121", "1000", "111901025@smail.iitpkd.ac.in", "");
            Assert.Equal("192.18.12.2121", result[0]);
            Assert.Equal("1000", result[1]);
            Assert.Equal("True", result[2]);
            Assert.Equal("False", result[3]);
            Assert.Equal("False", result[4]);
            Assert.Equal("False", result[5]);
            Assert.Equal("False", result[6]);           // Not verified

            // Valid case when it should get verified
            result = _viewModel.VerifyCredentials("Jasir", "192.18.12.2", "2003", "111901025@smail.iitpkd.ac.in", "");
            Assert.Equal("192.18.12.2", result[0]);
            Assert.Equal("2003", result[1]);
            Assert.Equal("True", result[2]);
            Assert.Equal("True", result[3]);
            Assert.Equal("True", result[4]);
            Assert.Equal("False", result[5]);
            Assert.Equal("True", result[6]);           // Verified
        }

        [Fact]
        public void OnDownloadImage()
        {
            // When Image URL is null
            var path = _viewModel.DownloadImage("", "111901025@smail.iitpkd.ac.in");
            Assert.Equal("./Resources/AuthScreenImg.jpg", path);

            // When Image URL is valid
            //path =_viewModel.DownloadImage("")
            path = _viewModel.DownloadImage("https://lh3.googleusercontent.com/a/ALm5wu0nw9fFKOPz2IhEMI4cY5HLgNMPnEr2LUHUu8E6wg=s96-c", "111901025@smail.iitpkd.ac.in");
            string dir = Environment.GetEnvironmentVariable("temp", EnvironmentVariableTarget.User);
            string absolute_path = System.IO.Path.Combine(dir, "111901025");
            Assert.Equal(absolute_path, path);
            Assert.True(File.Exists(path));
        }
    }
}
