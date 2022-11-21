using PlexShareCloudUX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareCloudUX;
using PlexShareCloud;

namespace PlexShareTests.CloudTests
{
    public class SessionsViewModelTests
    {
        SessionsViewModel viewModel = new SessionsViewModel("Dummy");
        [Fact]
        public void TestOnPropertyChange()
        {
            string changedProperty = "";
            viewModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                changedProperty = e.PropertyName;
            };
            viewModel.OnPropertyChanged("ReceivedSessions");
            Assert.Equal("ReceivedSessions", changedProperty);
        }

        [Fact]
        public void TestGetSubmissions()
        {
            viewModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                Assert.Equal("ReceivedSessions", e.PropertyName);
                Assert.Equal(new List<SessionEntity>(), this.viewModel.ReceivedSessions);
            };
            viewModel.GetSessions("Dummy");
        }
    }
}
