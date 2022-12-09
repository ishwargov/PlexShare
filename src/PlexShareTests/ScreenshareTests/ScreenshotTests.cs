///<author>Amish Ashish Saxena</author>
///<summary> 
///This file contains tests for Screeshot Class.  
///</summary>

using System.Drawing;
using System.Runtime.InteropServices;
using PlexShareScreenshare.Client;


namespace PlexShareTests.ScreenshareTests
{
    /// <summary>
    /// Inter class that has methods to get access to the physical display's properties.
    /// The properties are used to verify screenshot resoution in tests.
    /// </summary>
    internal class DisplayTools
    {
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        private enum DeviceCap
        {
            Desktopvertres = 117,
            Desktophorzres = 118
        }
        /// <summary>
        /// Gives the Primary Physical Display's Size in the form of resolution in pixels
        /// </summary>
        /// <returns>Width, Height of the Display resolution in pixels</returns>
        public static (int, int) GetPhysicalDisplaySize()
        {
            // Get the handle to the primary display device
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();

            int physicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.Desktopvertres);
            int physicalScreenWidth = GetDeviceCaps(desktop, (int)DeviceCap.Desktophorzres);

            return (physicalScreenWidth, physicalScreenHeight);
        }
    }

    [Collection("Sequential")]
    public class ScreenshotTests
    {
        /// <summary>
        /// Checks if the singleton instance of Screenshot works properly
        /// </summary>
        [Fact]
        public void SingletonTest()
        {
            Screenshot screenshot1 = Screenshot.Instance();
            Screenshot screenshot2 = Screenshot.Instance();
            Assert.Equal(screenshot1, screenshot2);
        }

        /// <summary>
        /// Check if the captured screenshot is of the expected resolution
        /// </summary>
        [Fact]
        public void ScreenshotResolutionTest()
        {
            Screenshot screenshot = Screenshot.Instance();
            Bitmap image = screenshot.MakeScreenshot();
            Thread.Sleep(100);

            var screenSize = DisplayTools.GetPhysicalDisplaySize();
            var screenWidth = screenSize.Item1;
            var screenHeight = screenSize.Item2;

            Assert.Equal(screenHeight * 2 / 3, image.Height);
            Assert.Equal(screenWidth * 2 / 3, image.Width);
        }
    }
}