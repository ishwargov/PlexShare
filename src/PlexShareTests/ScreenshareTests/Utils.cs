/// <author>Mayank Singla</author>
/// <summary>
/// Defines the static "Utils" class which defines
/// general utilities required for unit tests.
/// </summary>

using PlexShareScreenshare;
using PlexShareScreenshare.Server;
using System.Drawing;
using System.Drawing.Imaging;
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
        private static Random RandomGenerator { get; } = new(DateTime.Now.Second);

        /// <summary>
        /// The maximum dimension of a frame.
        /// </summary>
        private static int MaxFrameDimension { get; } = 400;

        /// <summary>
        /// Gets a mock instance for "SharedClientScreen" with given client Id (if provided).
        /// </summary>
        /// <param name="server">
        /// The server object required by the client object.
        /// </param>
        /// <param name="isDebugging">
        /// If we are in debugging mode.
        /// </param>
        /// <param name="id">
        /// Id of the client to bind to.
        /// </param>
        /// <returns>
        /// A mock instance for "SharedclientScreen".
        /// </returns>
        public static SharedClientScreen GetMockClient(ITimerManager server, bool isDebugging = false, int id = -1)
        {
            // Generate a random client Id if not given.
            string clientId = (id == -1) ? Utils.RandomGenerator.Next().ToString() : id.ToString();
            return new(clientId, Utils.RandomGenerator.Next().ToString(), server, isDebugging);
        }

        /// <summary>
        /// Get a list of mock "SharedClientScreen".
        /// </summary>
        /// <param name="server">
        /// The server object required by the client object.
        /// </param>
        /// <param name="count">
        /// Number of mock clients.
        /// </param>
        /// <param name="isDebugging">
        /// If we are in debugging mode.
        /// </param>
        /// <returns>
        /// A list of mock objects of "SharedClientScreen".
        /// </returns>
        public static List<SharedClientScreen> GetMockClients(ITimerManager server, int count, bool isDebugging = false)
        {
            List<SharedClientScreen> list = new();
            for (int i = 2; i < count + 2; ++i)
            {
                list.Add(Utils.GetMockClient(server, isDebugging, i));
            }
            return list;
        }

        /// <summary>
        /// Gets a mock serialized "REGISTER" packet received by screen share server.
        /// </summary>
        /// <param name="clientId">
        /// Id of the client who sends this packet.
        /// </param>
        /// <param name="clientName">
        /// Name of the client who sends this packet.
        /// </param>
        /// <returns>
        /// A mock serialized "REGISTER" packet received by screen share server.
        /// </returns>
        public static string GetMockRegisterPacket(string clientId, string clientName)
        {
            // Create a REGISTER packet with empty data and serialize it.
            DataPacket packet = new(clientId, clientName, nameof(ClientDataHeader.Register), "");
            return JsonSerializer.Serialize<DataPacket>(packet);
        }

        /// <summary>
        /// Gets a mock serialized "DEREGISTER" packet received by screen share server.
        /// </summary>
        /// <param name="clientId">
        /// Id of the client who sends this packet.
        /// </param>
        /// <param name="clientName">
        /// Name of the client who sends this packet.
        /// </param>
        /// <returns>
        /// A mock serialized "DEREGISTER" packet received by screen share server.
        /// </returns>
        public static string GetMockDeregisterPacket(string clientId, string clientName)
        {
            // Create a DEREGISTER packet with no data and serialize it.
            DataPacket packet = new(clientId, clientName, nameof(ClientDataHeader.Deregister), "");
            return JsonSerializer.Serialize<DataPacket>(packet);
        }

        /// <summary>
        /// Gets a mock serialized "CONFIRMATION" packet received by screen share server.
        /// </summary>
        /// <param name="clientId">
        /// Id of the client who sends this packet.
        /// </param>
        /// <param name="clientName">
        /// Name of the client who sends this packet.
        /// </param>
        /// <returns>
        /// A mock serialized "CONFIRMATION" packet received by screen share server.
        /// </returns>
        public static string GetMockConfirmationPacket(string clientId, string clientName)
        {
            // Create a CONFIRMATION packet with no data and serialize it.
            DataPacket packet = new(clientId, clientName, nameof(ClientDataHeader.Confirmation), "");
            return JsonSerializer.Serialize<DataPacket>(packet);
        }

        /// <summary>
        /// Gets a mock "Frame" object.
        /// </summary>
        /// <param name="height">
        /// Height of the frame.
        /// </param>
        /// <param name="width">
        /// Width of the frame.
        /// </param>
        /// <returns>
        /// A mock "Frame" object.
        /// </returns>
        public static Frame GetMockFrame(int height = -1, int width = -1)
        {
            int maxColor = 256;
            // Create mock Coordinates object.
            int x = Utils.RandomGenerator.Next(1, Utils.MaxFrameDimension), y = Utils.RandomGenerator.Next(1, Utils.MaxFrameDimension);
            Coordinates coordinates = new() { X = Math.Min(x, y), Y = Math.Max(x, y) };

            // Create mock Pixels object.
            List<Pixel> pixels = new();
            for (int i = 0; i < 10; ++i)
            {
                // Create mock RGB object.
                RGB rgb = new() { R = Utils.RandomGenerator.Next(0, maxColor), G = Utils.RandomGenerator.Next(0, maxColor), B = Utils.RandomGenerator.Next(0, maxColor) };
                pixels.Add(new() { Coordinates = coordinates, RGB = rgb });
            }

            height = (height != -1) ? height : Utils.RandomGenerator.Next(1, Utils.MaxFrameDimension);
            width = (width != -1) ? width : Utils.RandomGenerator.Next(1, Utils.MaxFrameDimension);
            return new() { Resolution = new() { Height = height, Width = width }, Pixels = pixels };
        }

        /// <summary>
        /// Gets a list of mock frame objects all having the same resolution.
        /// </summary>
        /// <param name="count">
        /// Number of mock frames to generate.
        /// </param>
        /// <returns>
        /// List of mock frames.
        /// </returns>
        public static List<Frame> GetMockFrames(int count = 10)
        {
            List<Frame> frames = new();
            for (int i = 0; i < count; ++i)
            {
                frames.Add(Utils.GetMockFrame(Utils.MaxFrameDimension, Utils.MaxFrameDimension));
            }
            return frames;
        }

        /// <summary>
        /// Gets a mock serialized image packet and the mock image inside that packet.
        /// </summary>
        /// <param name="id">
        /// Id of the client who sends this packet.
        /// </param>
        /// <param name="name">
        /// Name of the client who sends this packet.
        /// </param>
        /// <returns>
        /// Returns a mock serialized image packet and the mock image inside that packet.
        /// </returns>
        public static (string mockPacket, string mockImage) GetMockImagePacket(string id, string name)
        {
            // Create a mock received image.
            string mockImage = Utils.GetMockImage();
            DataPacket packet = new(id, name, nameof(ClientDataHeader.Image), mockImage);
            return (JsonSerializer.Serialize<DataPacket>(packet), mockImage);
        }

        /// <summary>
        /// Gets a mock Bitmap image from Unsplash.
        /// </summary>
        /// <returns>
        /// Returns a mock Bitmap image from Unsplash.
        /// </returns>
        public static Bitmap GetMockBitmap()
        {
            // Create a WebClient to get the image from the URL.
            using WebClient client = new();
            // Image stream read from the URL.
            using Stream stream = client.OpenRead($"https://source.unsplash.com/random/400x400?sig={Utils.RandomGenerator.Next() + 1}");
            Bitmap image = new(stream);
            return image;
        }

        /// <summary>
        /// Generates a mock received image from the client.
        /// </summary>
        /// <returns>
        /// The generated mock image.
        /// </returns>
        public static string GetMockImage()
        {
            // Create a mock bitmap image and convert it to base-64 string.
            Bitmap img = Utils.GetMockBitmap();
            MemoryStream ms = new();
            img.Save(ms, ImageFormat.Jpeg);
            return Convert.ToBase64String(ms.ToArray()) + "1";
        }

        /// <summary>
        /// Generates a random alphanumerical string.
        /// </summary>
        /// <param name="length">
        /// Length of the random string to generate.
        /// </param>
        /// <returns>
        /// The generated random alphanumerical string.
        /// </returns>
        public static string RandomString(int length)
        {
            // Pick a random alphanumeric character every time.
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Utils.RandomGenerator.Next(s.Length)]).ToArray());
        }
    }
}
