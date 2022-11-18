///<author>Satyam Mishra</author>
///<summary>
/// This file has ScreenProcessor class. It is responsible for 
/// processing the image from ScreenCapturer class and calculating
/// the image bits that are different from the previous image
///</summary>

using K4os.Compression.LZ4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.Json;
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
        private CancellationTokenSource? _cancellationTokenSource;

        // Storing the previous frame
        Bitmap? prevImage;

        private byte[] _compressionBuffer;
        private byte[] _compressedBuffer;

        /// <summary>
        /// Called by ScreenshareClient.
        /// Initializes queue, oldRes, newRes, cancellation token and the previous image.
        /// </summary>
        public ScreenProcessor(ScreenCapturer Capturer)
        {
            _capturer = Capturer;
            _processedFrame = new Queue<string>();
            ResolutionLock = new();

            _compressionBuffer = new byte[1280 * 720 * 4];
            _compressedBuffer = new byte[LZ4Codec.MaximumOutputSize(this._compressionBuffer.Length) + 4];

            Trace.WriteLine(Utils.GetDebugMessage("Successfully created an instance of ScreenProcessor", withTimeStamp: true));
        }

        /// <summary>
        /// Pops and return the image from the queue
        /// </summary>
        public string GetFrame(CancellationToken token)
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
        private unsafe Bitmap Process(Bitmap curr, Bitmap prev)
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
                        diff++;
                }
            }

            curr.UnlockBits(currData);
            prev.UnlockBits(prevData);
            newb.UnlockBits(bmd);

            if (diff >= 500) return null;
            return newb;
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
        public async Task StopProcessing()
        {
            Debug.Assert(_processorTask != null, Utils.GetDebugMessage("_processorTask was null, cannot call cancel."));
            Debug.Assert(_cancellationTokenSource != null, Utils.GetDebugMessage("_cancellationTokenSource was null, cannot call cancel."));

            try
            {
                _cancellationTokenSource.Cancel();
                await _processorTask;
            }
            catch (OperationCanceledException e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Processor task cancelled: {e.Message}", withTimeStamp: true));
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

        /// <summary>
        /// Called by StartProcessing if the image resolution has changed then set
        /// the new image resolution and inititalise prevImage variable.
        /// </summary>
        public string Compress(Bitmap img)
        {
            Bitmap? new_img = null;

            lock (ResolutionLock)
            {
                // TODO: change this later
                //if (prevImage != null && NewRes == CurrentRes)
                //{
                //new_img = Process(img, prevImage);
                //}
                new_img = null;
            }

            if (new_img == null)
            {
                MemoryStream ms = new();
                img.Save(ms, ImageFormat.Jpeg);
                return Convert.ToBase64String(ms.ToArray()) + "1";
            }
            else
            {
                MemoryStream ms = new();
                new_img.Save(ms, ImageFormat.Bmp);
                _compressionBuffer = ms.ToArray();

                int new_sz = LZ4Codec.Encode(
                this._compressionBuffer, 0, this._compressionBuffer.Length,
                _compressedBuffer, 4, _compressedBuffer.Length - 4);

                Buffer.BlockCopy(BitConverter.GetBytes(new_sz), 0, _compressedBuffer, 0, 4);
                byte[] data = new byte[new_sz + 4];
                Array.Copy(_compressedBuffer, 0, data, 0, new_sz + 4);
                return JsonSerializer.Serialize(data) + "0";
            }
        }
    }
}
