/**
 * Owned By: Joel Sam Mathew
 * Created By: Joel Sam Mathew
 * Date Created: 22/10/2022
 * Date Modified: 08/11/2022
**/

using PlexShareWhiteboard.BoardComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareWhiteboard.Server.Interfaces
{
    public interface IServerCommunicator
    {
        public void Broadcast(ShapeItem newShape, Operation op);

        public void Broadcast(List<ShapeItem> newShapes, Operation op);

        public void Broadcast(WBServerShape clientUpdate, string? userID);
    }
}
