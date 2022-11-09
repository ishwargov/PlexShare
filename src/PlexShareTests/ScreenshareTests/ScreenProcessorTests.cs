using PlexShareScreenshare.Client;
using PlexShareTests.ScreenshareTests;
using System.Diagnostics;
using System.Drawing;

ScreenProcessorTests tmp = new();
tmp.Test1();

namespace PlexShareTests.ScreenshareTests
{
    public class ScreenProcessorTests
    {
        [Fact]
        public void Test1()
        {
            ScreenCapturer screenCapturer = new();
            ScreenProcessor screenProcessor = new(screenCapturer);

            screenProcessor.StartProcessing();
            screenCapturer.StartCapture();

            Thread.Sleep(10000);

            screenCapturer.StopCapture();
            int v2 = screenProcessor.GetProcessedFrameLength();
            screenProcessor.StopProcessing();

            Assert.True(v2 > 0);
        }

        [Fact]
        public void Test2()
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
