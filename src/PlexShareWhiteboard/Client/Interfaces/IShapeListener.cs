/***************************
 * Filename    = WhiteBoardViewModel.cs
 *
 * Author      = Aiswarya H
 *
 * Product     = Plex Share
 * 
 * Project     = White Board
 *
 * Description = This is the IShapeListener Interface. 
 *               It provides all the functions that must be implemented by both Server and Client
 *               machines. (Especially receiving a shape and receiving messages - NewUser, Load, Save )
 ***************************/

using PlexShareWhiteboard.BoardComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareWhiteboard.Client.Interfaces
{
    /// <summary>
    ///         Implements all the functions required for a machine (Client or Server).
    /// </summary>
    public interface IShapeListener
    {
        void OnShapeReceived(ShapeItem newShape, Operation op);
        void SetUserId(string userId);
        public int OnSaveMessage(string userId);
        public List<ShapeItem> OnLoadMessage(int snapshotNumber, string userId);
        public void SetSnapshotNumber(int snapshotNumber);
        public int GetMaxZindex(ShapeItem lastShape);
    }
}