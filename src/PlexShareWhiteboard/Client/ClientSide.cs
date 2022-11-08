using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareWhiteboard.BoardComponents;
using PlexShareWhiteboard.Client.Interfaces;

namespace PlexShareWhiteboard.Client
{
    internal class ClientSide : IClientSide
    {

        public UndoRedo uR;
        IClientCommunicator communicator;
        IClientSnapshotHandler snapshotHandler;
        //WBViewModel viewmodel = new WBViewModel();
        public ClientSide()
        {
            uR = new UndoRedo();
            communicator = new ClientCommunicator();
            snapshotHandler = new ClientSnapshotHandler();
        }

        /// <summary>	
        /// When the View Model sends an object to the Client Side
        /// 1. Create a WhiteBoardObject (i.e. to combine ShapeItem with its Operation)
        /// 2. Insert the WhiteBoardObject into Undo Stack
        /// 3. Send the received shape to server
        /// </summary>
        public void OnShapeReceiveFromVM(ShapeItem oldShape, ShapeItem newShape, Operation op)
        {

            // adding to undo
            UndoStackElement ShapeWithOp = new UndoStackElement
            {
                PrvShape = oldShape,
                NewShape = newShape,
                Op = op
            };
            uR.InsertIntoStack(ShapeWithOp);


            // Send to server
            SendToServer(newShape, op);

        }

        public void OnNewUserJoinMessage(string message, string ipAddress)
        {
            communicator.SendMessageToServer(message, ipAddress);
        }

        public void OnSaveMessage(string userId)
        {
            snapshotHandler.SaveSnapshot(userId);
        }

        public void OnLoadMessage(string userId, int snapshotNumber)
        {
            snapshotHandler.RestoreSnapshot(snapshotNumber, userId);
        }

        // To send the ShapeItem along with operation to the server side
        public void SendToServer(ShapeItem boardShape, Operation op)
        {
            List<ShapeItem> shapeItems = new List<ShapeItem>
            {
                boardShape
            };
            WBServerShape wBServerShape = new WBServerShape(shapeItems, op, boardShape.Id);
            communicator.SendToServer(wBServerShape);
        }

        // When a client receives a new shape from the server
        public void ListenFromServer(List<ShapeItem> ShapeList, Operation op)
        {

            // It is assumed that the server broadcasts a List of ShapeItems i.e.
            // ShapeList and the type of operation op

            // To send the object to view model and update Observable Collection
            // of Shape Items
            switch (op)
            {
                case Operation.UndoClear:
                    //foreach (var item in ShapeList)
                    //viewmodel.CreateIncomingShape(item);
                    break;
                case Operation.Clear:
                    //viewmodel.ClearAllShapes();
                    break;
                case Operation.Creation:
                    //viewmodel.CreateIncomingShape(ShapeList[0]);
                    break;
                case Operation.Deletion:
                    //viewmodel.DeleteIncomingShape(ShapeList[0]);
                    break;
                case Operation.ModifyShape:
                    //viewmodel.ModifyIncomingShape(ShapeList[0]);
                    break;
            }
        }

    }
}