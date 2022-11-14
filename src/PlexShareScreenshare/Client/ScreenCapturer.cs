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
        private CancellationTokenSource? _cancellationTokenSource;

        private Task? _captureTask;

        private readonly Screenshot _screenshot;

        public ScreenCapturer()
        {
            _capturedFrame = new Queue<Bitmap>();
            _screenshot = Screenshot.Instance();
        }

        /// <summary>
        /// Returns the bitmap image at the front of _capturedFrame queue. 
        /// </summary>
        /// <returns>Bitmap image of 720p dimension</returns>
        public Bitmap GetImage(CancellationToken token)
        {
            while (_capturedFrame.Count == 0 && !token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
                Thread.Sleep(100);
            }

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

            _cancellationTokenSource = new();
            _captureTask = new Task(() =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (_capturedFrame.Count < MaxQueueLength)
                    {
                        lock (_capturedFrame)
                        {
                            try
                            {
                                Bitmap img = _screenshot.MakeScreenshot();
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
            }, _cancellationTokenSource.Token);

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
        public async Task StopCapture()
        {
            Debug.Assert(_cancellationTokenSource != null,
                Utils.GetDebugMessage("_cancellationTokenSource is null, cannot stop image capture"));
            Debug.Assert(_captureTask != null,
                Utils.GetDebugMessage("_cancellationTask is null, cannot stop image capture"));
            try
            {
                _cancellationTokenSource.Cancel();
                await _captureTask;
            }
            catch (OperationCanceledException e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Capturer task cancelled: {e.Message}", withTimeStamp: true));
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Unable to stop capture: {e.Message}", withTimeStamp: true));
            }

            _capturedFrame.Clear();
        }
    }
}
