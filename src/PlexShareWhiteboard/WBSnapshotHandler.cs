using PlexShareWhiteboard.BoardComponents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {

        public void SaveSnapshot()
        {
            int currSnapshotNumber = machine.OnSaveMessage(userId);
            CheckList.Add(currSnapshotNumber);

            UpdateCheckList(currSnapshotNumber);
        }
        public void UpdateCheckList(int n)
        {
            Trace.WriteLine("[Whiteboard] WBSnapshotHandler: Updating checklist with " + n);
            CheckList.Clear();
            for(int i = n; i>n-5 && i>0; i--)
            {
                CheckList.Add(i);
            }
            machine.SetSnapshotNumber(n);
        }

        public void LoadSnapshot(int snapshotNumber)
        {
            List<ShapeItem> shapeList = machine.OnLoadMessage(snapshotNumber, userId);
            ShapeItems.Clear();
            undoStack.Clear();
            redoStack.Clear();
            if (isServer)
            {
                ShapeItems.Clear();
                foreach(ShapeItem s in shapeList)
                    ShapeItems.Add(s);
            }
        }
    }
}
