using PlexShareWhiteboard.BoardComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareWhiteboard.Server.Interfaces
{
    internal interface IServerCommunicator
    {
        void Broadcast(ShapeItem newShape, Operation op);

        void Broadcast(List<ShapeItem> newShapes, Operation op);
    }
}
