/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains the class definition of the receiving queue
/// </summary>

using System.Diagnostics;

namespace PlexShareNetwork.Queues
{
    public class ReceivingQueue
    {
        public readonly IQueue _queue = new Queue();

        /// <summary>
        /// Inserts the given packet into the queue
        /// </summary>
        /// <param name="packet">
        /// Packet to be inserted into the queue
        /// </param>
        /// <returns> void </returns>
        public void Enqueue(Packet packet)
        {
            Trace.WriteLine("[Networking] ReceivingQueue.Enqueue() function called.");
            _queue.Enqueue(packet);
        }

        /// <summary>
        /// Removes and returns the front-most packet in the queue
        /// </summary>
        /// <returns>
        /// The front-most element of the queue
        /// </returns>
        public Packet Dequeue()
        {
            Trace.WriteLine("[Networking] ReceivingQueue.Dequeue() function called.");
            return _queue.Dequeue();
        }

        /// <summary>
        /// Returns the front-most packet in the queue
        /// </summary>
        /// <returns>
        /// The front-most element of the queue
        /// </returns>
        public Packet Peek()
        {
            return _queue.Peek();
        }

        /// <summary>
        /// Removes all elements in the queue
        /// </summary>
        /// <returns> void </returns>
        public void Clear()
        {
            _queue.Clear();
        }

        /// <summary>
        /// Returns the size of the queue
        /// </summary>
        /// <returns>
        /// Number of elements in the queue
        /// </returns>
        public int Size()
        {
            return _queue.Size();
        }

        /// <summary>
        /// Returns whether the queue is empty
        /// </summary>
        /// <returns>
        /// 'bool : true' if the queue is empty and 'bool : false' if not
        /// </returns>
        public bool IsEmpty()
        {
            return _queue.IsEmpty();
        }

        /// <summary>
        /// This function is needed to keep listening for packets on the receiving queue
        /// </summary>
        /// <returns>
        /// 'bool : true' if the queue is not empty, else the function keeps waiting for atleast one packet to appear in the queue
        /// and does not return until then
        /// </returns>
        public bool WaitForPacket()
        {
            Trace.WriteLine("[Networking] ReceivingQueue.WaitForPacket() function called.");
            return _queue.WaitForPacket();
        }
    }
}
