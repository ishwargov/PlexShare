///<author>Amish Ashish Saxena</author>
///<summary> 
///This file contains the ScreenCapturer Class that implements the
///screen capturing functionality. It is used by ScreenshareClient. 
///</summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace PlexShareScreenshare.Client
{
    /// <summary>
    /// Class contains implementation of the screen capturing using threads (tasks)
    /// </summary>
    public class ScreenCapturer
    {
        readonly Queue<Bitmap> _capturedFrame;

        // Limits the number of frames in the queue
        public const int MaxQueueLength = 50;

        // Token and its source for killing the task
        private CancellationTokenSource? Source;

        private Task? _captureTask;

        public ScreenCapturer()
        {
            _capturedFrame = new Queue<Bitmap>();
        }

        /// <summary>
        /// Returns the bitmap image at the front of _capturedFrame queue. 
        /// </summary>
        /// <returns>Bitmap image of 720p dimension</returns>
        public Bitmap GetImage()
        {
            while (_capturedFrame.Count == 0)
            {
                Thread.Sleep(100);
            }

            // TODO : check if lock is freed after returning
            lock (_capturedFrame)
            {
                return _capturedFrame.Dequeue();
            }
        }

        /// <summary>
        /// Returns the length of the _capturedFrame queue
        /// </summary>
        /// <returns>Integer value containing the length of queue.</returns>
        public int GetCapturedFrameLength()
        {
            lock (_capturedFrame)
            {
                return _capturedFrame.Count;
            }
        }

        /// <summary>
        /// Creates the task for capturing screenshots and starts capturing
        /// </summary>
        public void StartCapture()
        {
            Source = new CancellationTokenSource();
            CancellationToken Token = Source.Token;

            _captureTask = new Task(() =>
            {
                Screenshot screenshot = Screenshot.Instance();

                while (!Source.IsCancellationRequested)
                {
                    if (_capturedFrame.Count < MaxQueueLength)
                    {
                        lock (_capturedFrame)
                        {
                            try
                            {
                                Bitmap img = screenshot.MakeScreenshot();
                                if (img != null)
                                    _capturedFrame.Enqueue(img);
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
                        Thread.Sleep(100);
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
            Source?.Cancel();
            _captureTask?.Wait();
            lock (_capturedFrame)
            {
                _capturedFrame.Clear();
            }
        }
    }
}
