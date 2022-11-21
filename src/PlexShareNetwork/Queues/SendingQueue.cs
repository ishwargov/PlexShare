/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains the class definition of sending queues, in which one is a high priority queue and the other is a low priority
/// queue
/// </summary>

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace PlexShareNetwork.Queues
{
    public class SendingQueue
    {
        private readonly IQueue _highPriorityQueue = new Queue();
        private readonly IQueue _lowPriorityQueue = new Queue();

        // A dictionary to map each registered module to a priority value (true means it is of high priority)
        private Dictionary<string, bool> _modulesToPriorityMap = new Dictionary<string, bool>();

        // Lock to ensure mutual exclusion while adding to the dictionary
        private readonly object _lock = new object();

        // This value is used to determine which queue is going to be dequeued
        private int _priorityValue = 0;

        // Stores the ratio of priority of the two queues
        private readonly static int _highPriorityValue = 2;
        private readonly static int _lowPriorityValue = 1;

        // Stores the sums of priorities, which is used in the 'Dequeue' function
        private readonly static int _totalRatio = _highPriorityValue + _lowPriorityValue;

        /// <summary>
        /// Called by the Communicator submodule of each client in order to use queues
        /// </summary>
        /// <param name="moduleName">
        /// Name of the module to register
        /// </param>
        /// <param name="isHighPriority">
        /// 'bool : true' if the packet is of high priority, and 'bool : false' if not
        /// </param>
        /// <returns>
        /// 'bool : true' if the module is successfully registered, else 'bool : false'
        /// </returns>
        public bool RegisterModule(string moduleName, bool isHighPriority)
        {
            Trace.WriteLine("[Networking] SendingQueue.RegisterModule() function called.");
            bool isSuccessful = true;

            // Adding the priority of the module into the dictionary
            lock (_lock)
            {
                // If the module name is already taken
                if (_modulesToPriorityMap.ContainsKey(moduleName))
                    isSuccessful = false;
                else
                    _modulesToPriorityMap.Add(moduleName, isHighPriority);
            }

            Trace.WriteLine("[Networking] SendingQueue.RegisterModule() function returned.");
            return isSuccessful;
        }

        /// <summary>
        /// Inserts an element into the appropriate queue based on priority
        /// </summary>
        /// <param name="packet">
        /// The packet to be inserted into a queue
        /// </param>
        /// <returns> void </returns>
        public bool Enqueue(Packet packet)
        {
            Trace.WriteLine("[Networking] SendingQueue.Enqueue() function called.");

            string moduleName = packet.moduleOfPacket;
            bool isHighPriority, containsKey;

            // Finding out if the module is registered in the first place
            lock (_lock)
            {
                containsKey = _modulesToPriorityMap.ContainsKey(moduleName);
            }

            // If the module is not registered at all
            if (!containsKey)
            {
                Trace.WriteLine($"[Networking] Module {moduleName} is not registered.");

                // Returning that the enqueueing failed
                return false;
            }

            isHighPriority = _modulesToPriorityMap[moduleName];

            // Checking which queue to enqueue in
            if (isHighPriority)
                _highPriorityQueue.Enqueue(packet);
            else
                _lowPriorityQueue.Enqueue(packet);

            // Returning the enqueueing is done
            return true;
        }

        /// <summary>
        /// Removes and returns the front-most element in the appropriate queue based on priority
        /// </summary>
        /// <returns>
        /// The front-most element of the queue (based on priority)
        /// </returns>
        public Packet Dequeue()
        {
            Trace.WriteLine("[Networking] SendingQueue.Dequeue() function called.");

            Packet packet = null;

            // Dequeueing based on priority
            if (_priorityValue < _highPriorityValue)
            {
                if (!_highPriorityQueue.IsEmpty())
                {
                    packet = _highPriorityQueue.Dequeue();

                    // Updating the priority value which determines which queue is to be dequeued next
                    _priorityValue++;
                    _priorityValue %= _totalRatio;
                }
                else
                    packet = _lowPriorityQueue.Dequeue();
            }
            else
            {
                if (!_lowPriorityQueue.IsEmpty())
                {
                    packet = _lowPriorityQueue.Dequeue();

                    // Updating the priority value which determines which queue is to be dequeued next
                    _priorityValue++;
                    _priorityValue %= _totalRatio;
                }
                else
                    packet = _highPriorityQueue.Dequeue();
            }

            return packet;
        }

        /// <summary>
        /// Removes all elements in the queue
        /// </summary>
        /// <returns> void </returns>
        public void Clear()
        {
            // Clearing both queues
            _highPriorityQueue.Clear();
            _lowPriorityQueue.Clear();
        }

        /// <summary>
        /// Returns the sum of sizes of both queues
        /// </summary>
        /// <returns>
        /// Sum of sizes of both queues
        /// </returns>
        public int Size()
        {
            return _highPriorityQueue.Size() + _lowPriorityQueue.Size();
        }

        /// <summary>
        /// Informs if both queues are empty
        /// </summary>
        /// <returns>
        /// 'bool : true' if both queues are empty, else returns 'bool : false'
        /// </returns>
        public bool IsEmpty()
        {
            return _highPriorityQueue.IsEmpty() && _lowPriorityQueue.IsEmpty();
        }

        /// <summary>
        /// Returns if at least one queue is non-empty
        /// </summary>
        /// <returns>
        /// 'bool : true' if the either queue is not empty, else the function keeps waiting for atleast one packet to appear in the
        /// one of the queues and does not return until then
        /// </returns>
        public bool WaitForPacket()
        {
            bool isEmpty = true;

            while (true)
            {
                // Sleeping for some time
                Thread.Sleep(100);

                isEmpty = IsEmpty();

                if (!isEmpty)
                    break;
            }

            return isEmpty;
        }
    }
}
