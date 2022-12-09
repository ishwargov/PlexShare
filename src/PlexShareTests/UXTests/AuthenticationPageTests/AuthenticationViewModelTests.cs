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

namespace PlexShareTests.UXTests.AuthenticationPageTests
{
    public class AuthenticationViewModelUnitTest
    {
        private AuthenticationViewModel viewModel;
        public AuthenticationViewModelUnitTest()
        {
            viewModel = new AuthenticationViewModel();
        }

        /// <summary>
        /// This is used to test any kind of timeout scenario
        /// 1. If the browser window is closed and never reopened
        /// 2. Internet is not working
        /// 3. Not entering credentials for too long
        /// </summary>
        [Fact]
        public async void WebPageTimeout()
        {
            // Giving a timeout of 50 ms to reduce the wait time
            var returnval = await viewModel.AuthenticateUser(50);
            // Assert
            Assert.Equal("false", returnval[0]);
        }

        // Commenting this out since this is 
        // semi automatic

        /// <summary>
        /// In case all information have been provided succesfully
        /// </summary>
        //[Fact]
        //public async void ValidData()
        //{
        //    var returnVal = await viewModel.AuthenticateUser();
        //    // Assert
        //    Assert.Equal("true", returnVal[0]);
        //    Assert.NotEmpty(returnVal[1]);
        //    Assert.NotEmpty(returnVal[2]);
        //    Assert.NotEmpty(returnVal[3]);
        //}
    }
}