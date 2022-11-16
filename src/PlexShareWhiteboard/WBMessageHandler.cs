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
        public ShapeItem sugu = null;
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
                                     WBServerShape deserializedObject = serializer.DeserializeWBServerShape(serializedData);
                                     List<ShapeItem> shapeItems = serializer.ConvertToShapeItem(deserializedObject.ShapeItems);
                                     Trace.WriteLine("ServerBoardCommunicator.onDataReceived: Receiving the XML string " + deserializedObject.Op);
                                     var userId = deserializedObject.UserID;
                                     switch (deserializedObject.Op)
                                     {
                                         case Operation.RestoreSnapshot:
                                             serverSide.RestoreSnapshotHandler(deserializedObject);
                                             LoadBoard(shapeItems);
                                             break;
                                         case Operation.CreateSnapshot:
                                             serverSide.CreateSnapshotHandler(deserializedObject);
                                             DisplayMessage(deserializedObject.UserID, deserializedObject.SnapshotNumber); //message that board number is saved
                                             break;
                                         case Operation.Creation:
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
                                             LoadBoard(shapeItems,true);
                                             serverSide.NewUserHandler(deserializedObject);
                                             break;
                                         default:
                                             Console.WriteLine("Unidentified Operation at ServerBoardCommunicator");
                                             break;
                                     }


                                     Trace.WriteLine(
                                         "WBMessageHandler.OnDataReceived: Took necessary actions on received object"
                                     );
                                 }
                                 catch (Exception e)
                                 {
                                     Trace.WriteLine("ServerBoardCommunicator.onDataReceived: Exception Occured");
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
                                     switch (deserializedShape.Op)
                                     {
                                         case Operation.RestoreSnapshot:
                                             LoadBoard(shapeItems);
                                             break;
                                         case Operation.CreateSnapshot:
                                             DisplayMessage(deserializedShape.UserID, deserializedShape.SnapshotNumber); //message that board number is saved
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
                                     Trace.WriteLine("[Whiteboard] OnDataReceived: Exception Occured");
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
            if(!isNewUser)
                ClearAllShapes();
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
