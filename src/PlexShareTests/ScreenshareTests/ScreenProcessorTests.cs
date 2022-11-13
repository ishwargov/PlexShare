using PlexShareScreenshare;
using PlexShareScreenshare.Client;
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
        public void TestSameImagePixelDiffZero()
        {
            ScreenCapturer screenCapturer = new();
            CancellationTokenSource source = new();

            screenCapturer.StartCapture();
            Bitmap img = screenCapturer.GetImage(source.Token);
            screenCapturer.StopCapture();

            List<Pixel> tmp = ScreenProcessor.ProcessUsingLockbits(img, img);
            Assert.True(tmp.Count == 0);
        }
    }
}
