/// <author>Mayank Singla</author>
/// <summary>
/// Defines the static "Utils" class which defines
/// general utilities required for unit tests.
/// </summary>

using PlexShareScreenshare;
using PlexShareScreenshare.Server;
using System.Drawing;
using System.Net;
using System.Text.Json;

namespace PlexShareTests.ScreenshareTests
{
    /// <summary>
    /// Defines the static "Utils" class which defines
    /// general utilities required for unit tests.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Random number generator.
        /// </summary>
        private static readonly Random rand = new(DateTime.Now.Second);

        /// <summary>
        /// Gets a mock instance for "SharedClientScreen" with given client Id (if provided).
        /// </summary>
        /// <param name="server">
        /// The server object required by the client object
        /// </param>
        /// <param name="id">
        /// Id of the client to bind to
        /// </param>
        /// <returns>
        /// A mock instance for "SharedclientScreen"
        /// </returns>
        public static SharedClientScreen GetMockClient(ITimerManager server, int id = -1)
        {
            // Generate a random client Id if not given
            string clientId = (id == -1) ? rand.Next().ToString() : id.ToString();
            return new(clientId, rand.Next().ToString(), server);
        }

        /// <summary>
        /// Get a list of mock "SharedClientScreen"
        /// </summary>
        /// <param name="server">
        /// The server object required by the client object
        /// </param>
        /// <param name="count">
        /// Number of mock clients
        /// </param>
        /// <returns>
        /// A list of mock "SharedClientScreen"
        /// </returns>
        public static List<SharedClientScreen> GetMockClients(ITimerManager server, int count)
        {
            List<SharedClientScreen> list = new();
            for (int i = 2; i < count + 2; ++i)
            {
                list.Add(Utils.GetMockClient(server, i));
            }
            return list;
        }

        /// <summary>
        /// Gets a mock serialized "REGISTER" packet received by screen share server.
        /// </summary>
        /// <param name="clientId">
        /// Id of the client who sends this packet
        /// </param>
        /// <param name="clientName">
        /// Name of the client who sends this packet
        /// </param>
        /// <returns>
        /// A mock serialized "REGISTER" packet received by screen share server
        /// </returns>
        public static string GetMockRegisterPacket(string clientId, string clientName)
        {
            // Create a REGISTER packet and serialize it
            DataPacket packet = new(clientId, clientName, nameof(ClientDataHeader.Register), "");
            return JsonSerializer.Serialize<DataPacket>(packet);
        }

        /// <summary>
        /// Gets a mock "Frame" object.
        /// </summary>
        /// <returns>
        /// A mock "Frame" object
        /// </returns>
        public static Frame GetMockFrame()
        {
            // Create mock Coordinates object
            Coordinates coordinates = new() { X = rand.Next(1, 100), Y = rand.Next(1, 100) };

            // Create mock Pixels object
            List<Pixel> pixels = new();
            for (int i = 0; i < 10; ++i)
            {
                // Create mock RGB object
                RGB rgb = new() { R = rand.Next(1, 100), G = rand.Next(1, 100), B = rand.Next(1, 100) };
                pixels.Add(new() { Coordinates = coordinates, RGB = rgb });
            }

            return new() { Resolution = new() { Height = rand.Next(1, 100), Width = rand.Next(1, 100) }, Pixels = pixels };
        }

        /// <summary>
        /// Gets a mock serialized image packet and the mock frame inside that packet.
        /// </summary>
        /// <param name="id">
        /// Id of the client who sends this packet
        /// </param>
        /// <param name="name">
        /// Name of the client who sends this packet
        /// </param>
        /// <returns>
        /// Returns a mock serialized image packet and the mock frame inside that packet
        /// </returns>
        public static (string mockPacket, Frame mockFrame) GetMockImagePacket(string id, string name)
        {
            // Create a mock frame object
            Frame mockFrame = Utils.GetMockFrame();
            string serializedData = JsonSerializer.Serialize<Frame>(mockFrame);
            DataPacket packet = new(id, name, nameof(ClientDataHeader.Image), serializedData);
            return (JsonSerializer.Serialize<DataPacket>(packet), mockFrame);
        }

        /// <summary>
        /// Gets a mock Bitmap image from Unsplash.
        /// </summary>
        /// <returns>
        /// Returns a mock Bitmap image from Unsplash
        /// </returns>
        public static Bitmap GetMockBitmap()
        {
            // Create a WebClient to get the image from the URL
            using WebClient client = new();
            // Image stream read from the URL
            using Stream stream = client.OpenRead($"https://source.unsplash.com/random/400x400?sig={rand.Next() + 1}");
            Bitmap image = new(stream);
            return image;
        }
    }
}
