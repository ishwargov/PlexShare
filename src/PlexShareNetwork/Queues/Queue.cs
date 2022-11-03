/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains the class definition of a queue
/// </summary>

using System;
using System.Collections.Generic;
using System.Threading;

namespace PlexShareNetworking.Queues
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
                    Console.WriteLine(e.StackTrace);
                    packet = null;
                }
            }

            return packet;
        }

        /// <summary>
        /// Returns the front-most element in the queue without popping it
        /// </summary>
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
                    Console.WriteLine(e.StackTrace);
                    start = null;
                }
            }

            return start;
        }

        /// <summary>
        /// Removes all elements in the queue
        /// </summary>
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
