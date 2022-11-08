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
