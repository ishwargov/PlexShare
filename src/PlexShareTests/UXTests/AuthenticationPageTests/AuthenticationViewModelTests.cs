/******************************************************************************
 * Filename    = AuthenticationViewModelTests.cs
 *
 * Author      = Parichita Das
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareApp
 *
 * Description = Unit Testing the View Model of Authentication module
 *****************************************************************************/

using AuthViewModel;
using Xunit.Abstractions;

namespace PlexShareTests.UXTests.AuthenticationPageTests
{
    public class AuthenticationViewModelUnitTest
    {
        private AuthenticationViewModel viewModel;
        //private readonly ITestOutputHelper output;
        //private TestConte
        public AuthenticationViewModelUnitTest(ITestOutputHelper output)
        {
            viewModel = new AuthenticationViewModel();
            //this.output = output;
        }

        [Fact]
        public async void WebPageTimeout()
        {
            // Closing the Webpage
            // Internet not working
            // Not entering credentials for too long
            var returnval = await viewModel.AuthenticateUser(50);
            // Assert
            Assert.Equal("false", returnval[0]);
        }

        [Fact]
        public async void ValidData()
        {
            var returnVal = await viewModel.AuthenticateUser();
            // Assert
            Assert.Equal("true", returnVal[0]);
            Assert.NotEmpty(returnVal[1]);
            Assert.NotEmpty(returnVal[2]);
            Assert.NotEmpty(returnVal[3]);
        }
    }
}
