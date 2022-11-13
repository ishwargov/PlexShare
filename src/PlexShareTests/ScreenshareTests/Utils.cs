/// <author>Mayank Singla</author>

using PlexShareScreenshare;
using PlexShareScreenshare.Server;
using System.Drawing;
using System.Net;
using System.Text.Json;

namespace PlexShareTests.ScreenshareTests
{
    public static class Utils
    {
        private static readonly Random rand = new(DateTime.Now.Second);

        public static SharedClientScreen GetMockClient(ITimerManager server, int id = -1)
        {
            string clientId = (id == -1) ? rand.Next().ToString() : id.ToString();
            return new(clientId, rand.Next().ToString(), server);
        }

        public static List<SharedClientScreen> GetMockClients(ITimerManager server, int count)
        {
            List<SharedClientScreen> list = new();
            for (int i = 2; i < count + 2; ++i)
            {
                list.Add(Utils.GetMockClient(server, i));
            }
            return list;
        }

        public static string GetMockRegisterPacket(string clientId, string clientName)
        {
            DataPacket packet = new(clientId, clientName, nameof(ClientDataHeader.Register), "");
            return JsonSerializer.Serialize<DataPacket>(packet);
        }

        public static Frame GetMockFrame()
        {
            Coordinates coordinates = new() { X = rand.Next(1, 100), Y = rand.Next(1, 100) };

            List<Pixel> pixels = new();
            for (int i = 0; i < 10; ++i)
            {
                RGB rgb = new() { R = rand.Next(1, 100), G = rand.Next(1, 100), B = rand.Next(1, 100) };
                pixels.Add(new() { Coordinates = coordinates, RGB = rgb });
            }

            return new() { Resolution = new() { Height = rand.Next(1, 100), Width = rand.Next(1, 100) }, Pixels = pixels };
        }

        public static (string mockPacket, Frame mockFrame) GetMockImagePacket(string id, string name)
        {
            Frame mockFrame = Utils.GetMockFrame();
            string serializedData = JsonSerializer.Serialize<Frame>(mockFrame);
            DataPacket packet = new(id, name, nameof(ClientDataHeader.Image), serializedData);
            return (JsonSerializer.Serialize<DataPacket>(packet), mockFrame);
        }

        public static Bitmap GetMockBitmap()
        {

            using WebClient client = new();
            using Stream stream = client.OpenRead($"https://source.unsplash.com/random/400x400?sig={rand.Next() + 1}");
            Bitmap image = new(stream);
            return image;
        }
    }
}
