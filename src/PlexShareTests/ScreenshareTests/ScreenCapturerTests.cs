using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using PlexShareScreenshare.Client;

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
            screenCapturer.StopCapture();

            int count = 0;
            for (int i = 0; i < 10; i++)
            {
                Bitmap frame = screenCapturer.GetImage();
                if (frame != null)
                    count++;
            }
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
            screenCapturer.StopCapture();

            Assert.True(screenCapturer.GetCapturedFrameLength() is >= 0 and <= ScreenCapturer.MaxQueueLength);
        }
    }
}
