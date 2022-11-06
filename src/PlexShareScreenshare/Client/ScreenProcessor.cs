///<author>Satyam Mishra</author>
///<summary>
/// This file has ScreenProcessor class. It is responsible for 
/// processing the image from ScreenCapturer class and calculating
/// the image bits that are different from the previous image
///</summary>
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
// Each frame consists of the resolution of the image and the ImageDiffList
using Frame = System.Tuple<System.Tuple<int, int>,
                        System.Collections.Generic.List<System.Tuple<System.Tuple<int, int>,
                        System.Tuple<int, int, int>>>>;
// This datatype is for storing a list of the coordinate
// of pixel and its RGB values i.e. list of ((x, y), (R, G, B))
using ImageDiffList = System.Collections.Generic.List<System.Tuple<System.Tuple<int, int>,
                        System.Tuple<int, int, int>>>;

namespace PlexShareScreenshare.Client
{
    /// <summary>
    /// Class contains implementation of the screen processing using threads (tasks)
    /// </summary>
    internal class ScreenProcessor
    {
        // The queue in which the image will be enqueued after
        // processing it
        private readonly Queue<Frame> _processedFrame;

        // Processing task
        private Task? _processorTask;

        // The screen capturer object
        private readonly ScreenCapturer _capturer;

        // Old and the new resolutions 
        private Tuple<int, int> OldRes;
        public Tuple<int, int> NewRes { private get; set; }

        // Tokens added to be able to stop the thread execution
        private CancellationTokenSource? tokenSource;

        // Storing the previous frame
        Bitmap prevImage;

        /// <summary>
        /// Called by ScreenshareClient
        /// Initialize queue, oldRes, newRes,
        /// cancellation token and the previous image
        /// </summary>
        public ScreenProcessor(ScreenCapturer Capturer)
        {
            this._capturer = Capturer;
            _processedFrame = new Queue<Frame>();
            OldRes = new Tuple<int, int>(720, 1280);
            NewRes = new Tuple<int, int>(720, 1280);
            prevImage = new Bitmap(720, 1280);
        }

        /// <summary>
        /// Pops and return the image from the queue
        /// </summary>
        public Frame GetImage()
        {
            while (_processedFrame.Count != 0) Thread.Sleep(100);
            lock (_processedFrame)
            {
                return _processedFrame.Dequeue();
            }
        }
        /// <summary>
        /// In this function we go through every pixel of both the images and
        /// returns list of those pixels which are different in both the images
        /// </summary>
        private static ImageDiffList ProcessUsingLockbits(Bitmap processedBitmap, Bitmap processedBitmap1)
        {
            // List for storing the difference in pixels
            ImageDiffList tmp = new();
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
            // int bytesPerPixel1 = Bitmap.GetPixelFormatSize(processedBitmap1.PixelFormat) / 8;
            int byteCount1 = bitmapData1.Stride * processedBitmap1.Height;
            byte[] pixels1 = new byte[byteCount1];
            IntPtr ptrFirstPixel1 = bitmapData1.Scan0;
            Marshal.Copy(ptrFirstPixel1, pixels1, 0, pixels1.Length);
            // int heightInPixels1 = bitmapData1.Height;
            // int widthInBytes1 = bitmapData1.Width * bytesPerPixel1;
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
                        Tuple<int, int> coordinates = new(x / bytesPerPixel, y);
                        Tuple<int, int, int> colors = new(newRed, newGreen, newBlue);
                        tmp.Add(new Tuple<Tuple<int, int>, Tuple<int, int, int>>(coordinates, colors));
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
            while (true)
            {
                Bitmap img = _capturer.GetImage();
                img = Compress(img);
                ImageDiffList DiffList = ProcessUsingLockbits(prevImage, img);
                lock (_processedFrame)
                {
                    _processedFrame.Enqueue(new Frame(NewRes, DiffList));
                }
                prevImage = img;
            }
        }

        /// <summary>
        /// Called by ScreenshareClient when the client starts screen sharing
        /// Will have a lambda function - Process and pushes to the queue
        /// Create the task for the lambda function 
        /// </summary>
        public void StartProcessing()
        {
            tokenSource = new CancellationTokenSource();
            CancellationToken processingToken = tokenSource.Token;
            _processorTask = new Task(Processing, processingToken);
            _processorTask.Start();
        }

        /// <summary>
        /// Called when the server asks to stop
        /// Kill the task
        /// Empty the queue
        /// </summary>
        public void SuspendProcessing()
        {
            StopProcessing();
        }

        /// <summary>
        /// Called when the server asks to send
        /// Resume the thread
        /// </summary>
        public void ResumeProcessing()
        {
            StartProcessing();
        }

        /// <summary>
        /// Called by ScreenshareClient when the client stops screen sharing
        /// kill the processor task and make the processor task variable null
        /// Empty the Queue
        /// </summary>
        public void StopProcessing()
        {
            tokenSource?.Cancel();
            _processedFrame.Clear();
        }

        /// <summary>
        /// Setting new resolution for sending the image 
        /// </summary>
        /// <param name="res"> New resolution values </param>
        public void SetNewResolution(Tuple<int, int> res)
        {
            // taking lock since newres is shared variable as it is
            // used even in Compress function
            lock (NewRes)
            {
                NewRes = res;
            }
        }

        /// <summary>
        /// Called by StartProcessing
        /// if the image resolution has changed then set the 
        /// new image resolution and inititalise prevImage variable
        /// </summary>
        public Bitmap Compress(Bitmap img)
        {
            lock (NewRes)
            {
                if (NewRes != OldRes)
                {
                    prevImage = new Bitmap(NewRes.Item1, NewRes.Item2);
                    OldRes = NewRes;
                }
            }
            img = new Bitmap(img, NewRes.Item1, NewRes.Item2);
            return img;
        }
    }
}
