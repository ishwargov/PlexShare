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

namespace PlexShareScreenshare.Client
{
    /// <summary>
    /// Class contains implementation of the screen capturing using threads (tasks)
    /// </summary>
    internal class ScreenCapturer
    {
        private Queue<Bitmap> CapturedFrame;

        // Limits the number of frames in the queue
        const int MaxQueueLength = 50;

        // Task to capture the screen asynchronously
        private Task CaptureTask;

        // Token and its source for killing the task
        private CancellationTokenSource Source;
        private CancellationToken Token;

        ScreenCapturer() 
        {
            CapturedFrame = new Queue<Bitmap>();
            CaptureTask = null;
        }

        // Pops and return the image from the queue
        /// <summary>
        /// Returns the bitmap image at the front of CapturedFrame queue. 
        /// </summary>
        /// <returns>Bitmap image of 720p dimension</returns>
        public Bitmap GetImage() 
        {
            return CapturedFrame.Dequeue(); ;
        }

        /// <summary>
        /// Creates the task for capturing screenshots and starts capturing
        /// </summary>
        public void StartCapture()
        {
            Source = new CancellationTokenSource();
            Token = Source.Token;
            
            CaptureTask = new Task(() =>
            {
                Screenshot screenshot = new Screenshot();
                while(true)
                {
                    if(CapturedFrame.Count < MaxQueueLength)
                    {
                        lock (CapturedFrame)
                        {
                            CapturedFrame.Enqueue(screenshot.MakeScreenshot());
                        }
                    }
                    else
                    {
                        // Sleep for some time, if queue is filled 
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }, Token);

            CaptureTask.Start();
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
        /// Stops the capturing by Cancelling the task and clears the CapturedFrame queue.
        /// </summary>
        public void StopCapture()
        {
            Source.Cancel();
            CapturedFrame.Clear();
            CaptureTask = null;
        }
    }
}
