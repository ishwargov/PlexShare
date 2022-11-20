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
        private static readonly Random RandomGenerator = new(DateTime.Now.Second);

        private static readonly int MaxFrameDimension = 400;

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
        public static SharedClientScreen GetMockClient(ITimerManager server, bool isDebugging = false, int id = -1)
        {
            // Generate a random client Id if not given
            string clientId = (id == -1) ? RandomGenerator.Next().ToString() : id.ToString();
            return new(clientId, RandomGenerator.Next().ToString(), server, isDebugging);
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
        public static List<SharedClientScreen> GetMockClients(ITimerManager server, int count, bool isDebugging = false)
        {
            List<SharedClientScreen> list = new();
            for (int i = 2; i < count + 2; ++i)
            {
                list.Add(Utils.GetMockClient(server, isDebugging));
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
        public static Frame GetMockFrame(int height = -1, int width = -1)
        {
            int maxColor = 256;
            // Create mock Coordinates object
            int x = RandomGenerator.Next(1, MaxFrameDimension), y = RandomGenerator.Next(1, MaxFrameDimension);
            Coordinates coordinates = new() { X = Math.Min(x, y), Y = Math.Max(x, y) };

            // Create mock Pixels object
            List<Pixel> pixels = new();
            for (int i = 0; i < 10; ++i)
            {
                // Create mock RGB object
                RGB rgb = new() { R = RandomGenerator.Next(0, maxColor), G = RandomGenerator.Next(0, maxColor), B = RandomGenerator.Next(0, maxColor) };
                pixels.Add(new() { Coordinates = coordinates, RGB = rgb });
            }

            height = (height != -1) ? height : RandomGenerator.Next(1, MaxFrameDimension);
            width = (width != -1) ? width : RandomGenerator.Next(1, MaxFrameDimension);
            return new() { Resolution = new() { Height = height, Width = width }, Pixels = pixels };
        }

        /// <summary>
        /// Gets a list of mock frame objects all having the same resolution.
        /// </summary>
        /// <param name="count">
        /// Number of mock frames to generate
        /// </param>
        /// <returns></returns>
        public static List<Frame> GetMockFrames(int count = 10)
        {
            List<Frame> frames = new();
            for (int i = 0; i < count; ++i)
            {
                frames.Add(GetMockFrame(MaxFrameDimension, MaxFrameDimension));
            }
            return frames;
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
            using Stream stream = client.OpenRead($"https://source.unsplash.com/random/400x400?sig={RandomGenerator.Next() + 1}");
            Bitmap image = new(stream);
            return image;
        }
    }
}
