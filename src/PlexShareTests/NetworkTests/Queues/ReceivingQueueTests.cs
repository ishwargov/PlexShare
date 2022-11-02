/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains all the tests written for the sending queues
/// </summary>

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace Networking.Queues.Test
{
    [TestClass()]
    public class ReceivingQueueTests
    {
        ReceivingQueue _receivingQueue = new ReceivingQueue();

        [TestMethod()]
        public void EnqueueOnePacketTest()
        {
            string serializedString = NetworkingGlobals.RandomString(10);
            string destinationModule = NetworkingGlobals.RandomString(5);
            string moduleName = NetworkingGlobals.dashboardName;

            Packet packet = new Packet(serializedString, destinationModule, moduleName);

            // Enqueueing a single random packet
            _receivingQueue.Enqueue(packet);

            Assert.AreEqual(_receivingQueue.Size(), 1);
        }

        [TestMethod()]
        public void EnqueueMultiplePacketsTest()
        {
            string moduleName = NetworkingGlobals.dashboardName;
            string destinationModule = NetworkingGlobals.RandomString(5);

            int number_of_packets = 100;

            // Enqueueing multiple packets
            for (int i = 0; i < number_of_packets; ++i)
                _receivingQueue.Enqueue(new Packet(NetworkingGlobals.RandomString(10), destinationModule, moduleName));

            Assert.AreEqual(_receivingQueue.Size(), number_of_packets);
        }

        [TestMethod()]
        public void EnqueueOrderMultiplePacketsOrderTest()
        {
            string moduleName = NetworkingGlobals.dashboardName;
            string destinationModule = NetworkingGlobals.RandomString(5);

            int number_of_packets = 100;

            // For comparison of each packet
            Stack stack = new Stack();

            // Enqueueing multiple packets
            for (int i = 0; i < number_of_packets; ++i)
            {
                Packet packet = new Packet(NetworkingGlobals.RandomString(10), destinationModule, moduleName);
                _receivingQueue.Enqueue(packet);

                stack.Push(packet);
            }

            // Reversing the stack's elements for comparison
            Stack reverseStack = new Stack();
            while (stack.Count != 0)
                reverseStack.Push(stack.Pop());

            // Checking each element
            while (reverseStack.Count != 0 && !_receivingQueue.IsEmpty())
                Assert.AreEqual(_receivingQueue.Dequeue(), reverseStack.Pop());

            // The reverse stack and receiving queue need to be empty
            Assert.AreEqual(_receivingQueue.Size(), 0);
            Assert.AreEqual(_receivingQueue.Size(), reverseStack.Count);
        }

        [TestMethod()]
        public void ConcurrentEnqueuesTest()
        {
            int number_of_packets = 100;

            // Starting a thread
            var thread1 = Task.Run(() =>
            {
                for (int i = 0; i < number_of_packets; ++i)
                {
                    // Taking a random module name
                    string moduleName = NetworkingGlobals.RandomString(10);

                    // Creating a packet of the above module
                    Packet packet = new Packet(NetworkingGlobals.RandomString(10), NetworkingGlobals.RandomString(5), moduleName);

                    _receivingQueue.Enqueue(packet);
                }
            });

            // Starting another thread
            var thread2 = Task.Run(() =>
            {
                for (int i = 0; i < number_of_packets; ++i)
                {
                    // Taking a random module name
                    string moduleName = NetworkingGlobals.RandomString(10);

                    // Creating a packet of the above module
                    Packet packet = new Packet(NetworkingGlobals.RandomString(10), NetworkingGlobals.RandomString(5), moduleName);

                    _receivingQueue.Enqueue(packet);
                }
            });

            // Waiting for all tasks to be finished
            Task.WaitAll(thread1, thread2);

            Assert.AreEqual(_receivingQueue.Size(), 2 * number_of_packets);
        }

        [TestMethod()]
        public void ClearReceivingQueueTest()
        {
            int number_of_packets = 100;

            for (int i = 0; i < number_of_packets; ++i)
            {
                // Taking a random module name
                string moduleName = NetworkingGlobals.RandomString(10);

                // Creating a packet of the above module
                Packet packet = new Packet(NetworkingGlobals.RandomString(10), NetworkingGlobals.RandomString(5), moduleName);

                _receivingQueue.Enqueue(packet);
            }

            // Checking if all packets are enqueued
            Assert.AreEqual(_receivingQueue.Size(), number_of_packets);

            _receivingQueue.Clear();

            // Checking if all packets are removed
            Assert.AreEqual(_receivingQueue.Size(), 0);
        }

        [TestMethod()]
        public void concurrentDequeueTest()
        {
            string serializedData = NetworkingGlobals.RandomString(10);
            string destinationModule = NetworkingGlobals.RandomString(5);
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
            Assert.IsTrue(dequeueOne == null || dequeueTwo == null);
            Assert.IsTrue(dequeueOne != null || dequeueTwo != null);

            // Comparing with the packet enqueued
            if (dequeueOne != null)
                Assert.AreEqual(packet, dequeueOne);
            else
                Assert.AreEqual(packet, dequeueTwo);
        }

        [TestMethod()]
        public void concurrentPeekTest()
        {
            string serializedData = NetworkingGlobals.RandomString(10);
            string destinationModule = NetworkingGlobals.RandomString(5);
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
            Assert.IsTrue(peekOne != null);
            Assert.AreEqual(peekOne, peekTwo);

            // Comparing with the packet enqueued
            Assert.AreEqual(packet, peekOne);
        }

        [TestMethod()]
        public void WaitForPacketTest()
        {
            string serializedData = NetworkingGlobals.RandomString(10);
            string destinationModule = NetworkingGlobals.RandomString(5);
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
            Thread.Sleep(1000);

            // Checking that the thread has not terminated yet
            Assert.IsTrue(waitThread.IsAlive);

            // Enqueueing a packet now
            enqueueThread.Start();

            // Sleeping for some more time
            Thread.Sleep(3000);

            // The waiting thread must have been terminated by now
            Assert.IsFalse(waitThread.IsAlive);
            Assert.IsFalse(isEmpty);
        }

        [TestMethod()]
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
            Thread.Sleep(3000);

            // Checking that the thread has not terminated yet
            Assert.IsTrue(waitThread.IsAlive);
        }
    }
}
