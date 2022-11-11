///<author>Aditya Agarwal</author>
///<summary> 
///This file contains the ScreenStitcher Class that implements the
///screen stitching functionality. It is used by ScreenshareServer. 
///</summary>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;


// Each frame consists of the resolution of the image and the ImageDiffList
using Frame = System.Tuple<System.Tuple<int, int>,
                        System.Collections.Generic.List<System.Tuple<System.Tuple<int, int>,
                        System.Tuple<int, int, int>>>>;


namespace ScreenShare.Server
{
    /// <summary>
    /// Class contains implementation of the screen stitching using threads (tasks)
    /// </summary>
    internal class ScreenStitcher
    {
        private SharedClientScreen _sharedClientScreen;

        private Task _stitchTask;
        private Bitmap _stichedImage;

        // new frame to hold the updated pixels
        private Frame _newFrame;
        private Bitmap _oldImage;
        private Tuple<int, int> _resolution;

        // Token and its source for killing the task
        private CancellationTokenSource Source;
        private CancellationToken Token;

        // Called by the `SharedClientScreen`
        public ScreenStitcher(SharedClientScreen scs)
        {
            _stichedImage = null;
            _newFrame = null;
            _oldImage = null;
            _stitchTask = null;
            _resolution = null;
            _sharedClientScreen= scs;
        }
        /// <summary>
        /// Creates(if not exist) and start the task `_stitchTask`
        /// Will read the image using `_sharedClientScreen.GetFrame`
        /// and puts the final image using `_sharedClientScreen.PutFinalImage`
        /// </summary>
        public void StartStitching()
        {
            Source = new CancellationTokenSource();
            Token = Source.Token;

            if (_stitchTask == null)
            {
                
                    _stitchTask = new Task(() =>
                    {
                        while (!Source.IsCancellationRequested)
                        {
                            _newFrame = _sharedClientScreen.GetImage();
                            _stichedImage = Stitch(_oldImage, _newFrame);
                            _sharedClientScreen.PutFinalImage(_stichedImage);
                        }
                    }, Token);
            }

            _stitchTask.Start();

        }

        // Kills the task `_stitchTask`
        public void StopStitching()
        {
            Source.Cancel();
            _stitchTask.Wait();
            _stichedImage = null;
            _stitchTask = null;
        }

        /// <summary>
        /// Function to stitch the new image over the old image
        /// If resolution is changed update whole Image, else update the changed pixels
        /// </summary>
        /// <param name="oldImage">The previous image of client's screen</param>
        /// <param name="frame">The list of the updated pixels, list of : { x, y, (R, G, B)}</param>
        /// <returns>The updated new image of client's screen</returns>

        private Bitmap Stitch(Bitmap oldImage, Frame frame)
        {
            Tuple<int, int> _newResolution = frame.Item1;

            if (_newResolution != _resolution)
            {
                oldImage = new Bitmap(_newResolution.Item1, _newResolution.Item2);
            }

            for (int x = 0; x < frame.Item2.Count; x++)
            {
             
                int xCordinate = frame.Item2[x].Item1.Item1;
                int yCordinate = frame.Item2[x].Item1.Item2;
                int red = frame.Item2[x].Item2.Item1;
                int green = frame.Item2[x].Item2.Item2;
                int blue = frame.Item2[x].Item2.Item3;
                Color newColor = Color.FromArgb(red, green, blue);

                oldImage.SetPixel(xCordinate, yCordinate, newColor);
            }

            _resolution= _newResolution;
            return oldImage;
        }
    }
}
