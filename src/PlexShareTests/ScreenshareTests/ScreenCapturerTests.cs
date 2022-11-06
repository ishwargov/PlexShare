using PlexShareScreenshare.Client;
using System.Drawing;

namespace PlexShareTests.ScreenshareTests
{
    public class ScreenCapturerTests
    {
        /// <summary>
        /// Capture for some time and see if the elements of queues are not null
        /// </summary>
        [Fact]
        public void Test1()
        {
            ScreenCapturer screenCapturer = new ScreenCapturer();
            screenCapturer.StartCapture();
            Thread.Sleep(500);
            int count = 0;
            for (int i = 0; i < 10; i++)
            {
                Bitmap frame = screenCapturer.GetImage();
                if (frame != null)
                    count++;
            }

            screenCapturer.StopCapture();
            Assert.Equal(10, count);
        }

        /// <summary>
        /// Queue length should always be between 0 and MaxQueueLength
        /// </summary>
        [Fact]
        public void Test2()
        {
            ScreenCapturer screenCapturer = new ScreenCapturer();
            screenCapturer.StartCapture();

            Thread.Sleep(500);
            int framesCaptured = screenCapturer.GetCapturedFrameLength();

            screenCapturer.StopCapture();
            Assert.True(framesCaptured is > 0 and <= ScreenCapturer.MaxQueueLength);
        }
    }
}
