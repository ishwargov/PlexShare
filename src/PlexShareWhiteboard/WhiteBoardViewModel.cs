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
        Brush fillBrush = Brushes.Azure;
        String curId = "u0_f0";
        int curIdVal = 0;
        int userId = 0;
        Brush strokeBrush = Brushes.Black;
        string mode = "create_rectangle";
        int curZIndex = 0;
        ShapeItem currentShape = null;
        ShapeItem lastShape;
        SelectObject select = new SelectObject();
        List<ShapeItem> highlightShapes;

        public WhiteBoardViewModel()
        {
            ShapeItems = new ObservableCollection<ShapeItem>();
            highlightShapes = new List<ShapeItem>();
        }


        public void ShapeFinished(Point a)
        {
            Debug.Write("Shape Finished with Before mode: " + mode);
            lastShape = null;
            if (mode == "scale_mode" || mode == "dimensionChange_mode" || mode == "translate_mode")
                mode = "select_mode";
            Debug.WriteLine(" and After mode : " + mode);
        }

        public void IncrementId()
        {
            curIdVal++;
            curId = "u" + userId + "_f" + curIdVal;
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
            curZIndex++;
        }
        public void DecreaseZIndex()
        {
            curZIndex--;
        }
    }
}
