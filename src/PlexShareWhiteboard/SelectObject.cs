using PlexShareWhiteboard.BoardComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlexShareWhiteboard
{
    internal class SelectObject
    {
        public bool ifSelected = false;
        public ShapeItem selectedObject;
        public Point InitialSelectionPoint;

        public SelectObject()
        {
            ifSelected = false;
        }
    }
}
