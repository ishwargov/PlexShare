/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains all the tests written for the sending queues
/// </summary>

using PlexShareNetwork.Queues;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PlexShareTests.NetworkTests.Queues
{
    public class ReceivingQueueTests
    {
        ReceivingQueue _receivingQueue = new ReceivingQueue();

        /// <summary>
        /// Testing whether a packet is getting inserted into the receiving queue when enqueued
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void EnqueueOnePacketTest()
        {
            string serializedString = NetworkTestGlobals.RandomString(10);
            string destinationModule = NetworkTestGlobals.RandomString(5);
            string moduleName = NetworkTestGlobals.dashboardName;

            Packet packet = new Packet(serializedString, destinationModule, moduleName);

            // Enqueueing a single random packet
            _receivingQueue.Enqueue(packet);

            Assert.Equal(_receivingQueue.Size(), 1);
        }

        /// <summary>
        /// Testing whether multiple packets are getting inserted into the receiving queue when enqueued
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void EnqueueMultiplePacketsTest()
        {
            string moduleName = NetworkTestGlobals.dashboardName;
            string destinationModule = NetworkTestGlobals.RandomString(5);

            int number_of_packets = 100;

            // Enqueueing multiple packets
            for (int i = 0; i < number_of_packets; ++i)
                _receivingQueue.Enqueue(new Packet(NetworkTestGlobals.RandomString(10), destinationModule, moduleName));

            Assert.Equal(_receivingQueue.Size(), number_of_packets);
        }

        /// <summary>
        /// Testing whether packets are getting inserted into the receiving queue in the order in which they are enqueued
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void EnqueueMultiplePacketsOrderTest()
        {
            string moduleName = NetworkTestGlobals.dashboardName;
            string destinationModule = NetworkTestGlobals.RandomString(5);

            int number_of_packets = 100;

            // For comparison of each packet
            Stack stack = new Stack();

            // Enqueueing multiple packets
            for (int i = 0; i < number_of_packets; ++i)
            {
                Packet packet = new Packet(NetworkTestGlobals.RandomString(10), destinationModule, moduleName);
                _receivingQueue.Enqueue(packet);

                stack.Push(packet);
            }

            // Reversing the stack's elements for comparison
            Stack reverseStack = new Stack();
            while (stack.Count != 0)
                reverseStack.Push(stack.Pop());

            // Checking each element
            while (reverseStack.Count != 0 && !_receivingQueue.IsEmpty())
                Assert.Equal(_receivingQueue.Dequeue(), reverseStack.Pop());

            // The reverse stack and receiving queue need to be empty
            Assert.Equal(_receivingQueue.Size(), 0);
            Assert.Equal(_receivingQueue.Size(), reverseStack.Count);
        }

        /// <summary>
        /// Testing whether concurrent insertion of packets into the receiving queue works fine
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void ConcurrentEnqueuesTest()
        {
            int number_of_packets = 100;

            // Starting a thread
            var thread1 = Task.Run(() =>
            {
                for (int i = 0; i < number_of_packets; ++i)
                {
                    // Taking a random module name
                    string moduleName = NetworkTestGlobals.RandomString(10);

                    // Creating a packet of the above module
                    Packet packet = new Packet(NetworkTestGlobals.RandomString(10), NetworkTestGlobals.RandomString(5), moduleName);

                    _receivingQueue.Enqueue(packet);
                }
            });

            // Starting another thread
            var thread2 = Task.Run(() =>
            {
                for (int i = 0; i < number_of_packets; ++i)
                {
                    // Taking a random module name
                    string moduleName = NetworkTestGlobals.RandomString(10);

                    // Creating a packet of the above module
                    Packet packet = new Packet(NetworkTestGlobals.RandomString(10), NetworkTestGlobals.RandomString(5), moduleName);

                    _receivingQueue.Enqueue(packet);
                }
            });

            // Waiting for all tasks to be finished
            Task.WaitAll(thread1, thread2);

            Assert.Equal(_receivingQueue.Size(), 2 * number_of_packets);
        }

        /// <summary>
        /// Testing whether Clear() method of the receiving queue removes all entries in the queue
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void ClearReceivingQueueTest()
        {
            int number_of_packets = 100;

            for (int i = 0; i < number_of_packets; ++i)
            {
                // Taking a random module name
                string moduleName = NetworkTestGlobals.RandomString(10);

                // Creating a packet of the above module
                Packet packet = new Packet(NetworkTestGlobals.RandomString(10), NetworkTestGlobals.RandomString(5), moduleName);

                _receivingQueue.Enqueue(packet);
            }

            // Checking if all packets are enqueued
            Assert.Equal(_receivingQueue.Size(), number_of_packets);

            _receivingQueue.Clear();

            // Checking if all packets are removed
            Assert.Equal(_receivingQueue.Size(), 0);
        }

        /// <summary>
        /// Testing whether concurrent dequeue of a packet in the receiving queue works fine
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void ConcurrentDequeueTest()
        {
            string serializedData = NetworkTestGlobals.RandomString(10);
            string destinationModule = NetworkTestGlobals.RandomString(5);
            string moduleName = "Demo";

            // Creating a packet
            Packet packet = new Packet(serializedData, destinationModule, moduleName);

            _receivingQueue.Enqueue(packet);

            Packet dequeueOne = null, dequeueTwo = null;

            // Concurrently dequeueing
            Parallel.Invoke(
                () => { dequeueOne = _receivingQueue.Dequeue(); },
                () => { dequeueTwo = _receivingQueue.Dequeue(); }
                );

            // Exactly one of the dequeue must be fruitful
            Assert.True(dequeueOne == null || dequeueTwo == null);
            Assert.True(dequeueOne != null || dequeueTwo != null);

            // Comparing with the packet enqueued
            if (dequeueOne != null)
                Assert.Equal(packet, dequeueOne);
            else
                Assert.Equal(packet, dequeueTwo);
        }

        /// <summary>
        /// Testing whether concurrent peek of a packet in the receiving queue works fine
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void ConcurrentPeekTest()
        {
            string serializedData = NetworkTestGlobals.RandomString(10);
            string destinationModule = NetworkTestGlobals.RandomString(5);
            string moduleName = "Demo";

            // Creating a packet
            Packet packet = new Packet(serializedData, destinationModule, moduleName);

            _receivingQueue.Enqueue(packet);

            Packet peekOne = null, peekTwo = null;

            // Concurrently peeking
            Parallel.Invoke(
                () => { peekOne = _receivingQueue.Peek(); },
                () => { peekTwo = _receivingQueue.Peek(); }
                );

            // Output of both peeks must be same and not null
            Assert.True(peekOne != null);
            Assert.Equal(peekOne, peekTwo);

            // Comparing with the packet enqueued
            Assert.Equal(packet, peekOne);
        }

        /// <summary>
        /// Testing whether peek on an empty receiving queue works fine
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void PeekOnEmptyQueueTest()
        {
            Packet packet = _receivingQueue.Peek();
            Assert.Equal(packet, null);
        }

        /// <summary>
        /// Testing whether WaitForPacket() method detects a packet in the receiving queue and returns 'bool : true'
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void WaitForPacketTest()
        {
            string serializedData = NetworkTestGlobals.RandomString(10);
            string destinationModule = NetworkTestGlobals.RandomString(5);
            string moduleName = "Demo";

            // Creating a packet
            Packet packet = new Packet(serializedData, destinationModule, moduleName);

            // Thread to enqueue a packet
            Thread enqueueThread = new Thread(() =>
            {
                _receivingQueue.Enqueue(packet);
            });

            bool isEmpty = true;

            // Thread to wait for a packet
            Thread waitThread = new Thread(() =>
            {
                isEmpty = _receivingQueue.WaitForPacket();
            });

            // Beginning to wait
            waitThread.Start();

            // Sleeping for some time
            Thread.Sleep(3000);

            // Checking that the thread has not terminated yet
            Assert.True(waitThread.IsAlive);

            // Enqueueing a packet now
            enqueueThread.Start();

            // Sleeping until the thread is terminated
            while (waitThread.IsAlive)
                Thread.Sleep(1000);

            // The waiting thread must have been terminated by now
            Assert.False(isEmpty);
        }

        /// <summary>
        /// Testing whether WaitForPacket() method keeps waiting forever when no packet arrives at the receiving queue
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void WaitForeverForPacketTest()
        {
            // Thread to wait for a packet
            Thread waitThread = new Thread(() =>
            {
                _receivingQueue.WaitForPacket();
            });

            // Beginning to wait
            waitThread.Start();

            // Sleeping for some time
            Thread.Sleep(5000);

            // Checking that the thread has not terminated yet
            Assert.True(waitThread.IsAlive);
        }
    }
}
