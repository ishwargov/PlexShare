using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Client;
using PlexShareWhiteboard.Client.Interfaces;
using PlexShareWhiteboard.Server;
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
        string text = "";
        Point textBoxPoint = new (100, 100);

        Brush fillBrush = Brushes.Azure;
        Brush strokeBrush = Brushes.Black;
        string mode = "select_object";
        string modeForUndo = "select_object";
        ShapeItem currentShape = null;
        ShapeItem lastShape = null;
        int blobSize = 12;
        IShapeListener machine;
        UndoStackElement stackElement;

        public WhiteBoardViewModel()
        {
            // this will become client and server 
            Boolean isServer = true;
            if (isServer)
                //machine = new ServerSide();
                machine = ServerSide.Instance;
            else
                machine = new ClientSide();
            ShapeItems = new ObservableCollection<ShapeItem>();
            highlightShapes = new List<ShapeItem>();
            
            // this is a new user
            machine.OnShapeReceived(lastShape, Operation.NewUser);

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

        public ShapeItem UpdateFillColor(ShapeItem shape, Brush fillBrush)
        {
            Debug.WriteLine(" Updaing color in select with old color " + shape.Fill + " and new color " + fillBrush);
            shape.Fill = fillBrush;


            ShapeItem newShape = shape.DeepClone();
            newShape.Fill = fillBrush;

            for (int i = 0; i < ShapeItems.Count; i++)
            {
                if (ShapeItems[i].Id == shape.Id)
                {
                    ShapeItems[i] = newShape;
                }
            }
            return newShape;
        }


        public void ChangeFillBrush(SolidColorBrush br)
        {
            fillBrush = br;

            if (select.ifSelected == true)
            {
                Debug.WriteLine("select color changed to " + br.ToString());

                //select.initialSelectionObject = select.selectedObject;
                ShapeItem updateSelectShape = null;
                foreach(ShapeItem s in ShapeItems)
                    if (s.Id == select.selectedObject.Id)
                        updateSelectShape = s;

                select.initialSelectionObject = updateSelectShape.DeepClone();
                lastShape = UpdateFillColor(updateSelectShape, br);
                modeForUndo = "modify";
                ShapeFinished(new Point());
            }
        }
        public ShapeItem UpdateStrokeColor(ShapeItem shape, Brush strokeBrush)
        {
            shape.Fill = fillBrush;

            ShapeItem newShape = shape.DeepClone();
            newShape.Stroke = strokeBrush;

            for (int i = 0; i < ShapeItems.Count; i++)
            {
                if (ShapeItems[i].Id == shape.Id)
                {
                    ShapeItems[i] = newShape;
                }
            }
            return newShape;
        }


        public void ChangeStrokeBrush(SolidColorBrush br)
        {
            strokeBrush = br;
            if (select.ifSelected == true)
            {
                Debug.WriteLine("select color changed to " + br.ToString());
                ShapeItem updateSelectShape = null;
                foreach (ShapeItem s in ShapeItems)
                    if (s.Id == select.selectedObject.Id)
                        updateSelectShape = s;
                select.initialSelectionObject = updateSelectShape.DeepClone();
                lastShape = UpdateStrokeColor(updateSelectShape, br);
            }
        }

        //public void fillColour(SolidColorBrush br)
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
