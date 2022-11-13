using PlexShareScreenshare.Client;
using System.Drawing;

namespace PlexShareTests.ScreenshareTests
{
    [Collection("Sequential")]
    public class ScreenCapturerTests
    {
        /// <summary>
        /// Capture for some time and see if the elements of queues are not null
        /// </summary>
        [Fact]
        public void Test1()
        {
            Task<ScreenCapturer> task = Task.Run(() => { ScreenCapturer screenCapturer = new ScreenCapturer(); return screenCapturer; });
            task.Wait();
            var screenCapturer = task.Result;
            screenCapturer.StartCapture();
            Thread.Sleep(1000);
            int count = 0;
            CancellationTokenSource source = new();
            for (int i = 0; i < 50; i++)
            {
                Bitmap frame = screenCapturer.GetImage(source.Token);
                if (frame != null)
                    count++;
            }

            screenCapturer.StopCapture();
            Assert.Equal(50, count);
        }

        /// <summary>
        /// Queue length should always be between 0 and MaxQueueLength
        /// </summary>
        [Fact]
        public void Test2()
        {
            Task<ScreenCapturer> task = Task.Run(() => { ScreenCapturer screenCapturer = new ScreenCapturer(); return screenCapturer; });
            task.Wait();
            var screenCapturer = task.Result;
            screenCapturer.StartCapture();
            Console.WriteLine("Hello");
            Thread.Sleep(1000);
            int framesCaptured = screenCapturer.GetCapturedFrameLength();

            screenCapturer.StopCapture();
            Thread.Sleep(1);
            Assert.True(framesCaptured is > 0 and <= ScreenCapturer.MaxQueueLength);
        }
    }
}
