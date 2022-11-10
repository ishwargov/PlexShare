/// <author>Sughandhan S</author>
/// <created>11/11/2022</created>
/// <summary>
///     Unit Testing file for Utility Funtions
/// </summary>

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

            public static void DoEvent()
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
