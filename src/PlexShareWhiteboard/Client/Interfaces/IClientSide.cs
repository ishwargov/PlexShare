using PlexShareWhiteboard.BoardComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareWhiteboard.Client.Interfaces
{
    internal interface IClientSide
    {
        void OnShapeReceiveFromVM(ShapeItem oldShape, ShapeItem newShape, Operation op);

        void SendToServer(ShapeItem boardShape, Operation op);

        void ListenFromServer(List<ShapeItem> ShapeList, Operation op);

    }
}
