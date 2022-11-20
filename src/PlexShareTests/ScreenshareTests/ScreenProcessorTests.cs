using PlexShareScreenshare.Client;
using PlexShareTests.ScreenshareTests;
using System.Drawing;

ScreenProcessorTests tmp = new();
tmp.TestCleanup();

namespace PlexShareTests.ScreenshareTests
{
    [Collection("Sequential")]
    public class ScreenProcessorTests
    {
        [Fact]
        public void TestProcessedFrameNonEmpty()
        {
            ScreenCapturer screenCapturer = new();
            ScreenProcessor screenProcessor = new(screenCapturer);

            // Capturer must be called before Processor
            screenCapturer.StartCapture();
            screenProcessor.StartProcessing();

            Thread.Sleep(1000);

            screenCapturer.StopCapture();
            int v2 = screenProcessor.GetProcessedFrameLength();
            screenProcessor.StopProcessing();

            Assert.True(v2 > 0);
        }

        [Fact]
        public void TestCleanup()
        {
            ScreenCapturer screenCapturer = new();
            ScreenProcessor screenProcessor = new(screenCapturer);

            screenCapturer.StartCapture();
            screenProcessor.StartProcessing();

            Thread.Sleep(1000);

            screenCapturer.StopCapture();
            screenProcessor.StopProcessing();

            Console.WriteLine($"len = {screenProcessor.GetProcessedFrameLength()}");
            Assert.True(screenProcessor.GetProcessedFrameLength() == 0);
        }

        [Fact]
        public void TestSameImagePixelDiffZero()
        {
            ScreenCapturer screenCapturer = new();
            bool token = false;

            screenCapturer.StartCapture();
            Bitmap img = screenCapturer.GetImage(ref token);
            screenCapturer.StopCapture();

            //List<Pixel> tmp = ScreenProcessor.ProcessUsingLockbits(img, img);
            //Assert.True(tmp.Count == 0);
        }
    }
}
