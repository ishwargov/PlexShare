using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareCloud;
using PlexShareCloudUX;
using Xunit;

namespace PlexShareTests.CloudTests
{
    public class CurrentSubmissionsViewModelTests
    {
        CurrentSubmissionsViewModel viewModel = new CurrentSubmissionsViewModel("Dummy");
        [Fact]
        public void TestOnPropertyChange()
        {
            string changedProperty = "";
            viewModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                changedProperty = e.PropertyName;
            };
            viewModel.OnPropertyChanged("ReceivedSubmissions");
            Assert.Equal("ReceivedSubmissions", changedProperty);
        }

        [Fact]
        public void TestGetSubmissions()
        {
            viewModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                Assert.Equal("ReceivedSubmissions", e.PropertyName);
                Assert.Equal(new List<SubmissionEntity>(), this.viewModel.ReceivedSubmissions);
            };
            viewModel.GetSubmissions("Dummy");
        }

    }
}
