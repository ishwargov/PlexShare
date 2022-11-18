/// <author>Mayank Singla</author>
/// <summary>
/// Defines the "ScreenshareServer" class which represents the
/// data model for screen sharing on the server side machine.
/// </summary>

using PlexShareNetwork;
using PlexShareNetwork.Communication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Timers;

namespace PlexShareScreenshare.Server
{
    /// <summary>
    /// Represents the data model for screen sharing on the server side machine.
    /// </summary>
    public class ScreenshareServer :
        INotificationHandler, // To receive packets from the networking
        ITimerManager,        // Handles the timeout for screen sharing of clients
        IDisposable           // Handle cleanup work for the allocated resources
    {
        /// <summary>
        /// The only singleton instance for this class.
        /// </summary>
        private static ScreenshareServer? _instance;

        /// <summary>
        /// The networking object used to subscribe to the networking module
        /// and to send the packets to the clients.
        /// </summary>
        private readonly ICommunicator? _communicator;

        /// <summary>
        /// The subscriber which should be notified when subscribers list change.
        /// Here it will be the view model.
        /// </summary>
        private readonly IMessageListener _listener;

        /// <summary>
        /// The map between each client ID and their corresponding "SharedScreenObject"
        /// to keep track of all the active subscribers (screen sharers).
        /// </summary>
        private readonly Dictionary<string, SharedClientScreen> _subscribers;

        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Creates an instance of "ScreenshareServer" which represents the
        /// data model for screen sharing on the server side machine.
        /// </summary>
        /// <param name="listener">
        /// The subscriber which should be notified when subscribers list change
        /// </param>
        /// <param name="isDebugging">
        /// If we are in debugging mode
        /// </param>
        protected ScreenshareServer(IMessageListener listener, bool isDebugging)
        {
            if (!isDebugging)
            {
                // Get an instance of a communicator object
                _communicator = CommunicationFactory.GetCommunicator(isClientSide: false);

                // Subscribe to the networking module for packets
                _communicator.Subscribe(Utils.ModuleIdentifier, this, isHighPriority: true);
            }

            // Initialize the rest of the fields
            _subscribers = new();
            _listener = listener;
            _disposed = false;

            Trace.WriteLine(Utils.GetDebugMessage("Successfully created an instance of ScreenshareServer", withTimeStamp: true));
        }

        /// <summary>
        /// Destructor for the class that will perform some cleanup tasks.
        /// This destructor will run only if the Dispose method does not get called.
        /// It gives the class the opportunity to finalize.
        /// </summary>
        ~ScreenshareServer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(disposing: false) is optimal in terms of
            // readability and maintainability.
            Dispose(disposing: false);
        }

        /// <summary>
        /// Implements "INotificationHandler". It will be invoked when a data packet
        /// comes for the screen share module from the client to the server. Based on
        /// the header in the packet received, it will do further processing.
        /// </summary>
        /// <param name="serializedData">
        /// Data received inside the packet from the client
        /// </param>
        public void OnDataReceived(string serializedData)
        {
            try
            {
                DataPacket? packet = JsonSerializer.Deserialize<DataPacket>(serializedData);

                if (packet == null)
                {
                    Trace.WriteLine(Utils.GetDebugMessage($"Not able to deserialize data packet: {serializedData}", withTimeStamp: true));
                    return;
                }

                // Extract different fields from the object of the "DataPacket"
                string clientId = packet.Id;
                string clientName = packet.Name;
                ClientDataHeader header = Enum.Parse<ClientDataHeader>(packet.Header);
                string clientData = packet.Data;

                // Based on the packet header, do further processing
                switch (header)
                {
                    case ClientDataHeader.Register:
                        RegisterClient(clientId, clientName);
                        break;
                    case ClientDataHeader.Deregister:
                        DeregisterClient(clientId);
                        break;
                    case ClientDataHeader.Image:
                        PutImage(clientId, clientData);
                        break;
                    case ClientDataHeader.Confirmation:
                        UpdateTimer(clientId);
                        break;
                    default:
                        throw new Exception($"Unknown header {packet.Header}");
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Exception while processing the packet: {e.Message}", withTimeStamp: true));
            }
        }

        /// <summary>
        /// Implements "INotificationHandler". Not required by the screen share server module.
        /// </summary>
        /// <remarks>
        /// Being an interface method, it can't be marked as static.
        /// </remarks>
#pragma warning disable CA1822 // Mark members as static
        public void OnClientJoined<T>(T _) { }
#pragma warning restore CA1822 // Mark members as static

        /// <summary>
        /// Implements "INotificationHandler". It is invoked by the Networking Communicator
        /// when a client leaves the meeting.
        /// </summary>
        public void OnClientLeft(string clientId)
        {
            Debug.Assert(_subscribers != null, Utils.GetDebugMessage("_subscribers is found null"));

            // Deregister the client if it was sharing screen
            if (_subscribers.ContainsKey(clientId))
            {
                DeregisterClient(clientId);
            }
        }

        /// <summary>
        /// Implements "ITimerManager". Callback which will be invoked when the timeout occurs for the
        /// CONFIRMATION packet not received by the client.
        /// </summary>
        /// <param name="source">
        /// Default argument passed by the "Timer" class
        /// </param>
        /// <param name="e">
        /// Default argument passed by the "Timer" class
        /// </param>
        /// <param name="clientId">
        /// The Id of the client for which the timeout occurred
        /// </param>
        public void OnTimeOut(object? source, ElapsedEventArgs e, string clientId)
        {
            DeregisterClient(clientId);
            Trace.WriteLine(Utils.GetDebugMessage($"Timeout occurred for the client with id: {clientId}", withTimeStamp: true));
        }

        /// <summary>
        /// Implement "IDisposable". Disposes the managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, we should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time
            GC.SuppressFinalize(this);
        }

        /// /// <summary>
        /// Gets a singleton instance of "ScreenshareServer" class.
        /// </summary>
        /// <param name="listener">
        /// The subscriber which should be notified when subscribers list change
        /// </param>
        /// <param name="isDebugging">
        /// If we are in debugging mode or not
        /// </param>
        /// <returns>
        /// A singleton instance of "ScreenshareServer"
        /// </returns>
        public static ScreenshareServer GetInstance(IMessageListener listener, bool isDebugging = false)
        {
            Debug.Assert(listener != null, Utils.GetDebugMessage("listener is found null"));

            // Create a new instance if it was null before
            _instance ??= new(listener, isDebugging);
            return _instance;
        }

        /// <summary>
        /// Used to send various data packets to the clients.
        /// Also provide them the resolution of the image to send if asking
        /// the clients to send the image packet.
        /// </summary>
        /// <param name="clientIds">
        /// Client IDs to send the information
        /// </param>
        /// <param name="header">
        /// Corresponding header to send with the data packet.
        /// Should be a string value of the enum "ServerDataHeader"
        /// </param>
        /// <param name="numRowsColumns">
        /// Resolution of the image based on the number of rows and columns
        /// to send if asking the clients to send image packet
        /// </param>
        public void BroadcastClients(List<string> clientIds, string headerVal, (int Rows, int Cols) numRowsColumns)
        {
            Debug.Assert(_communicator != null, Utils.GetDebugMessage("_communicator is found null"));
            Debug.Assert(clientIds != null, Utils.GetDebugMessage("list of client Ids is found null"));

            // If there are no clients to broadcast to
            if (clientIds.Count == 0) return;

            // Validate header value
            try
            {
                ServerDataHeader _ = Enum.Parse<ServerDataHeader>(headerVal);
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to parse the header {headerVal} : {e.Message}", withTimeStamp: true));
                return;
            }

            // Serialize the data to send
            try
            {
                int product = numRowsColumns.Rows * numRowsColumns.Cols;
                string serializedData = JsonSerializer.Serialize(product);

                // Create the data packet to send
                DataPacket packet = new("1", "Server", headerVal, serializedData);

                // Serialize the data packet to send to clients
                string serializedPacket = JsonSerializer.Serialize(packet);

                // Send data packet to all the clients mentioned
                foreach (string clientId in clientIds)
                {
                    _communicator.Send(serializedPacket, Utils.ModuleIdentifier, clientId);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Exception while sending the packet to the client: {e.Message}", withTimeStamp: true));
            }
        }

        /// <summary>
        /// It executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the destructor and we should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing">
        /// Indicates if we are disposing this object
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (_disposed) return;

            // If disposing equals true, dispose all managed
            // and unmanaged resources
            if (disposing)
            {
                foreach (SharedClientScreen client in _subscribers.Values.ToList())
                {
                    DeregisterClient(client.Id);
                }
                _subscribers.Clear();
                _instance = null;
            }

            // Call the appropriate methods to clean up unmanaged resources here

            // Now disposing has been done
            _disposed = true;
        }

        /// <summary>
        /// Add this client to list of screen sharers. It also notifies the view
        /// model that a new client has started presenting screen.
        /// </summary>
        /// <param name="clientId">
        /// Id of the client to add to the screen sharers list
        /// </param>
        /// <param name="clientName">
        /// Name of the client to add to the screen sharers list
        /// </param>
        private void RegisterClient(string clientId, string clientName)
        {
            Debug.Assert(_subscribers != null, Utils.GetDebugMessage("_subscribers is found null"));

            // Acquire lock because timer threads could also execute simultaneously
            lock (_subscribers)
            {
                // Check if the clientId is present in the screen sharers list
                if (_subscribers.ContainsKey(clientId))
                {
                    Trace.WriteLine(Utils.GetDebugMessage($"Trying to register an already registered client with id {clientId}", withTimeStamp: true));
                    return;
                }

                try
                {
                    // Add this client to the list of screen sharers
                    _subscribers.Add(clientId, new(clientId, clientName, this));
                }
                catch (Exception e)
                {
                    Trace.WriteLine(Utils.GetDebugMessage($"Error adding client to the list of screen sharers: {e.Message}", withTimeStamp: true));
                    return;
                }

                NotifyUX();
            }

            Trace.WriteLine(Utils.GetDebugMessage($"Successfully registered the client- Id: {clientId}, Name: {clientName}", withTimeStamp: true));
        }

        /// <summary>
        /// Remove this client from the list of screen sharers. It also
        /// asks the client object to stop all its processing and notify the
        /// view model that a client has stopped screen sharing.
        /// </summary>
        /// <param name="clientId">
        /// Id of the client to remove from the screen sharer list
        /// </param>
        private void DeregisterClient(string clientId)
        {
            Debug.Assert(_subscribers != null, Utils.GetDebugMessage("_subscribers is found null"));

            SharedClientScreen client;

            // Acquire lock because timer threads could also execute simultaneously
            lock (_subscribers)
            {
                // Check if the clientId is present in the screen sharers list
                if (!_subscribers.ContainsKey(clientId))
                {
                    Trace.WriteLine(Utils.GetDebugMessage($"Trying to deregister a client with id {clientId} which is not present in subscribers list", withTimeStamp: true));
                    return;
                }

                // Remove the client from the list of screen sharers
                client = _subscribers[clientId];
                _ = _subscribers.Remove(clientId);

                NotifyUX();
            }

            // Stop all processing for this client
            try
            {
                client.StopProcessing().Wait();
            }
            catch (OperationCanceledException e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Task canceled for the client with id {clientId}: {e.Message}", withTimeStamp: true));
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to stop the task for the removed client with id {clientId}: {e.Message}", withTimeStamp: true));
            }
            finally
            {
                client.Dispose();
            }

            Trace.WriteLine(Utils.GetDebugMessage($"Successfully removed the client with Id {clientId}", withTimeStamp: true));
        }

        /// <summary>
        /// Adds the image received from the client to the client's image queue.
        /// </summary>
        /// <param name="clientId">
        /// Client Id for which the image arrived
        /// </param>
        /// <param name="data"></param>
        private void PutImage(string clientId, string data)
        {
            Debug.Assert(_subscribers != null, Utils.GetDebugMessage("_subscribers is found null"));

            // Check if the clientId is present in the screen sharers list
            if (!_subscribers.ContainsKey(clientId))
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Client with id {clientId} is not present in subscribers list", withTimeStamp: true));
                return;
            }

            // Put the image to the client's image queue
            try
            {
                //Frame frame = JsonSerializer.Deserialize<Frame>(data);
                SharedClientScreen client = _subscribers[clientId];
                client.PutImage(data);
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Exception while processing the received frame: {e.Message}", withTimeStamp: true));
            }
        }

        /// <summary>
        /// Reset the timer for the client.
        /// </summary>
        /// <param name="clientId">
        /// Id of the client for which the timer needs to be reset
        /// </param>
        private void UpdateTimer(string clientId)
        {
            Debug.Assert(_subscribers != null, Utils.GetDebugMessage("_subscribers is found null"));

            // Check if the clientId is present in the screen sharers list
            if (!_subscribers.ContainsKey(clientId))
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Client with id {clientId} is not present in subscribers list", withTimeStamp: true));
                return;
            }

            // Reset the timer for the client
            try
            {
                SharedClientScreen client = _subscribers[clientId];
                client.UpdateTimer();

                // Send Confirmation packet back to the client
                List<string> clientIds = new()
                {
                    clientId
                };
                //BroadcastClients(clientIds, nameof(ServerDataHeader.Confirmation), (0, 0));
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to update the timer for the client with id {clientId}: {e.Message}", withTimeStamp: true));
            }
        }

        /// <summary>
        /// Notifies the view model with the updates list of screen sharers.
        /// </summary>
        private void NotifyUX()
        {
            Debug.Assert(_subscribers != null, Utils.GetDebugMessage("_subscribers is found null"));
            Debug.Assert(_listener != null, Utils.GetDebugMessage("_listener is found null"));

            _listener.OnSubscribersChanged(_subscribers.Values.ToList());
        }
    }
}
