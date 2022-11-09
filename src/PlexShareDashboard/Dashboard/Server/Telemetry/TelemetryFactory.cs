using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareDashboard.Dashboard.Server.Telemetry
{
    public class TelemetryFactory
    {
        //this is the factory for the telmetry module 
        //we will be using the singleton design pattern 
        private static readonly Telemetry telemetry;

        static TelemetryFactory()
        {
            if (telemetry == null) telemetry = new Telemetry();
        }

        public static Telemetry GetTelemetryInstance()
        {
            return telemetry;
        }
    }
}
