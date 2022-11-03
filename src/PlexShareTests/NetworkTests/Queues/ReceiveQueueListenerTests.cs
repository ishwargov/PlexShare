/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains all the tests written for the receiving queue listener
/// </summary>

using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace PlexShareNetwork.Queues.Tests
{
    public class ReceiveQueueListenerTests
    {
        // The receiving queue which the listener is listening on
        ReceivingQueue _receivingQueue = new ReceivingQueue();

        // Map from modules to their notification handlers
        Dictionary<string, INotificationHandler> _modulesToNotificationHandlerMap = new Dictionary<string, INotificationHandler>();

        // The pivot of this test
        ReceiveQueueListener _receiveQueueListener;

        [Fact]
        public void CallAccurateHandlerCheck()
        {
            // Clearing the handlers list for fresh testing
            DemoNotificationHandlerIdentifier.notificationHandlerList.Clear();

            _receiveQueueListener = new ReceiveQueueListener(_modulesToNotificationHandlerMap, _receivingQueue);

            string moduleName = NetworkingGlobals.dashboardName;
            INotificationHandler notificationHandler = new DemoNotificationHandler();

            // Registering
            _receiveQueueListener.RegisterModule(moduleName, notificationHandler);

            string serializedData = NetworkingGlobals.RandomString(10);
            string destination = NetworkingGlobals.RandomString(5);

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

        [Fact]
        public void CallMultipleAccurateHandlersCheck()
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
                string moduleName = NetworkingGlobals.RandomString(5 + i);
                string serializedData = NetworkingGlobals.RandomString(10);
                string destination = NetworkingGlobals.RandomString(5);
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

            // The number of handlers called and stored in the demo static class must be equal
            Assert.Equal(listOfHandlersCalled.Count, DemoNotificationHandlerIdentifier.notificationHandlerList.Count);

            int len = listOfHandlersCalled.Count;

            // Checking whether appropriate handlers were called
            for (int i = 0; i < len; ++i)
                Assert.Equal(listOfHandlersCalled[i], DemoNotificationHandlerIdentifier.notificationHandlerList[i]);
        }
    }

    // Demo class for testing
    public class DemoNotificationHandler : INotificationHandler
    {
        public void OnDataReceived(string serializedData)
        {
            DemoNotificationHandlerIdentifier.notificationHandlerList.Add(this);
        }
    }

    // Stores the instance of notification handler of the module which was called
    public static class DemoNotificationHandlerIdentifier
    {
        public static List<INotificationHandler> notificationHandlerList = new List<INotificationHandler>();
    }
}
