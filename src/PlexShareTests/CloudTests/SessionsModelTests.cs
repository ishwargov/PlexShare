using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareCloud;
using PlexShareCloudUX;

namespace PlexShareTests.CloudTests
{
    public class SessionsModelTests
    {

        [Fact]

        public void TestGetSessionsDetails()
        {
            SessionsModel sessionsModel = new SessionsModel();
            var task = sessionsModel.GetSessionsDetails("Dummy");
            task.Wait();
            IReadOnlyList<SessionEntity> entities = new List<SessionEntity>();
            Assert.Equal(task.Result, entities);
        }

    }
}
