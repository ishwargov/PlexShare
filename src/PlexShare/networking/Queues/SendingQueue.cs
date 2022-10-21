using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.Queues
{
    public class SendingQueue : IQueue
    {
        private Queue<Packet> _highPriorityQueue;
        private Queue<Packet> _lowPriorityQueue;

        private readonly object _lock;

        public SendingQueue()
        {
            this._highPriorityQueue = new Queue<Packet>();
            this._lowPriorityQueue = new Queue<Packet>();

            this._lock = new object();
        }

        public void Clear()
        {
            lock (_lock)
            {
                // Clearing both queues
                this._highPriorityQueue.Clear();
                this._lowPriorityQueue.Clear();
            }
        }

        public Packet Dequeue(bool isHighPriority)
        {
            Packet packet = null;

            lock (_lock)
            {
                // Dequeueing from the queue depending on the priority
                packet = isHighPriority ? _highPriorityQueue.Dequeue() : _lowPriorityQueue.Dequeue();
            }

            return packet;
        }

        public void Enqueue(Packet packet, bool isHighPriority)
        {
            lock (_lock)
            {
                // Enqueueing in the queue based on priority
                if (isHighPriority)
                    _highPriorityQueue.Enqueue(packet);
                else
                    _lowPriorityQueue.Enqueue(packet);
            }
        }

        public bool IsEmpty(bool isHighPriority)
        {
            bool isEmpty = true;

            lock (_lock)
            {
                // Checking the size of the queue depending on the priority
                isEmpty = isHighPriority ? _highPriorityQueue.Count == 0 : _highPriorityQueue.Count == 0;
            }

            return isEmpty;
        }

        public Packet Peek(bool isHighPriority)
        {
            Packet start = null;

            lock(_lock)
            {
                // Peeking in the queue depending on the priority
                start = isHighPriority ? _highPriorityQueue.Peek() : _lowPriorityQueue.Peek();
            }

            return start;
        }

        public int Size(bool isHighPriority)
        {
            int size = 0;

            lock (_lock)
            {
                // Returning the size depending on the priority
                size = isHighPriority ? _highPriorityQueue.Count : _lowPriorityQueue.Count;
            }

            return size;
        }
    }
}
