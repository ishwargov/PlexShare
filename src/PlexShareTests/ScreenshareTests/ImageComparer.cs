/// <author>Satyam Mishra</author>
/// <summary>
/// Defines the static "Utils" class which defines
/// general utilities required for unit tests.
/// </summary>

using System.Drawing;
using System.Drawing.Imaging;

namespace PlexShareTests.ScreenshareTests
{
    /// <summary>
    /// Defines the static "Utils" class which defines
    /// general utilities required for unit tests.
    /// </summary>
    public static partial class Utils
    {
        /// <summary>
        /// Takes two images and compares if they are equal.
        /// </summary>
        /// <param name="image1">First image.</param>
        /// <param name="image2">Second image.</param>
        /// <returns>True if they are equal else false.</returns>
        public static bool CompareBitmap(Bitmap image1, Bitmap image2)
        {
            byte[] image1Bytes;
            byte[] image2Bytes;

            using (var mstream = new MemoryStream())
            {
                image1.Save(mstream, ImageFormat.Bmp);
                image1Bytes = mstream.ToArray();
            }

            using (var mstream = new MemoryStream())
            {
                image2.Save(mstream, ImageFormat.Bmp);
                image2Bytes = mstream.ToArray();
            }

            var image164 = Convert.ToBase64String(image1Bytes);
            var image264 = Convert.ToBase64String(image2Bytes);

            return string.Equals(image164, image264);
        }
    }
}
