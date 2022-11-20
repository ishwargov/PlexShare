/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains all the tests written for the receiving queue listener
/// </summary>

using PlexShareNetwork;
using PlexShareNetwork.Queues;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Xunit;

namespace PlexShareTests.NetworkTests.Queues
{
    public class ReceiveQueueListenerTests
    {
        // The receiving queue which the listener is listening on
        ReceivingQueue _receivingQueue = new ReceivingQueue();

        // Map from modules to their notification handlers
        Dictionary<string, INotificationHandler> _modulesToNotificationHandlerMap = new Dictionary<string, INotificationHandler>();

        // The pivot of this test
        ReceiveQueueListener _receiveQueueListener;

        /// <summary>
        /// Testing whether the accurate notification handler of a registered module is called when a packet of that module
        /// ends up in the receiving queue
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void CallAccurateHandlerTest()
        {
            // Clearing the handlers list for fresh testing
            DemoNotificationHandlerIdentifier.notificationHandlerList.Clear();

            _receiveQueueListener = new ReceiveQueueListener(_modulesToNotificationHandlerMap, _receivingQueue);

            string moduleName = NetworkTestGlobals.dashboardName;
            INotificationHandler notificationHandler = new DemoNotificationHandler();

            // Registering
            _receiveQueueListener.RegisterModule(moduleName, notificationHandler);

            string serializedData = NetworkTestGlobals.RandomString(10);
            string destination = NetworkTestGlobals.RandomString(5);

            // Starting it up
            _receiveQueueListener.Start();

            _receivingQueue.Enqueue(new Packet(serializedData, destination, moduleName));

            // Sleep for some time, for the listener to call the 'OnDataReceived' method
            while (DemoNotificationHandlerIdentifier.notificationHandlerList.Count < 1)
                Thread.Sleep(1000);

            // Only one handler must have been added to the list
            Assert.Equal(DemoNotificationHandlerIdentifier.notificationHandlerList.Count, 1);

            // Checking that the appropriate handler is called
            Assert.Equal(notificationHandler, DemoNotificationHandlerIdentifier.notificationHandlerList[0]);
        }

        /// <summary>
        /// Testing whether the accurate notification handlers of registered modules are called when packets of these
        /// modules end up in the receiving queue
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void CallMultipleAccurateHandlersTest()
        {
            // Clearing the handlers list for fresh testing
            DemoNotificationHandlerIdentifier.notificationHandlerList.Clear();

            _receiveQueueListener = new ReceiveQueueListener(_modulesToNotificationHandlerMap, _receivingQueue);

            // Starting it up
            _receiveQueueListener.Start();

            // To store the list of handlers called in order
            List<INotificationHandler> listOfHandlersCalled = new List<INotificationHandler>();

            for (int i = 0; i < 3; ++i)
            {
                string moduleName = NetworkTestGlobals.RandomString(5 + i);
                string serializedData = NetworkTestGlobals.RandomString(10);
                string destination = NetworkTestGlobals.RandomString(5);
                INotificationHandler notificationHandler = new DemoNotificationHandler();

                // Registering
                _receiveQueueListener.RegisterModule(moduleName, notificationHandler);

                // Enqueueing into the receiving queue
                _receivingQueue.Enqueue(new Packet(serializedData, destination, moduleName));

                listOfHandlersCalled.Add(notificationHandler);
            }

            // Sleep for some time, for the listener to call the 'OnDataReceived' method
            while (DemoNotificationHandlerIdentifier.notificationHandlerList.Count < listOfHandlersCalled.Count)
                Thread.Sleep(1000);

            // Asking the listener to stop
            _receiveQueueListener.Stop();

            // The number of handlers called and stored in the demo static class must be equal
            Assert.Equal(listOfHandlersCalled.Count, DemoNotificationHandlerIdentifier.notificationHandlerList.Count);

            int len = listOfHandlersCalled.Count;

            // Checking whether appropriate handlers were called
            for (int i = 0; i < len; ++i)
                Assert.Equal(listOfHandlersCalled[i], DemoNotificationHandlerIdentifier.notificationHandlerList[i]);
        }

        /// <summary>
        /// Testing whether duplicate registration of a module returns 'bool : false'
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void DuplicateRegistrationTest()
        {
            _receiveQueueListener = new ReceiveQueueListener(_modulesToNotificationHandlerMap, _receivingQueue);

            string moduleName = NetworkTestGlobals.dashboardName;
            INotificationHandler notificationHandler = new DemoNotificationHandler();

            // Registering
            _receiveQueueListener.RegisterModule(moduleName, notificationHandler);

            // Registering again
            bool isSuccessful = _receiveQueueListener.RegisterModule(moduleName, new DemoNotificationHandler());

            Assert.Equal(isSuccessful, false);
        }
    }

    // Demo class for testing
    public class DemoNotificationHandler : INotificationHandler
    {
        public void OnDataReceived(string serializedData)
        {
            DemoNotificationHandlerIdentifier.notificationHandlerList.Add(this);
        }

        public void OnClientJoined(TcpClient socket)
        { }

        public void OnClientLeft(string clientId)
        { }
    }

    // Stores the instance of notification handler of the module which was called
    public static class DemoNotificationHandlerIdentifier
    {
        public static List<INotificationHandler> notificationHandlerList = new List<INotificationHandler>();
    }
}
