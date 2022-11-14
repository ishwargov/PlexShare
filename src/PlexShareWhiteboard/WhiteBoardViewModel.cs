using PlexShareNetwork.Communication;
using PlexShareNetwork.Serialization;
using PlexShareNetwork;
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
using System.Configuration;
using System.ComponentModel;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<ShapeItem> ShapeItems { get; set; }
        public SelectObject select = new();
        List<ShapeItem> highlightShapes;

        String currentId = "u0_f0";
        int currentIdVal = 0;
        string userId = "0";
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
        Boolean isServer=false;

        private WhiteBoardViewModel()
        {
            // this will become client and server 
            isServer = true;
            
            ShapeItems = new ObservableCollection<ShapeItem>();
            highlightShapes = new List<ShapeItem>();

        }
        private static WhiteBoardViewModel instance;
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public static WhiteBoardViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WhiteBoardViewModel();
                }

                return instance;
            }
        }

        //public void SetUserId(string _userId)
        public void SetUserId(int _userId)
        {
            String userId = _userId.ToString();
            currentId = "u" + userId + "_f" + currentIdVal;
            currentIdVal++;

            if (isServer)
            {
                machine = ServerSide.Instance;
                machine.SetUserId(userId);
            }
            else
            {
                machine = ClientSide.Instance;
                machine.SetUserId(userId);
            }

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
