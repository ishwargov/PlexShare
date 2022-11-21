using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareCloudUX;
using Moq;
using PlexShareCloud;

namespace PlexShareTests.CloudTests
{
    public class UploadViewModelTests
    {
        UploadViewModel viewModel = new UploadViewModel("Dummy", "Dummy", false);

        [Fact]
        public void TestOnPropertyChange()
        {
            string changedProperty = "";
            viewModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                changedProperty = e.PropertyName;
            };
            viewModel.OnPropertyChanged("DocumentUploded");
            Assert.Equal("DocumentUploded", changedProperty);
        }
    }
}
