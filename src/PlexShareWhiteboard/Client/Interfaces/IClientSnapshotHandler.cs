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

namespace PlexShareWhiteboard.Client.Interfaces
{
    internal interface IClientSnapshotHandler
    {
        int SnapshotNumber { get; set; }
        void SaveSnapshot(string UserId);
        void RestoreSnapshot(int snapshotNumber, string UserId);
    }
}
