using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareCloud;
using PlexShareCloudUX;

namespace PlexShareTests.CloudTests
{
    public class CurrentSubmissionsModelTests
    {
        [Fact]
        public void TestGetSubmissions()
        {
            CurrentSubmissionsModel model = new CurrentSubmissionsModel();
            var task = model.GetSubmissions("0");
            task.Wait();
            IReadOnlyList<SubmissionEntity> entities = new List<SubmissionEntity>();
            Assert.Equal(task.Result, entities);
        }
    }
}
