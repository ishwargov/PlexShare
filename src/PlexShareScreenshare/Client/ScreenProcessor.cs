///<author>Satyam Mishra</author>
///<summary>
/// This file has ScreenProcessor class. It is responsible for 
/// processing the image from ScreenCapturer class and calculating
/// the image bits that are different from the previous image
///</summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace PlexShareScreenshare.Client
{
    /// <summary>
    /// Class contains implementation of the screen processing using threads (tasks)
    /// </summary>
    public class ScreenProcessor
    {
        // The queue in which the image will be enqueued after
        // processing it
        private readonly Queue<string> _processedFrame;

        // Processing task
        private Task? _processorTask;

        // The screen capturer object
        private readonly ScreenCapturer _capturer;

        // Current and the new resolutions 
        private Resolution CurrentRes;
        private Resolution NewRes;
        public readonly object ResolutionLock;

        // Height and Width of the images captured by the capturer
        int CapturedImageHeight;
        int CapturedImageWidth;

        // Tokens added to be able to stop the thread execution
        private bool _cancellationToken;

        // Storing the previous frame
        Bitmap? prevImage;

        /// <summary>
        /// Called by ScreenshareClient.
        /// Initializes queue, oldRes, newRes, cancellation token and the previous image.
        /// </summary>
        public ScreenProcessor(ScreenCapturer Capturer)
        {
            _capturer = Capturer;
            _processedFrame = new Queue<string>();
            ResolutionLock = new();

            Trace.WriteLine(Utils.GetDebugMessage("Successfully created an instance of ScreenProcessor", withTimeStamp: true));
        }

        /// <summary>
        /// Pops and return the image from the queue
        /// </summary>
        public string GetFrame(ref bool cancellationToken)
        {
            while (_processedFrame.Count == 0)
            {
                if (cancellationToken)
                    return "";
                Thread.Sleep(100);
            }
            lock (_processedFrame)
            {
                Trace.WriteLine(Utils.GetDebugMessage("Successfully sent frame", withTimeStamp: true));
                return _processedFrame.Dequeue();
            }
        }

        public int GetProcessedFrameLength()
        {
            lock (_processedFrame)
            {
                Trace.WriteLine(Utils.GetDebugMessage("Successfully sent frame length", withTimeStamp: true));
                return _processedFrame.Count;
            }
        }

        /// <summary>
        /// In this function we go through every pixel of both the images and
        /// returns list of those pixels which are different in both the images
        /// </summary>
        private unsafe Bitmap? Process(Bitmap curr, Bitmap prev)
        {
            BitmapData currData = curr.LockBits(new Rectangle(0, 0, curr.Width, curr.Height), ImageLockMode.ReadWrite, curr.PixelFormat);
            BitmapData prevData = prev.LockBits(new Rectangle(0, 0, prev.Width, prev.Height), ImageLockMode.ReadWrite, prev.PixelFormat);

            int bytesPerPixel = Bitmap.GetPixelFormatSize(curr.PixelFormat) / 8;
            int heightInPixels = currData.Height;
            int widthInBytes = currData.Width * bytesPerPixel;

            byte* currptr = (byte*)currData.Scan0;
            byte* prevptr = (byte*)prevData.Scan0;

            Bitmap newb = new Bitmap(curr.Width, curr.Height);
            BitmapData bmd = newb.LockBits(new Rectangle(0, 0, 10, 10), System.Drawing.Imaging.ImageLockMode.ReadOnly, newb.PixelFormat);
            byte* ptr = (byte*)bmd.Scan0;

            int diff = 0;

            for (int y = 0; y < heightInPixels; y++)
            {
                int currentLine = y * currData.Stride;

                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    int oldBlue = currptr[currentLine + x];
                    int oldGreen = currptr[currentLine + x + 1];
                    int oldRed = currptr[currentLine + x + 2];
                    int oldAlpha = currptr[currentLine + x + 3];

                    int newBlue = prevptr[currentLine + x];
                    int newGreen = prevptr[currentLine + x + 1];
                    int newRed = prevptr[currentLine + x + 2];
                    int newAlpha = prevptr[currentLine + x + 3];

                    ptr[currentLine + x] = (byte)(oldBlue ^ newBlue);
                    ptr[currentLine + x + 1] = (byte)(oldGreen ^ newGreen);
                    ptr[currentLine + x + 2] = (byte)(oldRed ^ newRed);
                    ptr[currentLine + x + 3] = (byte)(oldAlpha ^ newAlpha);

                    if ((oldBlue != newBlue) || (oldGreen != newGreen) || (oldRed != newRed) || (oldAlpha != newAlpha))
                    {
                        diff++;
                        if (diff > 5000)
                        {
                            curr.UnlockBits(currData);
                            prev.UnlockBits(prevData);
                            newb.UnlockBits(bmd);
                            return null;
                        }
                    }
                }
            }

            curr.UnlockBits(currData);
            prev.UnlockBits(prevData);
            newb.UnlockBits(bmd);

            return newb;
        }

        /// <summary>
        /// Main function which will run in loop and capture the image
        /// calculate the image bits differences and append it in the array
        /// </summary>
        private void Processing()
        {
            while (!_cancellationToken)
            {
                Bitmap img = _capturer.GetImage(ref _cancellationToken);
                if (_cancellationToken)
                    break;

                string serialized_buffer = Compress(img);

                lock (_processedFrame)
                {
                    _processedFrame.Enqueue(serialized_buffer);
                }
                prevImage = img;
            }
        }

        /// <summary>
        /// Called by ScreenshareClient when the client starts screen sharing.
        /// Creates a task for the Processing function.
        /// </summary>
        public void StartProcessing()
        {
            // dropping one frame to set the previous image value
            _cancellationToken = false;
            Bitmap? img = null;
            try
            {
                img = _capturer.GetImage(ref _cancellationToken);
                Debug.Assert(!_cancellationToken);
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to cancel processor task: {e.Message}", withTimeStamp: true));
            }

            Debug.Assert(img != null, Utils.GetDebugMessage("img is null"));
            CapturedImageHeight = img.Height;
            CapturedImageWidth = img.Width;

            NewRes = new() { Height = CapturedImageHeight, Width = CapturedImageWidth };
            CurrentRes = NewRes;
            prevImage = new Bitmap(NewRes.Width, NewRes.Height);

            Trace.WriteLine(Utils.GetDebugMessage("Previous image set and" +
                "going to start image processing", withTimeStamp: true));

            try
            {
                _processorTask = new Task(Processing);
                _processorTask.Start();
            }
            catch (OperationCanceledException e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Processor task cancelled: {e.Message}", withTimeStamp: true));
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to cancel processor task: {e.Message}", withTimeStamp: true));
            }
        }

        /// <summary>
        /// Called by ScreenshareClient when the client stops screen sharing
        /// kill the processor task and make the processor task variable null
        /// Empty the Queue.
        /// </summary>
        public void StopProcessing()
        {
            Debug.Assert(_processorTask != null, Utils.GetDebugMessage("_processorTask was null, cannot call cancel."));

            try
            {
                _cancellationToken = true;
                _processorTask.Wait();
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to cancel processor task: {e.Message}", withTimeStamp: true));
            }

            Debug.Assert(_processedFrame != null, Utils.GetDebugMessage("_processedTask is found null"));
            _processedFrame.Clear();

            Trace.WriteLine(Utils.GetDebugMessage("Successfully stopped image processing", withTimeStamp: true));
        }

        /// <summary>
        /// Setting new resolution for sending the image. 
        /// </summary>
        /// <param name="res"> New resolution values </param>
        public void SetNewResolution(int windowCount)
        {
            Debug.Assert(windowCount != 0, Utils.GetDebugMessage("windowCount is found 0"));
            Resolution res = new()
            {
                Height = CapturedImageHeight / windowCount,
                Width = CapturedImageWidth / windowCount
            };
            // taking lock since newres is shared variable as it is
            // used even in Compress function
            lock (ResolutionLock)
            {
                NewRes = res;
            }
            Trace.WriteLine(Utils.GetDebugMessage("Successfully changed the rew resolution" +
                " variable", withTimeStamp: true));
        }

        public static byte[] CompressByteArray(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        public static byte[] DecompressByteArray(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }

        /// <summary>
        /// Called by StartProcessing if the image resolution has changed then set
        /// the new image resolution and inititalise prevImage variable.
        /// </summary>
        public string Compress(Bitmap img)
        {
            Bitmap? new_img = null;

            lock (ResolutionLock)
            {
                if (prevImage != null && NewRes == CurrentRes)
                {
                    new_img = Process(img, prevImage);
                }
                else if (NewRes != CurrentRes)
                {
                    img = new Bitmap(img, NewRes.Width, NewRes.Height);
                    CurrentRes = NewRes;
                }
            }

            if (new_img == null)
            {
                MemoryStream ms = new();
                img.Save(ms, ImageFormat.Bmp);
                var data = CompressByteArray(ms.ToArray());
                return Convert.ToBase64String(data) + "1";
            }
            else
            {
                MemoryStream ms = new();
                new_img.Save(ms, ImageFormat.Bmp);
                var data = CompressByteArray(ms.ToArray());
                return Convert.ToBase64String(data) + "0";
            }
        }
    }
}
