using System;

namespace PlexShareNetworking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            PlexShareNetworking.Serialization.Serializer s = new PlexShareNetworking.Serialization.Serializer();
            var message = "Hello";

            String str = s.Serialize(message);

            Console.Out.WriteLine(str);
            Console.Out.WriteLine(s.Deserialize(str));
        }
    }
}
