using System;
using Xunit;
using PlexShareDashboard.Server.Persistence;
namespace PlexShareTests.DashboardTests.Persistence
{
    public class PersistenceUnitTest
    {
        public PersistenceUnitTest()
        {
            summary = "Hello succesfully saved the summary";
            bool save = PersistenceFactory.GetSummaryPersistenceInstance().SaveSummary(summary, true);

        }
    }
}

