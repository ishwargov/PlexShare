/***************************
 * Filename    = WBMessageHandler.cs
 *
 * Author      = Joel Sam Mathew
 *
 * Product     = Plex Share
 *
 * Project     = White Board
 *
 * Description = Methods to handle different kind of Operations and updates sent over 
 *               the network.
 ***************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using PlexShareNetwork;
using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Server;
using Serializer = PlexShareWhiteboard.BoardComponents.Serializer;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel : INotificationHandler
    {
        /// <summary>
        ///     Main thread dispatcher to make changes to UI.
        /// </summary>
        private Dispatcher ApplicationMainThreadDispatcher =>
    (Application.Current?.Dispatcher != null) ?
        Application.Current.Dispatcher :
        Dispatcher.CurrentDispatcher;

        /// <summary>
        ///     Implementing Network module's OnDataReceived to handle serialized data which come over the network.
        /// </summary>
        /// <param name="serializedData">Json string to be deserialized</param>
        public void OnDataReceived(string serializedData)
        {
            _ = ApplicationMainThreadDispatcher.BeginInvoke(
                     DispatcherPriority.Normal,
                     new Action<string>(serializedData =>
                     {
                         lock (this)
                         {
                             DataHandler(serializedData);
                         }
                     })
                     ,
                     serializedData);
        }

        /// <summary>
        ///     Handles different operations of updates over the network.
        /// </summary>
        /// <param name="serializedData">Json string to be deserialized</param>
        public void DataHandler(string serializedData)
        {

            Serializer serializer = new Serializer();
            ServerSide serverSide = ServerSide.Instance;
            
            if (isServer)
            {
                try
                {
                    WBServerShape deserializedObject = serializer.DeserializeWBServerShape(serializedData);
                    List<ShapeItem> shapeItems = serializer.ConvertToShapeItem(deserializedObject.ShapeItems);
                    Trace.WriteLine("[Whiteboard] WBMessageHandler.onDataReceived(Server): Receiving the json string " + deserializedObject.Op);
                    switch (deserializedObject.Op)
                    {
                        case Operation.RestoreSnapshot:
                            List<ShapeItem> loadedBoard = serverSide.OnLoadMessage(deserializedObject.SnapshotNumber, deserializedObject.UserID);
                            LoadBoard(loadedBoard);
                            break;
                        case Operation.CreateSnapshot:
                            serverSide.CreateSnapshotHandler(deserializedObject);
                            UpdateCheckList(deserializedObject.SnapshotNumber);
                            break;
                        case Operation.Creation:
                            shapeItems[0].ZIndex = Math.Max(shapeItems[0].ZIndex, machine.GetMaxZindex(shapeItems[0]));
                            CreateIncomingShape(shapeItems[0]);
                            serverSide.OnShapeReceived(shapeItems[0], deserializedObject.Op);
                            break;
                        case Operation.Deletion:
                            DeleteIncomingShape(shapeItems[0]);
                            serverSide.OnShapeReceived(shapeItems[0], deserializedObject.Op);
                            break;
                        case Operation.ModifyShape:
                            ModifyIncomingShape(shapeItems[0]);
                            serverSide.OnShapeReceived(shapeItems[0], deserializedObject.Op);
                            break;
                        case Operation.Clear:
                            ShapeItems.Clear();
                            undoStack.Clear();
                            redoStack.Clear();
                            serverSide.OnShapeReceived(shapeItems[0], deserializedObject.Op);
                            break;
                        case Operation.NewUser:
                            LoadBoard(shapeItems, true);
                            serverSide.NewUserHandler(deserializedObject);
                            break;
                    }


                    Trace.WriteLine(
                        "[Whiteboard] WBMessageHandler.onDataReceived(Server): Took necessary actions on received object"
                    );
                }
                catch (Exception e)
                {
                    Trace.WriteLine("[Whiteboard] WBMessageHandler.onDataReceived(Server): Exception Occured");
                    Trace.WriteLine(e.Message);
                }
            }
            else
            {
                try
                {
                    var deserializedShape = serializer.DeserializeWBServerShape(serializedData);
                    List<ShapeItem> shapeItems = serializer.ConvertToShapeItem(deserializedShape.ShapeItems);
                    Trace.WriteLine("[Whiteboard] WBMessageHandler.onDataReceived(Client): Receiving the json string " + deserializedShape.Op);

                    switch (deserializedShape.Op)
                    {
                        case Operation.RestoreSnapshot:
                            LoadBoard(shapeItems);
                            break;
                        case Operation.CreateSnapshot:
                            UpdateCheckList(deserializedShape.SnapshotNumber);
                            //DisplayMessage(deserializedShape.UserID, deserializedShape.SnapshotNumber);
                            break;
                        case Operation.Creation:
                            CreateIncomingShape(shapeItems[0]);
                            break;
                        case Operation.Deletion:
                            DeleteIncomingShape(shapeItems[0]);
                            break;
                        case Operation.ModifyShape:
                            ModifyIncomingShape(shapeItems[0]);
                            break;
                        case Operation.Clear:
                            ShapeItems.Clear();
                            undoStack.Clear();
                            redoStack.Clear();
                            break;
                        case Operation.NewUser:
                            LoadBoard(shapeItems, true);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine("[Whiteboard] WBMessageHandler.onDataReceived(Client): Exception Occured");
                    Trace.WriteLine(e.Message);
                }
            }
        }

        public void DisplayMessage(string userID, int snapshotNumber)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Loads the list of shape items into the board
        /// </summary>
        /// <param name="shapeItems">List of shape items</param>
        /// <param name="isNewUser">Indicator of new user joining</param>
        public void LoadBoard(List<ShapeItem> shapeItems, bool isNewUser = false)
        {
            if (!isNewUser)
            {
                ShapeItems.Clear();
                undoStack.Clear();
                redoStack.Clear();
            }
            Trace.WriteLine("[Whiteboard] LoadBoard: Loading shapeItems " + shapeItems);
            if(shapeItems != null)
            {
                foreach (ShapeItem shapeItem in shapeItems)
                {
                    CreateIncomingShape(shapeItem);
                }
            }
        }
    }
}
