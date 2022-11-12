using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareApp
{
    internal class TraceFile
    {
        public TraceFile()
        {
            string filepath = "";
            string tracefile = Path.Combine(filepath,
            string.Format("PlexShare-{0:yyyy-MM-dd_HH-mm-ss}.trace",
                      DateTime.Now));
            Trace.Listeners.Add(new
                TextWriterTraceListener(File.CreateText(tracefile)));
            Trace.AutoFlush = true;
            Trace.WriteLine("Test");
        }
    }
}
