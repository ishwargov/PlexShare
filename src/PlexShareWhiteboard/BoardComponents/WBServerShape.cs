/**
 * Owned By: Joel Sam Mathew
 * Created By: Joel Sam Mathew
 * Date Created: 22/10/2022
 * Date Modified: 08/11/2022
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareWhiteboard.BoardComponents
{
    public class WBServerShape
    {
        public WBServerShape(List<SerializableShapeItem> shapeItems, Operation op, string userID = "1", int snapshotNumber = -1)
        {
            ShapeItems = shapeItems;
            Op = op;
            SnapshotNumber = snapshotNumber;
            UserID = userID;
        }

        public List<SerializableShapeItem> ShapeItems { get; set; }
        public Operation Op { get; set; }
        public int SnapshotNumber { get; set; }
        public string UserID { get; set; }
    }
}
