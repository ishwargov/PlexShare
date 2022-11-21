using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareCloud;
using PlexShareCloudUX;

namespace PlexShareTests.CloudTests
{
    public class SubmissionsModelTests
    {
        [Fact]
        public void TestGetDownloadFolderPath()
        {
            string[] folder = SubmissionsModel.GetDownloadFolderPath().Split("\\");
            Assert.Equal("Downloads", folder[folder.Count()-1]);
        }

        [Fact]
        public void TestGetSubmissions()
        {
            SubmissionsModel model = new SubmissionsModel();
            var task = model.GetSubmissions("0");
            task.Wait();
            IReadOnlyList<SubmissionEntity> entities = new List<SubmissionEntity>();
            Assert.Equal(task.Result, entities);
        }



    }
}
