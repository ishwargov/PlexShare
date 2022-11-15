//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static PlexShareTests.UX.HomePageTests.HomePageUtils;
//using Client.ViewModel;
//using NUnit.Framework;
//using static Testing.UX.Authentication.AuthUtils;

//namespace PlexShareTests.UX.HomePageTests
//{
//    internal class HomePageViewModelTests
//    {
//        [TestFixture]
//        public class AuthViewModelUnitTest
//        {
//            [SetUp]
//            public void SetUp()
//            {
//                _viewModel = new AuthViewModel(new FakeClientSessionManager());
//            }

//            private AuthViewModel _viewModel;

//            [Test]
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
