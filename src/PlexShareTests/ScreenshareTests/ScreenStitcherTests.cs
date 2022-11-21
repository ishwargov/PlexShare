///<author>Aditya Agarwal</author>
///<summary>
///This file contains the test for ScreenStitcher Class that implements the
///screen stitching functionality. It is used by ScreenshareServer.
///</summary>

using PlexShareScreenshare.Client;
using PlexShareScreenshare.Server;
using System.Drawing;
using System.IO.Compression;

namespace PlexShareTests.ScreenshareTests
{
    [Collection("Sequential")]
    public class ScreenshareStitcherTests
    {
        private byte[] CompressByteArray(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        /// <summary>
        /// Test to check if the decompression algorithm works properly.
        /// </summary>
        [Fact]
        public void TestDecompressByteArray()
        {
            byte[] arr = { (byte)'c', (byte)'h', (byte)'a', (byte)'r' };
            var compressed_arr = CompressByteArray(arr);

            var decompressed_arr = ScreenStitcher.DecompressByteArray(compressed_arr);
            Assert.Equal(arr, decompressed_arr);
        }

        /// <summary>
        /// Test to check if the XOR algorithm in Process method works correctly.
        /// </summary>
        [Fact]
        public void TestProcess()
        {

            Bitmap prev_img = (Screenshot.Instance()).MakeScreenshot();
            Bitmap img = new Bitmap(prev_img);

            Bitmap diff = ScreenStitcher.Process(prev_img, img);
            Bitmap undo_diff = ScreenStitcher.Process(prev_img, diff);

            Assert.NotNull(diff);
            Assert.NotNull(undo_diff);

            Assert.True(Utils.CompareBitmap(prev_img, img));
        }
    }
}
