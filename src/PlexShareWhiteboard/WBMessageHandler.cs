/**
 * Owned By: Joel Sam Mathew
 * Created By: Joel Sam Mathew
 * Date Created: 22/10/2022
 * Date Modified: 08/11/2022
**/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using PlexShareNetwork;
using PlexShareNetwork.Communication;
using PlexShareNetwork.Serialization;
using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Server;
using Serializer = PlexShareWhiteboard.BoardComponents.Serializer;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel : INotificationHandler
    {
        private Dispatcher ApplicationMainThreadDispatcher =>
    (Application.Current?.Dispatcher != null) ?
        Application.Current.Dispatcher :
        Dispatcher.CurrentDispatcher;

        public void OnDataReceived(string serializedData)
        {
            _ = ApplicationMainThreadDispatcher.BeginInvoke(
                     DispatcherPriority.Normal,
                     new Action<string>(serializedData =>
                     {
                         lock (this)
                         {

                             Serializer serializer = new Serializer();
                             ServerSide serverSide = ServerSide.Instance;
                             ServerCommunicator serverCommunicator = ServerCommunicator.Instance;
                             if (isServer)
                             {
                                 try
                                 {
                                     Trace.WriteLine("[WhiteBoard] WBMessageHandler check0");
                                     WBServerShape deserializedObject = serializer.DeserializeWBServerShape(serializedData);
                                     Trace.WriteLine("[WhiteBoard] WBMessageHandler check1");
                                     List<ShapeItem> shapeItems = serializer.ConvertToShapeItem(deserializedObject.ShapeItems);
                                     Trace.WriteLine("[Whiteboard] WBMessageHandler.onDataReceived(Server): Receiving the json string " + deserializedObject.Op);
                                     
                                     //if(shapeItems.Count > 0)
                                     //    Trace.WriteLine("[Whiteboard] Abhm :" + shapeItems[0].TextString + shapeItems[0].Id);
                                     var userId = deserializedObject.UserID;
                                     switch (deserializedObject.Op)
                                     {
                                         case Operation.RestoreSnapshot:
                                             List<ShapeItem> loadedBoard = serverSide.OnLoadMessage(deserializedObject.SnapshotNumber, deserializedObject.UserID);
                                             LoadBoard(loadedBoard);
                                             break;
                                         case Operation.CreateSnapshot:
                                             serverSide.CreateSnapshotHandler(deserializedObject);
                                             //DisplayMessage(deserializedObject.UserID, deserializedObject.SnapshotNumber); 
                                             UpdateCheckList(deserializedObject.SnapshotNumber);
                                             break;
                                         case Operation.Creation:
                                             CreateIncomingShape(shapeItems[0]);
                                             Trace.WriteLine("[Whiteboard] Abhm :" + shapeItems[0].TextString + shapeItems[0].Id);
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
                                             LoadBoard(shapeItems,true);
                                             serverSide.NewUserHandler(deserializedObject);
                                             break;
                                         default:
                                             Console.WriteLine("[Whiteboard] WBMessageHandler.onDataReceived(Server): Unidentified Operation at ServerBoardCommunicator");
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
                                     Trace.WriteLine("[Whiteboard]  " + " Client msg received");
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
                                             DisplayMessage(deserializedShape.UserID, deserializedShape.SnapshotNumber); 
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
                                             LoadBoard(shapeItems,true);
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
                     })
                     ,
                     serializedData);
        }
        

        private void DisplayMessage(string userID, int snapshotNumber)
        {
            throw new NotImplementedException();
        }

        private void LoadBoard(List<ShapeItem> shapeItems, bool isNewUser = false)
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
