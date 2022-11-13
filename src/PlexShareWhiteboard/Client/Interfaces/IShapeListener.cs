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
 *               It provides an OnShapeReceived function to 
 *               receive a shape, implemented by both Server and Client.
 *               It is either given by ViewModel (if recepient is a Client) 
 *               or through network (if recepient is a Server).
 ***************************/

using PlexShareWhiteboard.BoardComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareWhiteboard.Client.Interfaces
{
    internal interface IShapeListener
    {
        void OnShapeReceived(ShapeItem newShape, Operation op);

        void SetUserId(string userId);
    }
}