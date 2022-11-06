/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains the definition of the class 'ReceiveQueueListener' which contains functions to spawn a thread to call module handlers
/// once packets appear in the receiving queue
/// </summary>

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace PlexShareNetwork.Queues
{
    public class ReceiveQueueListener
    {
        // Lock to ensure mutual exclusion while registering a module
        private readonly object _lock = new object();

        private Dictionary<string, INotificationHandler> _modulesToNotificationHandlerMap;
        private ReceivingQueue _receivingQueue;
        private bool _isRunning;

        // Constructor which is called by the Communicator
        public ReceiveQueueListener(Dictionary<string, INotificationHandler> modulesToNotificationHandlerMap, ReceivingQueue receivingQueue)
        {
            this._modulesToNotificationHandlerMap = modulesToNotificationHandlerMap;
            this._receivingQueue = receivingQueue;
        }

        /// <summary>
        /// Called by the Communicator submodule of each client in order to use queues
        /// </summary>
        public bool RegisterModule(string moduleName, INotificationHandler notificationHandler)
        {
            Trace.WriteLine("[Networking] ReceiveQueueListener.RegisterModule() function called.");
            bool isSuccessful = true;

            // Adding the priority of the module into the dictionary
            lock (_lock)
            {
                // If the module name is already taken
                if (_modulesToNotificationHandlerMap.ContainsKey(moduleName))
                    isSuccessful = false;
                else
                    _modulesToNotificationHandlerMap.Add(moduleName, notificationHandler);
            }

            return isSuccessful;
        }

        /// <summary>
        /// Called by the Communicator to start a thread for calling module handlers
        /// </summary>
        public void Start()
        {
            Trace.WriteLine("[Networking] ReceiveQueueListener.Start() function called.");
            ThreadStart listeningThreadRef = new ThreadStart(ListenOnQueue);

            // Creating a thread
            Thread listeningThread = new Thread(listeningThreadRef);

            // Starting the thread
            listeningThread.Start();

            // Declaring that the queue is running
            _isRunning = true;
        }

        /// <summary>
        /// Keep listening on the receiving queue and call the module's notification handlers if a packet appears in the queue
        /// </summary>
        private void ListenOnQueue()
        {
            Trace.WriteLine("[Networking] ReceiveQueueListener.ListenOnQueue() function called.");
            // Keep listening on the queue until the Communicator asks to stop
            while (_isRunning)
            {
                // Waiting for the receiving queue to contain a packet
                _receivingQueue.WaitForPacket();

                Packet packet = _receivingQueue.Dequeue();

                // Identifying the module which the packet belongs to
                string moduleName = packet.moduleOfPacket;

                if (!_modulesToNotificationHandlerMap.ContainsKey(moduleName))
                {
                    Trace.WriteLine($"Module {moduleName} does not have a handler.\n");
                    continue;
                }

                INotificationHandler notificationHandler = _modulesToNotificationHandlerMap[moduleName];

                // Calling the method 'OnDataReceived' on the handler of the appropriate module
                notificationHandler.OnDataReceived(packet.serializedData);
            }
        }

        /// <summary>
        /// Called by the Communicator to stop the thread that was run by the 'Start' function
        /// </summary>
        public void Stop()
        {
            Trace.WriteLine("[Networking] ReceiveQueueListener.Stop() function called.");
            // Stating to the thread to stop running
            _isRunning = false;
        }
    }
}
