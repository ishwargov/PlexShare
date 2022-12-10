///<author>Satyam Mishra</author>
///<summary>
/// This file has Frame and the related structures which are used for storing
/// the difference in the pixel and the resolution of image. It also has some other
/// general utilities.
///</summary>

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace PlexShareScreenshare
{
    /// <summary>
    /// struct for storing the resolution of a image
    /// </summary>
    public struct Resolution
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public bool Equals(Resolution p) => Height == p.Height && Width == p.Width;
        public static bool operator ==(Resolution lhs, Resolution rhs) => lhs.Equals(rhs);
        public static bool operator !=(Resolution lhs, Resolution rhs) => !(lhs == rhs);
    }

    /// <summary>
    /// Defines various general utilities.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// The string representing the module identifier for screen share.
        /// </summary>
        public const string ModuleIdentifier = "ScreenShare";

        /// <summary>
        /// Static method to get a nice debug message wrapped with useful information.
        /// </summary>
        /// <param name="message">
        /// Message to wrap
        /// </param>
        /// <param name="withTimeStamp">
        /// Whether to prefix the wrapped message with time stamp or not
        /// </param>
        /// <returns>
        /// The message wrapped with class and method name and prefixed with time stamp if asked
        /// </returns>
        public static string GetDebugMessage(string message, bool withTimeStamp = false)
        {
            // Get the class name and the name of the caller function
            StackFrame? stackFrame = (new StackTrace()).GetFrame(1);
            string className = stackFrame?.GetMethod()?.DeclaringType?.Name ?? "SharedClientScreen";
            string methodName = stackFrame?.GetMethod()?.Name ?? "GetDebugMessage";

            string prefix = withTimeStamp ? $"{DateTimeOffset.Now:F} | " : "";

            return $"{prefix}[{className}::{methodName}] : {message}";
        }

        /// <summary>
        /// Convert an object of "Bitmap" to an object of type "BitmapSource"
        /// </summary>
        /// <param name="bitmap">
        /// The object to convert to "BitmapSource"
        /// </param>
        /// <returns>
        /// The converted "BitmapSource" object
        /// </returns>
        public static BitmapSource BitmapToBitmapSource(this Bitmap bitmap)
        {
            // Create new memory stream to temporarily save the bitmap there
            using MemoryStream stream = new();
            bitmap.Save(stream, ImageFormat.Bmp);

            // Create a new BitmapSource and populate it with Bitmap
            stream.Position = 0;
            BitmapImage result = new();
            result.BeginInit();

            // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
            // Force the bitmap to load right now so we can dispose the stream.
            result.CacheOption = BitmapCacheOption.OnLoad;

            result.StreamSource = stream;
            result.EndInit();
            result.Freeze();
            return result;
        }

        /// <summary>
        /// Convert an object to type "BitmapSource" to an object of type "BitmapImage"
        /// </summary>
        /// <param name="bitmapSource">
        /// The object to convert to "BitmapImage"
        /// </param>
        /// <returns>
        /// The converted "BitmapImage" object
        /// </returns>
        public static BitmapImage BitmapSourceToBitmapImage(BitmapSource bitmapSource)
        {
            // Check if BitmapSource is already a BitmapImage
            if (bitmapSource is not BitmapImage bitmapImage)
            {
                // If not, then create a new BitmapImage
                bitmapImage = new BitmapImage();

                // Create an encoder and add BitmapSource to it
                BmpBitmapEncoder encoder = new();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                // Save the encoder temporarily to a memory stream
                using MemoryStream memoryStream = new();
                encoder.Save(memoryStream);
                memoryStream.Position = 0;

                // Populate the BitmapImage
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        /// <summary>
        /// Convert an object of "Bitmap" to an object of "BitmapImage"
        /// </summary>
        /// <param name="bitmap">
        /// The object to convert to "BitmapImage"
        /// </param>
        /// <returns>
        /// The converted "BitmapImage" object
        /// </returns>
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            return BitmapSourceToBitmapImage(BitmapToBitmapSource(bitmap));
        }
    }
}
