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
    public partial class WhiteBoardViewModel : INotifyPropertyChanged
    {
        //public AsyncObservableCollection<ShapeItem> ShapeItems { get; set; }
        public ObservableCollection<ShapeItem> ShapeItems { get; set; }
        public SelectObject select = new();
        List<ShapeItem> highlightShapes;


        public bool canDraw = false;
        String currentId = "u0_f0";
        int currentIdVal = 0;
        public string userId = "init";
        int currentZIndex = 0;
        Point textBoxPoint = new (100, 100);

        Brush fillBrush = Brushes.Azure;
        Brush strokeBrush = Brushes.Black;
        int strokeThickness = 1;
        string mode = "select_object";
        public string modeForUndo = "select_object";
        ShapeItem currentShape = null;
        public ShapeItem lastShape = null;
        public ShapeItem textBoxLastShape = null;

        int blobSize = 12;
        IShapeListener machine;
        UndoStackElement stackElement;
        public Boolean isServer=true;

        public ObservableCollection<int> CheckList { get; set; }
        List<int> snapshotNumbers = new() { 1, 2, 3, 4, 5 };

        private WhiteBoardViewModel()
        {
            // this will become client and server 
            //isServer = true;
            CheckList = new();
            //ShapeItems = new AsyncObservableCollection<ShapeItem>();
            ShapeItems = new ObservableCollection<ShapeItem>();
            highlightShapes = new List<ShapeItem>();
            if(userId.Equals("init"))
                canDraw = false;
            /*CheckList.Add(1);
            CheckList.Add(2);
            CheckList.Add(3);*/

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
            userId = _userId.ToString();
            Trace.WriteLine("[whiteboard] called setuserid " + userId);
            currentId = "u" + userId + "_f" + currentIdVal;
            currentIdVal++;

            if (isServer)
            {
                Trace.WriteLine("[WhiteBoard] setuserId this is a server");
                machine = ServerSide.Instance;
                machine.SetUserId(userId);
                
            }
            else
            {
                Trace.WriteLine("[WhiteBoard] setuserId this is a client");
                machine = ClientSide.Instance;
                machine.SetUserId(userId);
            }
            //machine.SetVMRef(this);
            canDraw = true;
            Trace.WriteLine("[whiteboard] setuserid over candraw" + canDraw);

        }
        public void IncrementId()
        {
            currentIdVal++;
            currentId = "u" + userId + "_f" + currentIdVal;
        }
        public void TextFinishPush()
        {
            stackElement = new UndoStackElement(textBoxLastShape, textBoxLastShape, Operation.Creation);
            InsertIntoStack(stackElement);

            if (textBoxLastShape != null)
            {
                //Debug.WriteLine("into undo " + textBoxLastShape.Id + " " + textBoxLastShape.TextString);
                machine.OnShapeReceived(textBoxLastShape, Operation.Creation);
            }
        }
        public void ChangeMode(string new_mode)
        {
            if (mode == "create_textbox")
            {
                if (textBoxLastShape != null && textBoxLastShape.TextString != null &&
                         textBoxLastShape.TextString.Length != 0)
                {

                    TextFinishPush();
                    Debug.WriteLine("entering undo modeeeee");

                }
                else if (textBoxLastShape != null)
                {
                    for (int i = 0; i < ShapeItems.Count; ++i)
                    {
                        if (textBoxLastShape.Id == ShapeItems[i].Id)
                        {
                            ShapeItems.RemoveAt(i);
                            break;
                        }
                    }
                }
                textBoxLastShape = null;
            }
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
            Debug.WriteLine("ChangeFillBrush called");
            fillBrush = br;

            if (select.ifSelected == true)
            {

                Debug.WriteLine("ChangeFillBrush select color changed to " + br.ToString());

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
            shape.Stroke = strokeBrush;

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
            Debug.WriteLine("ChangeStrokeBrush called");
            strokeBrush = br;

            if (select.ifSelected == true)
            {
                Debug.WriteLine("ChangeStrokeBrush select color changed to " + br.ToString());
                ShapeItem updateSelectShape = null;
                foreach (ShapeItem s in ShapeItems)
                    if (s.Id == select.selectedObject.Id)
                        updateSelectShape = s;

                select.initialSelectionObject = updateSelectShape.DeepClone();
                lastShape = UpdateStrokeColor(updateSelectShape, br);
                modeForUndo = "modify";
                ShapeFinished(new Point());
            }
        }

        //public void fillColour(SolidColorBrush br)

        public void ChangeStrokeThickness(int thickness)
        {
            strokeThickness = thickness;
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
