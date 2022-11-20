/******************************************************************************
 * Filename    = ChatUtility.cs
 *
 * Author      = Sughandhan S
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareTests
 *
 * Description = Unit Testing file for Utility Funtions
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PlexShareTests.ContentTests.UX
{
    public class ChatUtility
    {
        public static class DispatcherUtil
        {
            private static object ExitFrame(object frame)
            {
                ((DispatcherFrame)frame).Continue = false;
                return null;
            }

            public static void DoEvents()
            {
                var frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(ExitFrame), frame);
                Dispatcher.PushFrame(frame);
            }
        }
    }
}
