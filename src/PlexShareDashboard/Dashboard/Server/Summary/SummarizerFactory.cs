using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareDashboard.Dashboard.Server.Summary
{
    public static class SummarizerFactory
    {
        private static readonly ISummarizer _summarizer;

        static SummarizerFactory()
        {
            _summarizer = new Summarizer();
        }
        public static ISummarizer GetSummarizer()
        {
            return _summarizer;
        }
    }
}
