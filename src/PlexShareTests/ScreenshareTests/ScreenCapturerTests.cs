///<author>Amish Ashish Saxena</author>
///<summary> 
///This file contains tests for ScreenCapturer Class.  
///</summary>

using PlexShareScreenshare.Client;
using System.Drawing;

namespace PlexShareTests.ScreenshareTests
{
    [Collection("Sequential")]
    public class ScreenCapturerTests
    {
        /// <summary>
        /// Capture for some time and see if the captured images inside the queue are not null.
        /// </summary>
        [Fact]
        public void CorrectImageCaptureTest()
        {
            Task<ScreenCapturer> task = Task.Run(() => { ScreenCapturer screenCapturer = new ScreenCapturer(); return screenCapturer; });
            task.Wait();
            var screenCapturer = task.Result;
            screenCapturer.StartCapture();
            Thread.Sleep(1000);

            int count = 0;
            bool token = false;
            for (int i = 0; i < 50; i++)
            {
                Bitmap frame = screenCapturer.GetImage(ref token);
                if (frame != null)
                    count++;
            }

            screenCapturer.StopCapture();
            Assert.Equal(50, count);
        }

        /// <summary>
        /// Queue length should always be between 0 and MaxQueueLength.
        /// </summary>
        [Fact]
        public void CapturedFrameQueueFunctionality()
        {
            Task<ScreenCapturer> task = Task.Run(() => { ScreenCapturer screenCapturer = new ScreenCapturer(); return screenCapturer; });
            task.Wait();
            var screenCapturer = task.Result;

            screenCapturer.StartCapture();
            Thread.Sleep(1000);
            int framesCaptured = screenCapturer.GetCapturedFrameLength();
            screenCapturer.StopCapture();

            Assert.True(framesCaptured is > 0 and <= ScreenCapturer.MaxQueueLength);
        }

        /// <summary>
        /// Test the control flow where screen ccapturing is stopped once and then restarted and stopped.
        /// This is to simulate a real-life scenario.
        /// </summary>
        [Fact]
        public void MultipleStartStopCapturer()
        {
            Task<ScreenCapturer> task = Task.Run(() => { ScreenCapturer screenCapturer = new ScreenCapturer(); return screenCapturer; });
            task.Wait();
            var screenCapturer = task.Result;

            screenCapturer.StartCapture();
            Thread.Sleep(500);
            screenCapturer.StopCapture();
            Thread.Sleep(100);

            screenCapturer.StartCapture();
            Thread.Sleep(500);
            screenCapturer.StopCapture();

            // If the queue is empty, this means the capturing has been stopped sucessfully. 
            int framesCaptured = screenCapturer.GetCapturedFrameLength();
            Assert.True(framesCaptured == 0);
        }
    }
}
