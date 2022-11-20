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
    public interface IServerSnapshotHandler
    {
        public List<ShapeItem> LoadBoard(int snapshotNumber);
        public int SaveBoard(List<ShapeItem> boardShapes, string userID);
    }
}
