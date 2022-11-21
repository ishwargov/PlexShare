/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains the class definition of a queue
/// </summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace PlexShareNetwork.Queues
{
    public class Queue : IQueue
    {
        // Packets are enqueued to and dequeued from this queue
        private Queue<Packet> _queue;

        // Lock to ensure mutual exclusion
        private readonly object _lock;

        // Empty constructor
        public Queue()
        {
            this._queue = new Queue<Packet>();
            this._lock = new object();
        }

        /// <summary>
        /// Inserts an element into the queue
        /// </summary>
        /// <param name="packet">
        /// The packet to be inserted into the queue
        /// </param>
        /// <returns> void </returns>
        public void Enqueue(Packet packet)
        {
            lock (_lock)
            {
                _queue.Enqueue(packet);
            }
        }

        /// <summary>
        /// Removes and returns the front-most element in the queue
        /// </summary>
        /// <returns>
        /// The front-most element of the queue
        /// </returns>
        public Packet Dequeue()
        {
            Packet packet = null;

            lock (_lock)
            {
                try
                {
                    packet = _queue.Dequeue();
                }
                catch(InvalidOperationException e)
                {
                    // Arises if the queue is empty
                    Trace.WriteLine($"{e.StackTrace}");
                    packet = null;
                }
            }

            return packet;
        }

        /// <summary>
        /// Returns the front-most element in the queue without popping it
        /// </summary>
        /// <returns>
        /// The front-most element of the queue
        /// </returns>
        public Packet Peek()
        {
            Packet start = null;

            lock (_lock)
            {
                try
                {
                    start = _queue.Peek();
                }
                catch(InvalidOperationException e)
                {
                    // Arises if the queue is empty
                    Trace.WriteLine($"{e.StackTrace}");
                    start = null;
                }
            }

            return start;
        }

        /// <summary>
        /// Removes all elements in the queue
        /// </summary>
        /// <returns> void </returns>
        public void Clear()
        {
            lock (_lock)
            {
                this._queue.Clear();
            }
        }

        /// <summary>
        /// Returns the size of the queue
        /// </summary>
        /// <returns>
        /// Number of elements in the queue
        /// </returns>
        public int Size()
        {
            int size = 0;

            lock (_lock)
            {
                size = _queue.Count;
            }

            return size;
        }

        /// <summary>
        /// Returns the size of the queue
        /// </summary>
        /// <returns>
        /// 'bool : true' if the queue is empty and 'bool : false' if not
        /// </returns>
        public bool IsEmpty()
        {
            bool isEmpty = true;

            lock (_lock)
            {
                isEmpty = _queue.Count == 0;
            }

            return isEmpty;
        }

        /// <summary>
        /// The 'ReceivingQueueListener' needs this function to keep listening for packets on the receiving queue
        /// </summary>
        /// <returns>
        /// 'bool : true' if the queue is not empty, else the function keeps waiting for atleast one packet to appear in the queue
        /// and does not return until then
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
