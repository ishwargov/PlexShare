using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareApp
{
    public class TraceFile 
    {
        string traceFile;
        public TraceFile()
        {
            string filePath = "";
            traceFile = Path.Combine(filePath,
            string.Format("PlexShare-{0:yyyy-MM-dd_HH-mm-ss}.trace",DateTime.Now));
            Trace.Listeners.Add(new TextTracer(File.CreateText(traceFile)));
            Trace.AutoFlush = true;
            Trace.WriteLine("--PlexShare-Trace--");
        }
    }

    internal class TextTracer : TextWriterTraceListener
    {
        public TextTracer(TextWriter writer) : base(writer)
        {
        }
        public override void WriteLine(string? message)
        {
            base.WriteLine(string.Format("{0:r} \t {1}", DateTime.UtcNow, message));
        }
    }

}
