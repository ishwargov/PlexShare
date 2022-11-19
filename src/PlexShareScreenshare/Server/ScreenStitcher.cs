///<author>Aditya Agarwal</author>
///<summary>
///This file contains the ScreenStitcher Class that implements the
///screen stitching functionality. It is used by ScreenshareServer.
///</summary>

using K4os.Compression.LZ4;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;


namespace PlexShareScreenshare.Server
{
    /// <summary>
    /// Class contains implementation of the screen stitching using threads (tasks)
    /// </summary>
    internal class ScreenStitcher
    {
        private readonly SharedClientScreen _sharedClientScreen;

        private Task? _stitchTask;

        private Bitmap? _oldImage;
        private Resolution? _resolution;

        // Token for killing the task
        private bool _cancellationToken = false;

        private byte[] expansionBuffer;

        // Called by the `SharedClientScreen`
        public ScreenStitcher(SharedClientScreen scs)
        {
            _oldImage = null;
            _stitchTask = null;
            _resolution = null;
            _sharedClientScreen = scs;
            expansionBuffer = new byte[720 * 1280 * 4];
        }

        unsafe Bitmap Process(Bitmap curr, Bitmap prev)
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
                }
            }

            curr.UnlockBits(currData);
            prev.UnlockBits(prevData);
            newb.UnlockBits(bmd);

            return newb;
        }


        /// <summary>
        /// Creates(if not exist) and start the task `_stitchTask`
        /// Will read the image using `_sharedClientScreen.GetFrame`
        /// and puts the final image using `_sharedClientScreen.PutFinalImage`
        /// </summary>
        public void StartStitching()
        {
            _cancellationToken = false;

            if (_stitchTask == null)
            {

                _stitchTask = new Task(() =>
                {
                    while (!_cancellationToken)
                    {
                        string? newFrame = _sharedClientScreen.GetImage(ref _cancellationToken);
                        if (_cancellationToken) break;
                        if (newFrame == null)
                        {
                            Trace.WriteLine($"[ScreenSharing] New frame returned by _sharedClientScreen is null.");
                            continue;
                        }
                        Bitmap stichedImage = Stitch(_oldImage, newFrame);
                        _oldImage = stichedImage;
                        _sharedClientScreen.PutFinalImage(stichedImage);
                    }
                });

                _stitchTask.Start();
            }
        }

        // Kills the task `_stitchTask`
        public async void StopStitching()
        {
            try
            {
                _cancellationToken = true;
                await _stitchTask!;
                _stitchTask = null;
            }
            catch (OperationCanceledException e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Task canceled for the client: {e.Message}", withTimeStamp: true));
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to start the processing: {e.Message}", withTimeStamp: true));
            }
            finally
            {
                _stitchTask = null;
            }
        }

        /// <summary>
        /// Function to stitch the new image over the old image
        /// If resolution is changed update whole Image, else update the changed pixels
        /// </summary>
        /// <param name="oldImage">The previous image of client's screen</param>
        /// <param name="newFrame">The list of the updated pixels, list of : { x, y, (R, G, B)}</param>
        /// <returns>The updated new image of client's screen</returns>
        private Bitmap Stitch(Bitmap? oldImage, string newFrame)
        {

            char isCompleteFrame = newFrame[^1];
            newFrame = newFrame.Remove(newFrame.Length - 1);

            byte[]? deser;

            if (isCompleteFrame == '0')
            {
                deser = JsonSerializer.Deserialize<byte[]>(newFrame);
                Debug.Assert(deser != null && deser.Length > 0);
                int length = LZ4Codec.Decode(deser, 4, deser.Length - 4,
                                expansionBuffer, 0, expansionBuffer.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(length), 0, deser, 0, 4);
                deser = expansionBuffer;
            }
            else
            {
                deser = Convert.FromBase64String(newFrame);
            }

            MemoryStream ms = new(deser);
            var xor_bitmap = new Bitmap(ms);
            var newResolution = new Resolution() { Height = xor_bitmap.Height, Width = xor_bitmap.Width };


            if (oldImage == null || newResolution != _resolution)
            {
                oldImage = new Bitmap(newResolution.Width, newResolution.Height);
            }

            if (isCompleteFrame == '1') oldImage = xor_bitmap;
            else oldImage = Process(xor_bitmap, oldImage);

            _resolution = newResolution;
            return oldImage;
        }
    }
}
