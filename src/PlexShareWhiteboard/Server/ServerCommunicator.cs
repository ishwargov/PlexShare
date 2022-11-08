using PlexShareNetwork;
using PlexShareWhiteboard.BoardComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareNetwork;
using PlexShareWhiteboard.Server.Interfaces;
using PlexShareNetwork.Communication;
using PlexShareNetwork.Serialization;
using System.Diagnostics;
using PlexShareWhiteboard.Client;
using Serializer = PlexShareWhiteboard.BoardComponents.Serializer;

namespace PlexShareWhiteboard.Server
{
    internal class ServerCommunicator: IServerCommunicator
    {
        private static ServerCommunicator instance;
        private static Serializer serializer;
        private static ICommunicator communicator;
        private static readonly string moduleIdentifier = "Whiteboard";

        public static ServerCommunicator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServerCommunicator();
                    serializer = new Serializer();
                    communicator = CommunicationFactory.GetCommunicator(false);
                }

                return instance;
            }
        }


        public void Broadcast(ShapeItem newShape, Operation op)
        {
            List<ShapeItem> newShapeList = new List<ShapeItem>();
            newShapeList.Add(newShape);
            Broadcast(newShapeList, op);
        }

        public void Broadcast(List<ShapeItem> newShapes, Operation op)
        {
            List<SerializableShapeItem> newSerializableShapes = serializer.ConvertToSerializableShapeItem(newShapes);
            WBServerShape clientUpdate = new WBServerShape(newSerializableShapes, op);
            Broadcast(clientUpdate);
        }

        public void Broadcast(WBServerShape clientUpdate)
        {
            try
            {
                Trace.WriteLine("[Whiteboard] ClientCommunicator.Send: Sending objects to server");
                var serializedObj = serializer.SerializeWBServerShape(clientUpdate);
                communicator.Send(serializedObj, moduleIdentifier, null);
                Trace.WriteLine("[Whiteboard] ClientCommunicator.Send: Sent objects to server");
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Whiteboard] ClientCommunicator.Send: Exception Occured");
                Trace.WriteLine(e.Message);
            }
        }
    }
}
