///<author>Amish Ashish Saxena</author>
///<summary> 
///This file contains the ScreenCapturer Class that implements the
///screen capturing functionality. It is used by ScreenshareClient. 
///</summary>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;

namespace PlexShareScreenshare.Client
{
    /// <summary>
    /// Class contains implementation of the screen capturing using threads (tasks)
    /// </summary>
    internal class ScreenCapturer
    {
        private Queue<Bitmap> _capturedFrame;

        // Limits the number of frames in the queue
        const int MaxQueueLength = 50;

        // Task to capture the screen asynchronously
        private Task _captureTask;

        // Token and its source for killing the task
        private CancellationTokenSource Source;
        private CancellationToken Token;

        ScreenCapturer() 
        {
            _capturedFrame = new Queue<Bitmap>();
            _captureTask = null;
        }

        /// <summary>
        /// Returns the bitmap image at the front of _capturedFrame queue. 
        /// </summary>
        /// <returns>Bitmap image of 720p dimension</returns>
        public Bitmap GetImage() 
        {
            try
            {
                return _capturedFrame.Dequeue();
            }
            catch(Exception e)
            {
                Trace.WriteLine($"[ScreenSharing] Dequeue failed: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Creates the task for capturing screenshots and starts capturing
        /// </summary>
        public void StartCapture()
        {
            Source = new CancellationTokenSource();
            Token = Source.Token;
            
            _captureTask = new Task(() =>
            {
                Screenshot screenshot = new Screenshot();

                while (true)
                {
                    if(_capturedFrame.Count < MaxQueueLength)
                    {
                        lock (_capturedFrame)
                        {
                            try
                            {
                                _capturedFrame.Enqueue(screenshot.MakeScreenshot());
                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine($"[ScreenSharing] Could not capture screenshot: {e.Message}");
                            }
                        }
                    }
                    else
                    {
                        // Sleep for some time, if queue is filled 
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }, Token);

            _captureTask.Start();
        }

        public void ResumeCapture()
        {
            StartCapture();
        }

        public void SuspendCapture()
        {
            StopCapture();
        }

        /// <summary>
        /// Stops the capturing by Cancelling the task and clears the _capturedFrame queue.
        /// </summary>
        public void StopCapture()
        {
            Source.Cancel();
            _capturedFrame.Clear();
            _captureTask = null;
        }
    }
}
