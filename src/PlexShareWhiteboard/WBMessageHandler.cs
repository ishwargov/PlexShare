/**
 * Owned By: Joel Sam Mathew
 * Created By: Joel Sam Mathew
 * Date Created: 22/10/2022
 * Date Modified: 08/11/2022
**/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
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
        private bool IsServer()
        {
            throw new NotImplementedException();
        }

        public void OnDataReceived(string serializedData)
        {
            Serializer serializer = new Serializer();
            ServerSide serverSide = ServerSide.Instance;
            ServerCommunicator serverCommunicator = ServerCommunicator.Instance;
            if (IsServer())
            {
                try
                {
                    Trace.WriteLine("ServerBoardCommunicator.onDataReceived: Receiving the XML string");
                    WBServerShape deserializedObject = serializer.DeserializeWBServerShape(serializedData);
                    List<ShapeItem> shapeItems = serializer.ConvertToShapeItem(deserializedObject.ShapeItems);
                    var userId = deserializedObject.UserID;
                    switch (deserializedObject.Op)
                    {
                        case Operation.RestoreSnapshot:
                            serverSide.RestoreSnapshotHandler(deserializedObject);
                            break;
                        case Operation.CreateSnapshot:
                            serverSide.CreateSnapshotHandler(deserializedObject);
                            break;
                        case Operation.Creation:
                            serverSide.OnShapeReceived(shapeItems[0], deserializedObject.Op);
                            break;
                        case Operation.Deletion:
                            serverSide.OnShapeReceived(shapeItems[0], deserializedObject.Op);
                            break;
                        case Operation.ModifyShape:
                            serverSide.OnShapeReceived(shapeItems[0], deserializedObject.Op);
                            break;
                        case Operation.Clear:
                            serverSide.OnShapeReceived(shapeItems[0], deserializedObject.Op);
                            break;
                        case Operation.NewUser:
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
                            ClearAllShapes();
                            break;
                        case Operation.NewUser:
                            LoadBoard(shapeItems);
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

        private void DisplayMessage(string userID, int snapshotNumber)
        {
            throw new NotImplementedException();
        }

        private void LoadBoard(List<ShapeItem> shapeItems)
        {
            ClearAllShapes();
            foreach (ShapeItem shapeItem in shapeItems)
            {
                CreateIncomingShape(shapeItem);
            }
        }
    }
}
