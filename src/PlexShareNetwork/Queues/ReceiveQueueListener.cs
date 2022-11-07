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
        private readonly object _mapLock = new object();

        // Lock to ensure mutual exclusion while using the variable 'isRunning'
        private readonly object _isRunningLock = new object();

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
            lock (_mapLock)
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

            // Declaring that the queue is running
            lock (_isRunningLock)
            {
                _isRunning = true;
            }

            // Starting the thread
            listeningThread.Start();
        }

        /// <summary>
        /// Keep listening on the receiving queue and call the module's notification handlers if a packet appears in the queue
        /// </summary>
        private void ListenOnQueue()
        {
            Trace.WriteLine("[Networking] ReceiveQueueListener.ListenOnQueue() function called.");
            // Keep listening on the queue until the Communicator asks to stop
            while (true)
            {
                bool isThreadRunning = false;

                // Checking if the thread has to keep running
                lock (_isRunningLock)
                {
                    isThreadRunning = _isRunning;
                }

                // If the thread needs to be stopped
                if (!isThreadRunning)
                    break;

                // Waiting for the receiving queue to contain a packet
                _receivingQueue.WaitForPacket();

                Packet packet = _receivingQueue.Dequeue();

                // Identifying the module which the packet belongs to
                string moduleName = packet.moduleOfPacket;

                bool isModuleRegistered = false;

                // Finding if the module is registered
                lock (_mapLock)
                {
                    isModuleRegistered = _modulesToNotificationHandlerMap.ContainsKey(moduleName);
                }

                // There is nothing to do if the module is not registered
                if (!isModuleRegistered)
                {
                    Trace.WriteLine($"Module {moduleName} does not have a handler.\n");
                    continue;
                }

                INotificationHandler notificationHandler = null;

                // Getting the notification handler
                lock (_mapLock)
                {
                    notificationHandler = _modulesToNotificationHandlerMap[moduleName];
                }

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
            lock (_isRunningLock)
            {
                _isRunning = false;
            }
        }
    }
}
