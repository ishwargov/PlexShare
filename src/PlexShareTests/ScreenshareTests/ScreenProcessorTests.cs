using PlexShareTests.ScreenshareTests;

ScreenProcessorTests tmp = new();
tmp.Test1();

namespace PlexShareTests.ScreenshareTests
{
    public class ScreenProcessorTests
    {
        public void Test1()
        {
            Serializer serializer = new Serializer();
            Resolution res = new Resolution() { Height = 100, Width = 200 };
            Coordinates cor = new() { X = 10, Y = 20 };
            RGB rgb = new() { R = 1, G = 2, B = 3 };
            Pixel p = new() { Coordinates = cor, RGB = rgb };
            DataPacket pkt = new("1", "2", "name", "data");

            //string jsonString = JsonSerializer.Serialize<DataPacket>(pkt);
            string tmp = serializer.Serialize(pkt);
            Console.WriteLine(tmp);
            //pkt = serializer.Deserialize<DataPacket>(tmp);
            //pkt =
            //JsonSerializer.Deserialize<DataPacket>(jsonString);

            //Console.WriteLine($"{pkt.Id}, {pkt.Name},  hhhg");
            //Console.WriteLine("r = {0}, g = {1}, b = {2}", p.RGB.R, p.RGB.G, p.RGB.B);
            //Console.WriteLine("x = {0}, y = {1}", p.Coordinates.X, p.Coordinates.Y);
        }

        [Fact]
        public void TestProcessedFrameNonEmpty()
        {
            ScreenCapturer screenCapturer = new();
            ScreenProcessor screenProcessor = new(screenCapturer);

            screenProcessor.StartProcessing();
            screenCapturer.StartCapture();

            Thread.Sleep(1000);

            screenCapturer.StopCapture();
            int v2 = screenProcessor.GetProcessedFrameLength();
            screenProcessor.StopProcessing();

            Assert.True(v2 > 0);
        }

        [Fact]
        public void TestSameImagePixelDiffZero()
        {
            ScreenCapturer screenCapturer = new();

            screenCapturer.StartCapture();
            Bitmap img = screenCapturer.GetImage();
            screenCapturer.StopCapture();

            List<Pixel> tmp = ScreenProcessor.ProcessUsingLockbits(img, img);
            Assert.True(tmp.Count == 0);
        }
    }
}
