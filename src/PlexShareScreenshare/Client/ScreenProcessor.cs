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
using System.Runtime.InteropServices;
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
        private readonly Queue<Frame> _processedFrame;

        // Processing task
        private Task? _processorTask;

        // The screen capturer object
        private readonly ScreenCapturer _capturer;

        // Current and the new resolutions 
        private Resolution CurrentRes;
        private Resolution NewRes;
        public readonly Object ResolutionLock;

        // Height and Width of the images captured by the capturer
        int CapturedImageHeight;
        int CapturedImageWidth;

        // Tokens added to be able to stop the thread execution
        private CancellationTokenSource? _cancellationTokenSource;

        // Storing the previous frame
        Bitmap? prevImage;

        /// <summary>
        /// Called by ScreenshareClient.
        /// Initializes queue, oldRes, newRes, cancellation token and the previous image.
        /// </summary>
        public ScreenProcessor(ScreenCapturer Capturer)
        {
            _capturer = Capturer;
            _processedFrame = new Queue<Frame>();
            ResolutionLock = new();
            Trace.WriteLine(Utils.GetDebugMessage("Successfully created an instance of ScreenProcessor", withTimeStamp: true));
        }

        /// <summary>
        /// Pops and return the image from the queue
        /// </summary>
        public Frame GetFrame(CancellationToken token)
        {
            while (_processedFrame.Count == 0 && !token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
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
        public static List<Pixel> ProcessUsingLockbits(Bitmap processedBitmap, Bitmap processedBitmap1)
        {
            // List for storing the difference in pixels
            List<Pixel> tmp = new();
            int count = 0;
            // Getting BitmapData from the Bitmap of first image
            // by locking the bits
            BitmapData bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite, processedBitmap.PixelFormat);

            // Flattening of image into an array
            int bytesPerPixel = Bitmap.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * processedBitmap.Height;
            byte[] pixels = new byte[byteCount];
            IntPtr ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;
            processedBitmap.UnlockBits(bitmapData);

            // Getting BitmapData from the Bitmap of second image
            BitmapData bitmapData1 = processedBitmap1.LockBits(new Rectangle(0, 0, processedBitmap1.Width, processedBitmap1.Height), ImageLockMode.ReadWrite, processedBitmap1.PixelFormat);

            // Flattening of image into an array
            int byteCount1 = bitmapData1.Stride * processedBitmap1.Height;
            byte[] pixels1 = new byte[byteCount1];
            IntPtr ptrFirstPixel1 = bitmapData1.Scan0;
            Marshal.Copy(ptrFirstPixel1, pixels1, 0, pixels1.Length);
            processedBitmap1.UnlockBits(bitmapData1);

            // Now iterating over the image array and checking the difference 
            // in pixel values
            for (int y = 0; y < heightInPixels; y++)
            {
                int currentLine = y * bitmapData.Stride;
                int currentLine1 = y * bitmapData1.Stride;
                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    // getting the color values from the two images
                    int oldBlue = pixels[currentLine + x];
                    int oldGreen = pixels[currentLine + x + 1];
                    int oldRed = pixels[currentLine + x + 2];

                    int newBlue = pixels1[currentLine1 + x];
                    int newGreen = pixels1[currentLine1 + x + 1];
                    int newRed = pixels1[currentLine1 + x + 2];

                    // now if anyone of them is different then start save this pixel 
                    // coordinates and the RGB value of the second image
                    if (oldBlue != newBlue || oldGreen != newGreen || oldRed != newRed)
                    {
                        Coordinates coordinates = new Coordinates() { X = x / bytesPerPixel, Y = y };
                        RGB rgb = new RGB() { R = newRed, G = newGreen, B = newBlue };
                        Pixel tmpVal = new Pixel() { Coordinates = coordinates, RGB = rgb };
                        tmp.Add(tmpVal);
                        count++;
                    }
                }
            }
            // returning these pixel details
            return tmp;
        }

        /// <summary>
        /// Main function which will run in loop and capture the image
        /// calculate the image bits differences and append it in the array
        /// </summary>
        private void Processing()
        {
            _cancellationTokenSource!.Token.ThrowIfCancellationRequested();
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                Bitmap img = _capturer.GetImage(_cancellationTokenSource.Token);
                img = Compress(img);
                List<Pixel> DiffList = ProcessUsingLockbits(prevImage!, img);
                lock (_processedFrame)
                {
                    _processedFrame.Enqueue(new Frame() { Resolution = NewRes, Pixels = DiffList });
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
            _cancellationTokenSource = new();
            Bitmap? img = null;
            try
            {
                img = _capturer.GetImage(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Processor task cancelled: {e.Message}", withTimeStamp: true));
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
                _processorTask = new Task(Processing, _cancellationTokenSource.Token);
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
        public async void StopProcessing()
        {
            Debug.Assert(_processorTask != null, Utils.GetDebugMessage("_processorTask was null, cannot call cancel."));
            Debug.Assert(_cancellationTokenSource != null, Utils.GetDebugMessage("_cancellationTokenSource was null, cannot call cancel."));

            try
            {
                _cancellationTokenSource.Cancel();
            }
            catch (OperationCanceledException e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Processor task cancelled: {e.Message}", withTimeStamp: true));
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to cancel processor task: {e.Message}", withTimeStamp: true));
            }

            await _processorTask;

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

        /// <summary>
        /// Called by StartProcessing if the image resolution has changed then set
        /// the new image resolution and inititalise prevImage variable.
        /// </summary>
        public Bitmap Compress(Bitmap img)
        {
            lock (ResolutionLock)
            {
                if (NewRes != CurrentRes)
                {
                    prevImage = new Bitmap(NewRes.Height, NewRes.Width);
                    CurrentRes = NewRes;
                }
            }
            img = new Bitmap(img, NewRes.Height, NewRes.Width);
            return img;
        }
    }
}
