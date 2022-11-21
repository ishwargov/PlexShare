/*****************************
 * Filename    = IncomingOperations.cs
 *
 * Author      = Aiswarya H
 *
 * Product     = Plex Share
 * 
 * Project     = White Board
 *
 * Description = This is part of View Model.
 *               This contains all the operations to update the shape list when
 *               called by undo redo.
 ***************************/

using PlexShareWhiteboard.BoardComponents;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        /// <summary>
        /// this function is called when a shape is broad casted for creation
        /// it is checked if the shape is already existing (it is already existing for the client that send it to the server)
        /// if it is not existing, it is added
        /// if it is existing, it is removed and added, this is to ensure proper rendering
        /// </summary>
        /// <param name="newShape"></param>
        public void CreateIncomingShape(ShapeItem newShape)
        {
            if (newShape == null)
                return;

            int i, flag = 0;

            for (i = 0; i < ShapeItems.Count; ++i)
            {
                if (ShapeItems[i].Id == newShape.Id)
                {
                    flag = 1;
                    break;
                }
            }

            if(flag == 0)
                ShapeItems.Add(newShape);
            else
            {
                ShapeItems.RemoveAt(i); 
                ShapeItems.Add(newShape);
            }
        }

        /// <summary>
        /// this function is called when a shape is broad casted for modification
        /// the object in the list with same id is taken and updated with the new shape
        /// </summary>
        /// <param name="newShape"></param>
        public void ModifyIncomingShape(ShapeItem newShape)
        {
            if (newShape == null)
                return;

            int flag = 0;
            for (int i = 0; i < ShapeItems.Count; ++i)
            {
                if (ShapeItems[i].Id == newShape.Id)
                {
                    flag = 1;
                    ShapeItems.RemoveAt(i);
                    break;
                }
            }
            
            if (flag == 1)
                ShapeItems.Add(newShape);
        }

        /// <summary>
        /// this function is called when a shape is broad casted for deletion
        /// the object in the list with same id as that of new shape is taken and deleted if existing
        /// </summary>
        /// <param name="oldShape"></param>
        public void DeleteIncomingShape(ShapeItem oldShape)
        {
            if (oldShape == null)
                return;
       
            for (int i = 0; i < ShapeItems.Count; ++i)
            {
                if (ShapeItems[i].Id == oldShape.Id)
                {
                    ShapeItems.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// This function is called for broadcasting clear as well as for call from view to clear all shapes
        /// 
        /// </summary>
        public void ClearAllShapes()
        {
            ShapeItems.Clear();
            undoStack.Clear();
            redoStack.Clear();
            if(machine!=null)
                machine.OnShapeReceived(lastShape, Operation.Clear);
        }
    }
}
