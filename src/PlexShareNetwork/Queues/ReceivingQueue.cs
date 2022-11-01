/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains the class definition of the receiving queue
/// </summary>

namespace Networking.Queues
{
    public class ReceivingQueue
    {
        public readonly IQueue _queue = new Queue();

        /// <summary>
        /// Inserts the given packet into the queue
        /// </summary>
        public void Enqueue(Packet packet)
        {
            _queue.Enqueue(packet);
        }

        /// <summary>
        /// Removes and returns the front-most packet in the queue
        /// </summary>
        public Packet Dequeue()
        {
            return _queue.Dequeue();
        }

        /// <summary>
        /// Returns the front-most packet in the queue
        /// </summary>
        public Packet Peek()
        {
            return _queue.Peek();
        }

        /// <summary>
        /// Removes all elements in the queue
        /// </summary>
        public void Clear()
        {
            _queue.Clear();
        }

        /// <summary>
        /// Returns the size of the queue
        /// </summary>
        public int Size()
        {
            return _queue.Size();
        }

        /// <summary>
        /// Returns whether the queue is empty
        /// </summary>
        public bool IsEmpty()
        {
            return _queue.IsEmpty();
        }

        /// <summary>
        /// This function is needed to keep listening for packets on the receiving queue
        /// </summary>
        public bool WaitForPacket()
        {
            return _queue.WaitForPacket();
        }
    }
}
