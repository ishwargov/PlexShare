/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains all the tests written for the sending queues
/// </summary>

using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PlexShareNetworking.Queues.Tests
{
    public class SendingQueuesTests
    {
        private SendingQueues _sendingQueues = new SendingQueues();

        [Fact]
        public void EnqueueOnePacketTest()
        {
            // Registering the dashboard module
            _sendingQueues.RegisterModule(NetworkingGlobals.dashboardName, true);

            // Creating a packet
            Packet packet = new Packet(NetworkingGlobals.RandomString(10), NetworkingGlobals.dashboardName,
                NetworkingGlobals.dashboardName);

            // Enqueueing the packet
            _sendingQueues.Enqueue(packet);

            Assert.False(_sendingQueues.IsEmpty());
            Assert.Equal(_sendingQueues.Size(), 1);
        }

        [Fact]
        public void DequeueOnEmptyQueueTest()
        {
            // Dequeueing on an empty queue
            Packet packet = _sendingQueues.Dequeue();

            Assert.True(_sendingQueues.IsEmpty());
            Assert.Equal(packet, null);
        }

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
                    string moduleName = NetworkingGlobals.RandomString(10);

                    // Registering the module with a random priority
                    _sendingQueues.RegisterModule(moduleName, i % 2 == 0);

                    // Creating a packet of the above module
                    Packet packet = new Packet(NetworkingGlobals.RandomString(10), NetworkingGlobals.RandomString(5), moduleName);

                    _sendingQueues.Enqueue(packet);
                }
            });

            // Starting another thread
            var thread2 = Task.Run(() =>
            {
                for (int i = 0; i < number_of_packets; ++i)
                {
                    // Taking a random module name
                    string moduleName = NetworkingGlobals.RandomString(10);

                    // Registering the module with a random priority
                    _sendingQueues.RegisterModule(moduleName, i % 2 == 0);

                    // Creating a packet of the above module
                    Packet packet = new Packet(NetworkingGlobals.RandomString(10), NetworkingGlobals.RandomString(5), moduleName);

                    _sendingQueues.Enqueue(packet);
                }
            });

            // Waiting for all tasks to be finished
            Task.WaitAll(thread1, thread2);

            Assert.False(_sendingQueues.IsEmpty());
            Assert.Equal(_sendingQueues.Size(), 2 * number_of_packets);
        }

        [Fact]
        public void ClearSendingQueuesTest()
        {
            int number_of_packets = 100;

            for (int i = 0; i < number_of_packets; ++i)
            {
                // Taking a random module name
                string moduleName = NetworkingGlobals.RandomString(10);

                // Registering the module with a random priority
                _sendingQueues.RegisterModule(moduleName, i % 2 == 0);

                // Creating a packet of the above module
                Packet packet = new Packet(NetworkingGlobals.RandomString(10), NetworkingGlobals.RandomString(5), moduleName);

                _sendingQueues.Enqueue(packet);
            }

            // Checking if all packets are enqueued
            Assert.False(_sendingQueues.IsEmpty());
            Assert.Equal(_sendingQueues.Size(), number_of_packets);

            _sendingQueues.Clear();

            // Checking if all packets are removed
            Assert.True(_sendingQueues.IsEmpty());
            Assert.Equal(_sendingQueues.Size(), 0);
        }

        [Fact]
        public void DuplicateModuleNameTest()
        {
            string moduleName = "Demo module";

            bool isSuccessful = _sendingQueues.RegisterModule(moduleName, true);

            Assert.Equal(isSuccessful, true);

            // Duplicate registration
            isSuccessful = _sendingQueues.RegisterModule(moduleName, false);

            Assert.Equal(isSuccessful, false);
        }

        private void checkEnqueueOrderTest(bool isHighPriority)
        {
            string moduleName = "Demo high";
            _sendingQueues.RegisterModule(moduleName, isHighPriority);

            // To store every packet in the order of enqueueing
            Stack stack = new Stack();

            // Enqueueing packets of high priority
            int number_of_packets = 100;
            for (int i = 0; i < number_of_packets; ++i)
            {
                string serializedData = NetworkingGlobals.RandomString(10);
                string destinationModule = NetworkingGlobals.RandomString(5);

                Packet packet = new Packet(serializedData, destinationModule, moduleName);

                // Pushing into a stack
                stack.Push(packet);

                _sendingQueues.Enqueue(packet);
            }

            // Reversing the stack
            Stack reverseStack = new Stack();
            while (stack.Count != 0)
                reverseStack.Push(stack.Pop());

            // Dequeueing each packet
            while (!_sendingQueues.IsEmpty() && (reverseStack.Count != 0))
            {
                Packet packet = _sendingQueues.Dequeue();

                // Checking if each packet is in order of enqueueing
                Assert.Equal(packet, reverseStack.Pop());
            }

            // Both queue and stack need to be empty here
            Assert.Equal(_sendingQueues.Size(), 0);
            Assert.Equal(_sendingQueues.Size(), reverseStack.Count);
        }

        [Fact]
        public void checkEnqueueOrderForAllQueuesTest()
        {
            // Checking the order of enqueueing in both high and low priority case
            checkEnqueueOrderTest(true);
            checkEnqueueOrderTest(false);
        }

        [Fact]
        public void concurrentDequeueTest()
        {
            string serializedData = NetworkingGlobals.RandomString(10);
            string destinationModule = NetworkingGlobals.RandomString(5);
            string moduleName = "Demo";

            _sendingQueues.RegisterModule(moduleName, true);

            // Creating a packet
            Packet packet = new Packet(serializedData, destinationModule, moduleName);

            _sendingQueues.Enqueue(packet);

            Packet dequeueOne = null, dequeueTwo = null;

            // Concurrently dequeueing
            Parallel.Invoke(
                () => { dequeueOne = _sendingQueues.Dequeue(); },
                () => { dequeueTwo = _sendingQueues.Dequeue(); }
                );

            // Exactly one of the dequeues must be fruitful
            Assert.True(dequeueOne == null || dequeueTwo == null);
            Assert.True(dequeueOne != null || dequeueTwo != null);

            // Comparing with the packet enqueued
            if (dequeueOne != null)
                Assert.Equal(packet, dequeueOne);
            else
                Assert.Equal(packet, dequeueTwo);
        }

        [Fact]
        public void WaitForPacketTest()
        {
            string serializedData = NetworkingGlobals.RandomString(10);
            string destinationModule = NetworkingGlobals.RandomString(5);
            string moduleName = "Demo";

            _sendingQueues.RegisterModule(moduleName, false);

            // Creating a packet
            Packet packet = new Packet(serializedData, destinationModule, moduleName);

            // Thread to enqueue a packet
            Thread enqueueThread = new Thread(() =>
            {
                _sendingQueues.Enqueue(packet);
            });

            bool isEmpty = true;

            // Thread to wait for a packet
            Thread waitThread = new Thread(() =>
            {
                isEmpty = _sendingQueues.WaitForPacket();
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

        [Fact]
        public void WaitForeverForPacketTest()
        {
            // Thread to wait for a packet
            Thread waitThread = new Thread(() =>
            {
                _sendingQueues.WaitForPacket();
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
