///<author> Amish Ashish Saxena </author>
///<summary> This file contains the Screenshot Class that implements the screenshot functionality. ///</summary>
///<reference> https://github.com/0x2E757/ScreenCapturer ///</reference>
///<reference> https://github.com/sharpdx/SharpDX-Samples/blob/master/Desktop/Direct3D11.1/ScreenCapture/Program.cs ///</reference>

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Resource = SharpDX.DXGI.Resource;


namespace PlexShareScreenshare.Client
{
    /// <summary>
    /// Class contains the necessary functions for taking the screenshot of the current screen.
    /// </summary>
    internal class Screenshot
    {
        private static Screenshot? instance;
        public Boolean CaptureActive { get; private set; }
        private Factory1? Factory1;
        private Adapter1? Adapter1;
        private Device? Device;
        private Output? Output;
        private Output1? Output1;
        private Int32 Width;
        private Int32 Height;
        private Rectangle Bounds;
        private Texture2DDescription Texture2DDescription;
        private Texture2D? Texture2D;
        private OutputDuplication? OutputDuplication;
        private Bitmap? Bitmap;

        private Int32 MakeScreenshot_LastDisplayIndexValue;
        private Int32 MakeScreenshot_LastAdapterIndexValue;

        protected Screenshot()
        {
            CaptureActive = false;
            InitializeVariables(0, 0, true);
        }

        public static Screenshot Instance()
        {
            if (instance == null)
            {
                instance = new Screenshot();
            }
            return instance;
        }

        /// <summary>
        /// Core function of class for taking the screenshot. Uses SharpDX for faster image capture.
        /// </summary>
        /// <param name="displayIndex">Index for the display which is to be captured. Defaults to 0 (Primary Display)</param>
        /// <param name="adapterIndex">Index for the display card to be used. Defaults to 0 (Primary graphics card)</param>
        /// <param name="maxTimeout">Timeout to get duplicated frame</param>
        /// <returns>The bitmap image for the screenshot</returns>
        public Bitmap MakeScreenshot(Int32 displayIndex = 0, Int32 adapterIndex = 0, Int32 maxTimeout = 5000)
        {
            InitializeVariables(displayIndex, adapterIndex);
            Resource screenResource;
            OutputDuplication.TryAcquireNextFrame(maxTimeout, out _, out screenResource);
            if (screenResource == null) return null;
            Texture2D screenTexture2D = screenResource.QueryInterface<Texture2D>();
            Device.ImmediateContext.CopyResource(screenTexture2D, Texture2D);
            DataBox dataBox = Device.ImmediateContext.MapSubresource(Texture2D, 0, MapMode.Read, MapFlags.None);
            BitmapData bitmapData = Bitmap.LockBits(Bounds, ImageLockMode.WriteOnly, Bitmap.PixelFormat);
            IntPtr dataBoxPointer = dataBox.DataPointer;
            IntPtr bitmapDataPointer = bitmapData.Scan0;
            for (Int32 y = 0; y < Height; y++)
            {
                Utilities.CopyMemory(bitmapDataPointer, dataBoxPointer, Width * 4);
                dataBoxPointer = IntPtr.Add(dataBoxPointer, dataBox.RowPitch);
                bitmapDataPointer = IntPtr.Add(bitmapDataPointer, bitmapData.Stride);
            }
            Bitmap.UnlockBits(bitmapData);
            Device.ImmediateContext.UnmapSubresource(Texture2D, 0);
            OutputDuplication.ReleaseFrame();
            screenTexture2D.Dispose();
            screenResource.Dispose();
            Bitmap SmallBitmap = new Bitmap(Bitmap, 2 * Bitmap.Width / 3, 2 * Bitmap.Height / 3);
            return SmallBitmap;
        }

        /// <summary>
        /// Initializes the members of the class.
        /// </summary>
        /// <param name="displayIndex">Index for the display which is to be captured. Defaults to 0 (Primary Display)</param>
        /// <param name="adapterIndex">Index for the display card to be used. Defaults to 0 (Primary graphics card)</param>
        /// <param name="forcedInitialization"></param>
        private void InitializeVariables(Int32 displayIndex, Int32 adapterIndex, Boolean forcedInitialization = false)
        {
            Boolean displayIndexChanged = MakeScreenshot_LastDisplayIndexValue != displayIndex;
            Boolean adapterIndexChanged = MakeScreenshot_LastAdapterIndexValue != adapterIndex;
            if (displayIndexChanged || adapterIndexChanged || forcedInitialization)
            {
                DisposeVariables();
                Factory1 = new Factory1();
                Adapter1 = Factory1.GetAdapter1(adapterIndex);
                Device = new Device(Adapter1);
                Output = Adapter1.GetOutput(displayIndex);
                Output1 = Output.QueryInterface<Output1>();
                Width = Output1.Description.DesktopBounds.Right - Output1.Description.DesktopBounds.Left;
                Height = Output1.Description.DesktopBounds.Bottom - Output1.Description.DesktopBounds.Top;
                Bounds = new Rectangle(Point.Empty, new Size(Width, Height));
                Texture2DDescription = new Texture2DDescription
                {
                    CpuAccessFlags = CpuAccessFlags.Read,
                    BindFlags = BindFlags.None,
                    Format = Format.B8G8R8A8_UNorm,
                    Width = Width,
                    Height = Height,
                    OptionFlags = ResourceOptionFlags.None,
                    MipLevels = 1,
                    ArraySize = 1,
                    SampleDescription = { Count = 1, Quality = 0 },
                    Usage = ResourceUsage.Staging
                };
                Texture2D = new Texture2D(Device, Texture2DDescription);
                OutputDuplication = Output1.DuplicateOutput(Device);
                OutputDuplication.TryAcquireNextFrame(1000, out _, out _);
                OutputDuplication.ReleaseFrame();
                Bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppRgb);
                MakeScreenshot_LastAdapterIndexValue = adapterIndex;
                MakeScreenshot_LastDisplayIndexValue = displayIndex;
            }
        }

        /// <summary>
        /// Disposes the class memebers to avoid memory hogging.
        /// </summary>
        public void DisposeVariables()
        {
            Bitmap?.Dispose();
            OutputDuplication?.Dispose();
            Texture2D?.Dispose();
            Output1?.Dispose();
            Output?.Dispose();
            Device?.Dispose();
            Adapter1?.Dispose();
            Factory1?.Dispose();
            MakeScreenshot_LastAdapterIndexValue = -1;
            MakeScreenshot_LastDisplayIndexValue = -1;
        }
    }


}
