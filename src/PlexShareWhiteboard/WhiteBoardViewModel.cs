/********************************************************************************
 * Filename    = WhiteBoardViewModel.cs
 *
 * Author      = Jerry John Thomas
 *
 * Product     = Plex Share
 * 
 * Project     = White Board
 *
 * Description = This is the View Model.
 *               This contains the White Board View Model constructor and some 
 *               methods called from the view and all class variables.
 ********************************************************************************/

using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Client;
using PlexShareWhiteboard.Client.Interfaces;
using PlexShareWhiteboard.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ShapeItem> ShapeItems { get; set; }    // this contains all the shape items in the canvas
        public SelectObject select = new();                                // for select
        List<ShapeItem> highlightShapes;                                   // for highlighting selected shape

        public bool canDraw = false;                                       // a user can draw only if it is set to true
        String currentId = "u0_f0";                                        // current id of the shape
        int currentIdVal = 0;
        public string userId = "init";
        int currentZIndex = 0;                                             // z index indicates which object is in front
        Point textBoxPoint = new (100, 100);                               // point that is being clicked to write text box

        Brush fillBrush = null;                                            // stores color of the object (fill colour)
        Brush strokeBrush = Brushes.Black;                                 // stores color of the border
        int strokeThickness = 1;                                           // thickness of the stroke
        string mode = "select_object";                                     // declared for identifying which operation
        public string modeForUndo = "select_object";                       // declared for pushing to undo stack element         
        ShapeItem currentShape = null;
        public ShapeItem lastShape = null;
        public ShapeItem textBoxLastShape = null;

        int blobSize = 12;                                                 // size of the highlighting rectangle box
        public IShapeListener machine;                                     // can be client or server
        UndoStackElement stackElement;
        public Boolean isServer=true;                                      // indicates whether a machine is server or client

        public ObservableCollection<int> CheckList { get; set; }           // list for storing snapshots in client

        /// <summary>
        /// constructor
        /// </summary>
        private WhiteBoardViewModel()
        {
            CheckList = new();
            ShapeItems = new ObservableCollection<ShapeItem>();
            highlightShapes = new List<ShapeItem>();

            if(userId.Equals("init"))                                      // if user id is not set, then the user cannot draw
                canDraw = false;
        }

        private static WhiteBoardViewModel instance;
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// called when the property changes
        /// </summary>
        /// <param name="property"></param>
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// whiteboardviewmodel is singleton
        /// </summary>
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

        /// <summary>
        /// this is called by dashboard to set the userid of the machine
        /// </summary>
        /// <param name="_userId"></param>
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
            canDraw = true;
            Trace.WriteLine("[whiteboard] setuserid over candraw" + canDraw);
        }

        /// <summary>
        /// increments the currentIdVal and updates the current id
        /// the current id is set as the userid while creation of a shape
        /// </summary>
        public void IncrementId()
        {
            currentIdVal++;
            currentId = "u" + userId + "_f" + currentIdVal;
        }

        /// <summary>
        /// This function is used to push a text box shape to undo stack as well as send it to server.
        /// This is called by TextBoxAdding function.
        /// </summary>
        public void TextFinishPush()
        {
            Trace.WriteLine("[whiteboard] : text finished and pushed");
            stackElement = new UndoStackElement(textBoxLastShape, textBoxLastShape, Operation.Creation);
            InsertIntoStack(stackElement);

            if (textBoxLastShape != null)
            {
                machine.OnShapeReceived(textBoxLastShape, Operation.Creation);
            }
        }

        /// <summary>
        /// sets the current mode as the given mode
        /// these mode defines what operation is to be done
        /// </summary>
        /// <param name="new_mode"></param>
        public void ChangeMode(string new_mode)
        {
            Trace.WriteLine("[whiteboard] : mode changed to " + mode);
            TextBoxAddding(mode);
            mode = new_mode;
        }

        /// <summary>
        /// Used to update the fill color of a particular shape with a given color
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="fillBrush"></param>
        /// <returns name="newShape"></returns>
        public ShapeItem UpdateFillColor(ShapeItem shape, Brush fillBrush)
        {
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

        /// <summary>
        /// changes the global fill color of the brush with a given color
        /// if an object is selected, it is passed to UpdateFillColor to update the fill color of the shape
        /// </summary>
        /// <param name="br"></param>
        public void ChangeFillBrush(SolidColorBrush br)
        {
            fillBrush = br;

            if (select.ifSelected == true)
            {

                ShapeItem updateSelectShape = null;

                foreach(ShapeItem s in ShapeItems)
                    if (s.Id == select.selectedObject.Id)
                        updateSelectShape = s;

                select.initialSelectionObject = updateSelectShape.DeepClone();
                lastShape = UpdateFillColor(updateSelectShape, br);
                Trace.WriteLine("[whiteboard] : fill colour changed");
                modeForUndo = "modify";                                         // setting mode for undo redo purpose
                ShapeFinished(new Point());                                     // called so that it is passed to undo stack
            }
        }

        /// <summary>
        /// Used to update the border color of a particular shape with a given color
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="strokeBrush"></param>
        /// <returns> name="newShape"/</returns>
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

        /// <summary>
        /// changes the global border color of the brush with a given color
        /// if an object is selected, it is passed to UpdateStrokeColor to update the border color of the shape
        /// </summary>
        /// <param name="br"></param>
        public void ChangeStrokeBrush(SolidColorBrush br)
        {
            strokeBrush = br;

            if (select.ifSelected == true)
            {
                ShapeItem updateSelectShape = null;

                foreach (ShapeItem s in ShapeItems)
                    if (s.Id == select.selectedObject.Id)
                        updateSelectShape = s;

                select.initialSelectionObject = updateSelectShape.DeepClone();
                lastShape = UpdateStrokeColor(updateSelectShape, br);
                Trace.WriteLine("[whiteboard] : stroke colour changed");
                modeForUndo = "modify";                                         // setting mode for undo redo purpose
                ShapeFinished(new Point());                                     // called so that it is passed to undo stack
            }
        }

        /// <summary>
        /// changes the stroke thickness of the brush as a whole 
        /// </summary>
        /// <param name="thickness"></param>
        public void ChangeStrokeThickness(int thickness)
        {
            strokeThickness = thickness;
            Trace.WriteLine("[whiteboard] : stroke thicnkess changed");
        }

        /// <summary>
        /// function to increment the current zindex
        /// after every new shape is created and added to shape item list, this function is called
        /// </summary>
        public void IncreaseZIndex()
        {
            currentZIndex++;
        }
    }
}
