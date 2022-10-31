/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains the definition of the IQueue interface. It contains method blueprints of all methods of a queue which the
/// 'Communicator' and the 'ReceivingQueueListener' need
/// </summary>

namespace Networking.Queues
{
    public interface IQueue
    {
        /// <summary>
        /// Inserts an element into the queue
        /// </summary>
        public void Enqueue(Packet packet);

        /// <summary>
        /// Removes and returns the front-most element in the queue
        /// </summary>
        public Packet Dequeue();

        /// <summary>
        /// Returns the front-most element in the queue without popping it
        /// </summary>
        public Packet Peek();

        /// <summary>
        /// Removes all elements in the queue
        /// </summary>
        public void Clear();

        /// <summary>
        /// Returns the size of the queue
        /// </summary>
        public int Size();

        /// <summary>
        /// Returns the size of the queue
        /// </summary>
        public bool IsEmpty();

        /// <summary>
        /// The 'ReceivingQueueListener' needs this function to keep listening for packets on the receiving queue
        /// </summary>
        public bool WaitForPacket();
    }
}
