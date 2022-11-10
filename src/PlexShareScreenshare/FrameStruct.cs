///<author>Satyam Mishra</author>
///<summary>
/// This file has Frame and the related structures which are used for storing
/// the difference in the pixel and the resolution of image
///</summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareScreenshare
{
    /// <summary>
    /// struct for storing x and y coordinates of a pixel
    /// </summary>
    public struct Coordinates
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    /// <summary>
    /// struct for storing RGB value of a pixel
    /// </summary>
    public struct RGB
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
    }

    /// <summary>
    /// struct for storing both the coordinates and the RGB values
    /// </summary>
    public struct Pixel
    {
        public Coordinates Coordinates { get; set; }
        public RGB RGB { get; set; }
    }

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
    /// frame struct storing resolution of the image and list of 
    /// pixels which are different from the previous image
    /// </summary>
    public struct Frame
    {
        public Resolution Resolution { get; set; }
        public List<Pixel> Pixels { get; set; }
    }
}
