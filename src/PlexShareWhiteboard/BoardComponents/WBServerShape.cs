using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareWhiteboard.BoardComponents
{
    public class WBServerShape
    {
        public WBServerShape(List<ShapeItem> shapeItems, Operation op, string userID, int snapshotNumber = -1)
        {
            ShapeItems = shapeItems;
            Op = op;
            SnapshotNumber = snapshotNumber;
            UserID = userID;
        }

        public List<ShapeItem> ShapeItems { get; set; }
        public Operation Op { get; set; }
        public int SnapshotNumber { get; set; }
        public string UserID { get; set; }
    }
}
