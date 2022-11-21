using PlexShareCloudUX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareCloud;

namespace PlexShareTests.CloudTests
{
    public class SubmissionsViewModelTests
    {
        SubmissionsViewModel viewModel = new SubmissionsViewModel("Dummy");
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
