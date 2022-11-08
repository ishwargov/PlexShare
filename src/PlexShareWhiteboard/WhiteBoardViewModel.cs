using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        public ObservableCollection<ShapeItem> ShapeItems { get; set; }
        public SelectObject select = new();
        List<ShapeItem> highlightShapes;

        String currentId = "u0_f0";
        int currentIdVal = 0;
        int userId = 0;
        int currentZIndex = 0;

        Brush fillBrush = Brushes.Azure;
        Brush strokeBrush = Brushes.Black;
        string mode = "select_object";
        ShapeItem currentShape = null;
        ShapeItem lastShape = null;
        int blobSize = 12;

        public WhiteBoardViewModel()
        {
            ShapeItems = new ObservableCollection<ShapeItem>();
            highlightShapes = new List<ShapeItem>();
        }

        public void IncrementId()
        {
            currentIdVal++;
            currentId = "u" + userId + "_f" + currentIdVal;
        }
        public void ChangeMode(string new_mode)
        {
            mode = new_mode;
        }
        public void ChangeFillBrush(SolidColorBrush br)
        {
            fillBrush = br;
        }
        public void IncreaseZIndex()
        {
            currentZIndex++;
        }

        public void DecreaseZIndex()
        {
            currentZIndex--;
        }
        
    }
}
