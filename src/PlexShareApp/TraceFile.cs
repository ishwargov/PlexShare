/// <author>Ishwar Govind</author>
/// <summary>
/// Trace Configuration
/// Trace to a new file on every run
/// </summary>

using System;
using System.Diagnostics;
using System.IO;

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
