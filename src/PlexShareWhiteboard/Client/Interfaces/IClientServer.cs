using PlexShareWhiteboard.BoardComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareWhiteboard.Client.Interfaces
{
    internal interface IClientServer
    {
        void OnShapeReceived(ShapeItem newShape, Operation op);

    }
}