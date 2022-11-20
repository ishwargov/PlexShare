/// <author>Rupesh Kumar</author>
/// <summary>
/// this is the telemtry factory to be able to get the telemetry instance just be calling the function GetTelemetryInstance. This implements the singleton design pattern 
/// </summary>


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
