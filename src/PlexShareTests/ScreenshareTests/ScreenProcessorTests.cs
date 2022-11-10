using PlexShareScreenshare.Client;
using PlexShareScreenshare;
using PlexShareTests.ScreenshareTests;
using System.Diagnostics;
using System.Drawing;

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

            screenProcessor.StartProcessing();
            screenCapturer.StartCapture();

            Thread.Sleep(500);

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
