using PlexShareContent.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareTests.DashboardTests.SessionManagement.TestModules
{
    public class FakeContentServer 
    {
        public ChatThread[]  GetAllMessages()
        {
            return new ChatThread[0];
        }
    }
}
