
namespace Networking.Queues
{
    public interface IQueue
    {
        public int Size(bool isHighPriority);
        public bool IsEmpty(bool isHighPriority);
        public void Clear();
        public void Enqueue(Packet packet, bool isHighPriority);
        public Packet Dequeue(bool isHighPriority);
        public Packet Peek(bool isHighPriority);
    }
}
