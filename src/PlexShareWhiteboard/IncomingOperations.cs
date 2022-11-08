using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareWhiteboard.BoardComponents;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        public void CreateIncomingShape(ShapeItem newShape)
        {
            if (newShape == null)
                return;
            ShapeItems.Add(newShape);
        }

        public void ModifyIncomingShape(ShapeItem newShape)
        {
            if (newShape == null)
                return;
            for (int i = 0; i < ShapeItems.Count; ++i)
            {
                if (ShapeItems[i].Id == newShape.Id)
                    ShapeItems[i] = newShape;
            }
        }

        public void DeleteIncomingShape(ShapeItem oldShape)
        {
            if (oldShape == null)
                return;
            int i, flag = 0;
            for (i = 0; i < ShapeItems.Count; ++i)
            {
                if (ShapeItems[i].Id == oldShape.Id)
                {
                    flag = 1;
                    break;
                }
            }

            if (flag == 1)
                ShapeItems.Remove(ShapeItems[i]);
        }

        public void ClearAllShapes()
        {
            ShapeItems.Clear();
        }
    }
}
