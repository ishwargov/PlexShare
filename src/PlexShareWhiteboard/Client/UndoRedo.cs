using PlexShareWhiteboard.BoardComponents;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PlexShareWhiteboard.Client
{
    public class UndoRedo
    {
        private Stack<List<UndoStackElement>> undoStack; // Undo Stack
        private Stack<List<UndoStackElement>> redoStack; // Redo Stack
        ClientCommunicator clientCommunicator;

        // private static maxCapacity = 50;
        // private int userId;

        public UndoRedo()
        {
            ClientCommunicator clientCommunicator = new ClientCommunicator();
            undoStack = new Stack<List<UndoStackElement>>();
            redoStack = new Stack<List<UndoStackElement>>();
        }

        /// <summary>
        /// To Create an object again (when undo/redo is pressed)
        /// Set Operation as Creation and send to server
        /// </summary>
        private UndoStackElement Create(UndoStackElement obj)
        {
            obj.Op = Operation.Creation;
            //clientCommunicator.SendToServer(obj.NewShape,obj.Op);
            return obj;
        }

        /// <summary>
        /// To Delete an object (when undo/redo is pressed)
        /// Set Operation as Deletion and send to server
        /// </summary>
        private UndoStackElement Delete(UndoStackElement obj)
        {
            obj.Op = Operation.Deletion;
            //clientCommunicator.SendToServer(obj.NewShape,obj.Op);
            return obj;
        }

        /// <summary>
        /// If the top of the Stack has ModifyShape Operation
        /// 1. Undo should restore it to its previous (oldShape) form
        /// 2. Redo should redo the modify operation, i.e., change the object to its new form (newShape)
        /// And the corresponding shapes should be send to the Server with the operation ModifyShape
        /// </summary>
        private UndoStackElement ChangeShape(UndoStackElement obj, string TypeOfStack)
        {
            obj.Op = Operation.ModifyShape;
            //if(TypeOfStack == "Undo")
            //	clientCommunicator.SendToServer(obj.PrvShape,obj.Op);
            //else
            //              clientCommunicator.SendToServer(obj.NewShape,obj.Op);
            return obj;
        }

        /// <summary>
        /// If the operation performed is clear
        /// 1. If Undo then UndoClear should be passed to the server, which will
        /// then broadcast the list of objects in the board, before clear was used.
        /// Then we push it to the Redo Stack with Operation as Clear
        /// 2. If Redo then Clear should be passed to the
        /// </summary>
        private UndoStackElement ClearBoard(UndoStackElement obj)
        {
            //clientCommunicator.SendToServer(obj.PrvShape,obj.Op);
            obj.Op = Operation.Clear;
            return obj;
        }

        public void Undo()
        {
            List<UndoStackElement> topOfStack = undoStack.Pop();
            List<UndoStackElement> modifiedObjects = new List<UndoStackElement>();
            for (int i = 0; i < topOfStack.Count; i++)
            {
                UndoStackElement obj = topOfStack[i];
                UndoStackElement newObj = new UndoStackElement(); // object to be pushed on to redo stack

                switch (obj.Op)
                {
                    case Operation.Creation:
                        newObj = Delete(obj);
                        newObj.Op = Operation.Creation;
                        break;
                    case Operation.Deletion:
                        newObj = Create(obj);
                        newObj.Op = Operation.Creation;
                        break;
                    case Operation.ModifyShape:
                        newObj = ChangeShape(obj, "Undo");
                        break;
                    case Operation.Clear:
                        obj.Op = Operation.Clear;
                        newObj = ClearBoard(obj);
                        break;
                }
                modifiedObjects.Add(newObj);
            }
            redoStack.Push(modifiedObjects);
        }

        public void Redo()
        {
            List<UndoStackElement> topOfStack = redoStack.Pop();
            List<UndoStackElement> modifiedObjects = new List<UndoStackElement>();
            for (int i = 0; i < topOfStack.Count; i++)
            {
                UndoStackElement obj = topOfStack[i];
                UndoStackElement newObj = new UndoStackElement();

                switch (obj.Op)
                {
                    case Operation.Creation:
                        newObj = Create(obj);
                        break;
                    case Operation.Deletion:
                        newObj = Delete(obj);
                        break;
                    case Operation.ModifyShape:

                        newObj = ChangeShape(obj, "Redo");
                        break;
                    case Operation.Clear:
                        newObj = ClearBoard(obj);
                        break;
                }
                modifiedObjects.Add(newObj);
            }
            undoStack.Push(modifiedObjects);
        }

        public void InsertIntoStack(UndoStackElement obj)
        {
            // Stack size constraint required ?
            List<UndoStackElement> newElement = new List<UndoStackElement>();
            newElement.Add(obj);

            undoStack.Push(newElement);
        }

        //void removeObject(Object  objectId);
    }
}
