//using static PlexShareTests.UX.HomePageTests.HomePageUtils;
//using PlexShareApp;

//namespace PlexShareTests.UX.HomePageTests
//{
//    internal class HomePageViewModelTests
//    {
//        public class AuthViewModelUnitTest
//        {
//            //[SetUp]
//            private HomePageViewModel _viewModel;

//            public AuthViewModelUnitTest()
//            {
//                _viewModel = new HomePageViewModel();
//            }


//            [Fact]
//            public void OnUserLogin_ReturnBool()
//            {
//                //Assert
//                Assert.AreEqual(_viewModel.SendForAuth("192.168.1.1", 123, "Jasir"), true);
//                Assert.AreEqual(_viewModel.SendForAuth("192 168.1 .1", 123, "Jasir"), false);
//                Assert.AreEqual(_viewModel.SendForAuth("192.168.1.1", 123, ""), false);
//                Assert.AreEqual(_viewModel.SendForAuth(" ", 123, ""), false);
//                Assert.AreEqual(_viewModel.SendForAuth("", 123, "Jasir"), false);
//            }
//        }
//    }
//}
