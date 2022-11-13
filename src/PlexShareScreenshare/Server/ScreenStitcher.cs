///<author>Aditya Agarwal</author>
///<summary>
///This file contains the ScreenStitcher Class that implements the
///screen stitching functionality. It is used by ScreenshareServer.
///</summary>

using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
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
        private CancellationTokenSource? _tokenSource;

        // Called by the `SharedClientScreen`
        public ScreenStitcher(SharedClientScreen scs)
        {
            _oldImage = null;
            _stitchTask = null;
            _resolution = null;
            _sharedClientScreen = scs;
        }
        /// <summary>
        /// Creates(if not exist) and start the task `_stitchTask`
        /// Will read the image using `_sharedClientScreen.GetFrame`
        /// and puts the final image using `_sharedClientScreen.PutFinalImage`
        /// </summary>
        public void StartStitching()
        {
            _tokenSource = new();

            if (_stitchTask == null)
            {
                _tokenSource.Token.ThrowIfCancellationRequested();

                _stitchTask = new Task(() =>
                {
                    while (!_tokenSource.Token.IsCancellationRequested)
                    {
                        _tokenSource.Token.ThrowIfCancellationRequested();
                        Frame? newFrame = _sharedClientScreen.GetImage(_tokenSource.Token);
                        if (newFrame == null)
                        {
                            Trace.WriteLine($"[ScreenSharing] New frame returned by _sharedClientScreen is null.");
                            continue;
                        }
                        Bitmap stichedImage = Stitch(_oldImage, (Frame)newFrame);
                        _oldImage = stichedImage;
                        _sharedClientScreen.PutFinalImage(stichedImage);
                    }
                }, _tokenSource.Token);

                _stitchTask.Start();
            }
        }

        // Kills the task `_stitchTask`
        public async void StopStitching()
        {
            try
            {
                _tokenSource!.Cancel();
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
                _tokenSource!.Dispose();
                _tokenSource = null;
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
        private Bitmap Stitch(Bitmap? oldImage, Frame newFrame)
        {
            Resolution newResolution = newFrame.Resolution;

            if (oldImage == null || newResolution != _resolution)
            {
                oldImage = new Bitmap(newResolution.Width, newResolution.Height);
            }

            foreach (var pixel in newFrame.Pixels)
            {
                int xCordinate = pixel.Coordinates.X;
                int yCordinate = pixel.Coordinates.Y;
                int red = pixel.RGB.R;
                int green = pixel.RGB.G;
                int blue = pixel.RGB.B;
                Color newColor = Color.FromArgb(red, green, blue);
                oldImage.SetPixel(xCordinate, yCordinate, newColor);
            }

            _resolution = newResolution;
            return oldImage;
        }
    }
}
