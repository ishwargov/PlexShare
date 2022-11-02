///<author>Satyam Mishra</author>
///<summary>
/// This file has ScreenProcessor class. It is responsible for 
/// processing the image from ScreenCapturer class and calculating
/// the image bits that are different from the previous image
///</summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// This datatype is for storing a list of the coordinate
// of pixel and its RGB values i.e. list of ((x, y), (R, G, B))
using ImageDiffList = System.Collections.Generic.List<System.Tuple<System.Tuple<int, int>, 
						System.Tuple<int, int, int>>>;

// Each frame consists of the resolution of the image and the ImageDiffList
using Frame = System.Tuple<System.Tuple<int, int>,
						System.Collections.Generic.List<System.Tuple<System.Tuple<int, int>, 
						System.Tuple<int, int, int>>>>;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using PlexShareScreenshare.Client;

namespace ScreenShare.Client
{
	/// <summary>
	/// Class contains implementation of the screen processing using threads (tasks)
	/// </summary>
	internal class ScreenProcessor
	{
		// The queue in which the image will be enqueued after
		// processing it
		private Queue<Frame> ProcessedFrame;

		// Processing task
		private Task ProcessorTask;

		// The screen capturer object
		private ScreenCapturer Capturer;

		// Old and the new resolutions 
		private Tuple<int, int> OldRes { get; set; }
		public Tuple<int, int> NewRes { private get; set; }

		// tokens added to be able to stop the thread execution
		CancellationTokenSource tokenSource;
		CancellationToken token;

		// storing the previous frame
		Bitmap prevImage;

        /// <summary>
        /// Called by ScreenShareClient
        /// Initialize queue, oldRes, newRes,
        /// cancellation token and the previous image
        /// </summary>
        ScreenProcessor(ScreenCapturer Capturer)
		{
			this.Capturer = Capturer;
			ProcessedFrame = new Queue<Frame>();
			OldRes = new Tuple<int, int>(720, 1280);
			NewRes = new Tuple<int, int>(720, 1280);
			tokenSource = new CancellationTokenSource();
			token = tokenSource.Token;
			prevImage = new Bitmap(720, 1280);
        }

        /// <summary>
        /// Pops and return the image from the queue
        /// </summary>
        public Frame GetImage() 
		{
			while (ProcessedFrame.Count != 0) Thread.Sleep(100);
			lock(ProcessedFrame)
			{
                return ProcessedFrame.Dequeue();
            }
		}
		/// <summary>
		/// In this function we go through every pixel of both the images and
		/// returns list of those pixels which are different in both the images
		/// </summary>
		private ImageDiffList ProcessUsingLockbits(Bitmap processedBitmap, Bitmap processedBitmap1)
		{
			ImageDiffList tmp = new ImageDiffList();
			int count = 0;
			BitmapData bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite, processedBitmap.PixelFormat);

			int bytesPerPixel = Bitmap.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
			int byteCount = bitmapData.Stride * processedBitmap.Height;
			byte[] pixels = new byte[byteCount];
			IntPtr ptrFirstPixel = bitmapData.Scan0;
			Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
			int heightInPixels = bitmapData.Height;
			int widthInBytes = bitmapData.Width * bytesPerPixel;
			processedBitmap.UnlockBits(bitmapData);

			BitmapData bitmapData1 = processedBitmap1.LockBits(new Rectangle(0, 0, processedBitmap1.Width, processedBitmap1.Height), ImageLockMode.ReadWrite, processedBitmap1.PixelFormat);

			int bytesPerPixel1 = Bitmap.GetPixelFormatSize(processedBitmap1.PixelFormat) / 8;
			int byteCount1 = bitmapData1.Stride * processedBitmap1.Height;
			byte[] pixels1 = new byte[byteCount1];
			IntPtr ptrFirstPixel1 = bitmapData1.Scan0;
			Marshal.Copy(ptrFirstPixel1, pixels1, 0, pixels1.Length);
			int heightInPixels1 = bitmapData1.Height;
			int widthInBytes1 = bitmapData1.Width * bytesPerPixel1;

			processedBitmap1.UnlockBits(bitmapData1);

			for (int y = 0; y < heightInPixels; y++)
			{
				int currentLine = y * bitmapData.Stride;
				int currentLine1 = y * bitmapData1.Stride;
				for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
				{
					int oldBlue = pixels[currentLine + x];
					int oldGreen = pixels[currentLine + x + 1];
					int oldRed = pixels[currentLine + x + 2];

					int newBlue = pixels1[currentLine1 + x];
					int newGreen = pixels1[currentLine1 + x + 1];
					int newRed = pixels1[currentLine1 + x + 2];

					if (oldBlue != newBlue || oldGreen != newGreen || oldRed != newRed)
					{
						Tuple<int, int> coordinates = new Tuple<int, int>(x / bytesPerPixel, y);
						Tuple<int, int, int> colors = new Tuple<int, int, int>(newRed, newGreen, newBlue);
						tmp.Add(new Tuple<Tuple<int, int>, Tuple<int, int, int>>(coordinates, colors));
						count++;
					}
				}
			}
			return tmp;
		}
		/// <summary>
		/// main function which will run in loop and capture the image
		/// calculate the image bits differences and 
		/// </summary>
		private void Processing()
		{
			while (true) 
			{
				Bitmap img = Capturer.GetImage();
                img = Compress(img);
                ImageDiffList DiffList = ProcessUsingLockbits(prevImage, img);
				lock (ProcessedFrame)
				{
                    ProcessedFrame.Append(new Frame(NewRes, DiffList));
                }
                prevImage = img;
			}
		}

        /// <summary>
        /// Called by ScreenShareClient when the client starts screen sharing
        /// Will have a lambda function - Process and pushes to the queue
        /// Create the task for the lambda function 
        /// </summary>
        public void StartProcessing()
		{
            ProcessorTask = new Task(Processing);
			ProcessorTask.Start();
            ProcessorTask.WaitAsync(token);
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
        /// Called by ScreenShareClient when the client stops screen sharing
        /// kill the processor task and make the processor task variable null
        /// Empty the Queue
        /// </summary>
        public void StopProcessing()
		{
            tokenSource.Cancel();
        }

        /// <summary>
        /// Called by StartProcessing
        /// run the compression algorithm and returns list of changes in pixels
        /// Does it need to be public?
        /// </summary>
        public Bitmap Compress(Bitmap img)
		{
			if (NewRes != OldRes)
			{
                prevImage = new Bitmap(NewRes.Item1, NewRes.Item2);
				OldRes = NewRes;
            }
			img = new Bitmap(img, NewRes.Item1, NewRes.Item2);
			return img;
		}
	}
}