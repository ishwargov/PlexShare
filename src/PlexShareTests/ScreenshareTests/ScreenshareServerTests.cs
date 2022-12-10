/// <author>Mayank Singla</author>
/// <summary>
/// Defines the "ScreenshareServerTests" class which contains tests for
/// methods defined in "ScreenshareServer" class.
/// </summary>

using Moq;
using PlexShareNetwork.Communication;
using PlexShareScreenshare;
using PlexShareScreenshare.Server;
using System.Text.Json;
using SSUtils = PlexShareScreenshare.Utils;

namespace PlexShareTests.ScreenshareTests
{
    /// <summary>
    /// Contains tests for methods defined in "ScreenshareServer" class.
    /// </summary>
    /// <remarks>
    /// It is marked sequential to run the tests sequentially so that each test
    /// can do their cleanup work in case they are using the singleton server class.
    /// </remarks>
    [Collection("Sequential")]
    public class ScreenshareServerTests
    {
        /// <summary>
        /// Update time values after the timeout time.
        /// </summary>
        public static IEnumerable<object[]> PostTimeoutTime =>
            new List<object[]>
            {
                new object[] { SharedClientScreen.Timeout + 100 },
                new object[] { SharedClientScreen.Timeout + 1000 },
                new object[] { SharedClientScreen.Timeout + 2000 },
            };

        /// <summary>
        /// Update time values before the timeout time.
        /// </summary>
        public static IEnumerable<object[]> PreTimeoutTime =>
            new List<object[]>
            {
                new object[] { SharedClientScreen.Timeout - 2000 },
                new object[] { SharedClientScreen.Timeout - 1000 },
            };

        /// <summary>
        /// Tests the successful registration of the clients when clients start screen sharing.
        /// </summary>
        [Fact]
        public void TestRegisterClient()
        {
            // Arrange.
            // Create mock server and mock clients.
            var viewmodelMock = new Mock<IMessageListener>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            int numClients = 5;
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);

            // Act.
            // Register the clients by sending mock register packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Sending the register request again should do nothing.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers =
                server.GetPrivate<Dictionary<string, SharedClientScreen>>("_subscribers").Values.ToList();

            // Check that each client registered is still present.
            Assert.True(subscribers.Count == numClients);
            foreach (SharedClientScreen client in clients)
            {
                Assert.True(subscribers.FindIndex(c => c.Id == client.Id) != -1);
            }

            // Check view model was notified at least once regarding new registration of clients.
            viewmodelMock.Verify(vm => vm.OnSubscribersChanged(It.IsAny<List<SharedClientScreen>>()),
                Times.AtLeastOnce(), $"Expected view model to be notified at least once");
            // Check view model was notified at least once regarding new registration of clients.
            viewmodelMock.Verify(vm => vm.OnScreenshareStart(It.IsAny<string>(), It.IsAny<string>()),
                Times.AtLeastOnce(), $"Expected view model to be notified for popup at least once");

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            server.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests the successful de-registration of the clients when clients stop screen sharing.
        /// </summary>
        [Fact]
        public void TestDeregisterClient()
        {
            // Arrange.
            // Create mock server and mock clients.
            var viewmodelMock = new Mock<IMessageListener>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            int numClients = 5;
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);

            // Act.
            // Register the clients by sending mock register packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Deregister the clients by sending mock deregister packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockDeregisterPacket = Utils.GetMockDeregisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockDeregisterPacket);
            }

            // Sending de-register request second time should do nothing.
            foreach (SharedClientScreen client in clients)
            {
                string mockDeregisterPacket = Utils.GetMockDeregisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockDeregisterPacket);
            }

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers =
                server.GetPrivate<Dictionary<string, SharedClientScreen>>("_subscribers").Values.ToList();

            // Check that all the clients are properly de-registered.
            Assert.True(subscribers.Count == 0);

            // Check view model was notified at least once regarding de-registration of clients.
            viewmodelMock.Verify(vm => vm.OnSubscribersChanged(It.IsAny<List<SharedClientScreen>>()),
                Times.AtLeastOnce(), $"Expected view model to be notified at least once");
            viewmodelMock.Verify(vm => vm.OnScreenshareStop(It.IsAny<string>(), It.IsAny<string>()),
                Times.AtLeastOnce(), $"Expected view model to be notified for popup at least once");

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            server.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests the successful image packet retrieval from the client.
        /// </summary>
        /// <param name="numImages">
        /// The number of images sent for the client to the server.
        /// </param>
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        public void TestPutImage(int numImages)
        {
            // Arrange.
            // Create mock server and mock client.
            var viewmodelMock = new Mock<IMessageListener>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            SharedClientScreen client = Utils.GetMockClient(server, isDebugging: true);

            // Act.
            // Create mock images and mock image packets for the client.
            List<(string MockImagePacket, string MockImage)> clientImagePackets = new();
            for (int i = 0; i < numImages; ++i)
            {
                clientImagePackets.Add(Utils.GetMockImagePacket(client.Id, client.Name));
            }

            // Register the client on the server first.
            string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
            server.OnDataReceived(mockRegisterPacket);

            // Send mock image packets for the client to the server.
            for (int i = 0; i < numImages; ++i)
            {
                server.OnDataReceived(clientImagePackets[i].MockImagePacket);
            }

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers =
                server.GetPrivate<Dictionary<string, SharedClientScreen>>("_subscribers").Values.ToList();

            // Check that all the sent images for the client are successfully enqueued in the client's image queue.
            Assert.True(subscribers.Count == 1);
            int clientIdx = subscribers.FindIndex(c => c.Id == client.Id);
            Assert.True(clientIdx != -1);

            SharedClientScreen serverClient = subscribers[clientIdx];
            Assert.True(serverClient.GetPrivate<Queue<string>>("_imageQueue").Count == numImages);
            for (int i = 0; i < numImages; ++i)
            {
                string? receivedImage = serverClient.GetImage(serverClient.TaskId);
                Assert.True(receivedImage != null);
                Assert.True(receivedImage == clientImagePackets[i].MockImage);
            }

            // Cleanup.
            client.Dispose();
            serverClient.Dispose();
            server.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests the false image packet retrieval from unknown client.
        /// </summary>
        /// <param name="numImages">
        /// The number of images sent for the client to the server.
        /// </param>
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        public void TestPutFaultImage(int numImages)
        {
            // Arrange.
            // Create mock server and mock client.
            var viewmodelMock = new Mock<IMessageListener>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            SharedClientScreen client = Utils.GetMockClient(server, isDebugging: true);

            // Act.
            // Create mock images and mock image packets for the client.
            List<(string MockImagePacket, string MockImage)> clientImagePackets = new();
            for (int i = 0; i < numImages; ++i)
            {
                clientImagePackets.Add(Utils.GetMockImagePacket(client.Id + "1", client.Name));
            }

            // Register the client on the server first.
            string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
            server.OnDataReceived(mockRegisterPacket);

            // Send mock image packets for the client to the server.
            for (int i = 0; i < numImages; ++i)
            {
                server.OnDataReceived(clientImagePackets[i].MockImagePacket);
            }

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers =
                server.GetPrivate<Dictionary<string, SharedClientScreen>>("_subscribers").Values.ToList();

            // Check that no image is being put the client image queue.
            Assert.True(subscribers.Count == 1);
            int clientIdx = subscribers.FindIndex(c => c.Id == client.Id);
            Assert.True(clientIdx != -1);

            SharedClientScreen serverClient = subscribers[clientIdx];
            Assert.True(serverClient.GetPrivate<Queue<string>>("_imageQueue").Count == 0);

            // Cleanup.
            client.Dispose();
            serverClient.Dispose();
            server.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests the successful arrival of the client's CONFIRMATION packet before their timeout.
        /// </summary>
        /// <param name="timeOfArrival">
        /// The time of arrival of the client's CONFIRMATION packet.
        /// </param>
        [Theory]
        [MemberData(nameof(PreTimeoutTime))]
        public void TestSuccessfulConfirmationPacketArrival(int timeOfArrival)
        {
            // Arrange.
            // Create mock server and mock clients.
            var viewmodelMock = new Mock<IMessageListener>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            int numClients = 5;
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);

            // Act.
            // Register all the clients to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Sleep until the CONFIRMATION packet arrives.
            Thread.Sleep(timeOfArrival);

            // Send client's CONFIRMATION packet to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockConfirmationPacket = Utils.GetMockConfirmationPacket(client.Id, client.Name);
                server.OnDataReceived(mockConfirmationPacket);
            }

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers =
                server.GetPrivate<Dictionary<string, SharedClientScreen>>("_subscribers").Values.ToList();

            // Check that no client is de-registered as their CONFIRMATION packet arrives before time.
            Assert.True(subscribers.Count == numClients);
            foreach (SharedClientScreen client in clients)
            {
                Assert.True(subscribers.FindIndex(c => c.Id == client.Id) != -1);
            }

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            server.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests the unsuccessful arrival of the client's CONFIRMATION packet before their timeout.
        /// </summary>
        /// <param name="timeOfArrival">
        /// The time of arrival of the client's CONFIRMATION packet.
        /// </param>
        [Theory]
        [MemberData(nameof(PostTimeoutTime))]
        public void TestUnuccessfulConfirmationPacketArrival(int timeOfArrival)
        {
            // Arrange.
            // Create mock server and mock clients.
            var viewmodelMock = new Mock<IMessageListener>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            int numClients = 5;
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);

            // Act.
            // Register the clients to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Sleep until the CONFIRMATION packet arrives.
            Thread.Sleep(timeOfArrival);

            // Send client's CONFIRMATION packet to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockConfirmationPacket = Utils.GetMockConfirmationPacket(client.Id, client.Name);
                server.OnDataReceived(mockConfirmationPacket);
            }

            // Assert.
            // Get the private list of the subscribers stored in the server.
            List<SharedClientScreen> subscribers =
                server.GetPrivate<Dictionary<string, SharedClientScreen>>("_subscribers").Values.ToList();

            // Check that all the clients are de-registered as their CONFIRMATION packet arrives
            // after the timeout time.
            Assert.True(subscribers.Count == 0);

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            server.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests the successful de-registration of a client when the client leaves.
        /// </summary>
        [Fact]
        public void TestOnClientLeft()
        {
            // Arrange.
            // Create mock server and mock clients.
            var viewmodelMock = new Mock<IMessageListener>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            int numClients = 5;
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);

            // Act.
            // Register clients to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Tell the server that all the clients left.
            foreach (SharedClientScreen client in clients)
            {
                server.OnClientLeft(client.Id);
            }

            // Assert.
            // Get the private list of the subscribers stored in the server.
            List<SharedClientScreen> subscribers =
                server.GetPrivate<Dictionary<string, SharedClientScreen>>("_subscribers").Values.ToList();

            // Check that all the clients are successfully de-registered on the server.
            Assert.True(subscribers.Count == 0);

            // Check view model was notified at least once regarding de-registration of clients.
            viewmodelMock.Verify(vm => vm.OnSubscribersChanged(It.IsAny<List<SharedClientScreen>>()),
                Times.AtLeastOnce(), $"Expected view model to be notified at least once");
            viewmodelMock.Verify(vm => vm.OnScreenshareStop(It.IsAny<string>(), It.IsAny<string>()),
                Times.AtLeastOnce(), $"Expected view model to be notified for popup at least once");

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            server.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests the successful sending of packets to the clients.
        /// </summary>
        [Fact]
        public void TestBroadcastClients()
        {
            // Arrange.
            // Create mock server and networking communicator.
            var viewmodelMock = new Mock<IMessageListener>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            var communicatorMock = new Mock<ICommunicator>();
            server.SetPrivate("_communicator", communicatorMock.Object);

            // Create mock clients.
            int numClients = 5;
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);
            List<string> clientIds = clients.Select(client => client.Id).ToList();

            // Act.
            (int Rows, int Cols) numRowsColumns = (1, 2);
            // Tell the server to send packets when an empty list of clients is given.
            server.BroadcastClients(new(), nameof(ServerDataHeader.Send), numRowsColumns);
            // Tell the server to send the clients the packet with given header and data.
            server.BroadcastClients(clientIds, nameof(ServerDataHeader.Stop), numRowsColumns);

            // Assert.
            // Verify that the "Send" method for the mock communicator was called as many times the number
            // clients which were given to the server to broadcast to.
            communicatorMock.Verify(communicator =>
                communicator.Send(It.IsAny<string>(), SSUtils.ModuleIdentifier, It.IsIn<string>(clientIds)),
                Times.Exactly(numClients));

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            server.Dispose();
        }

        /// <summary>
        /// Tests sending an ill created packet to the server.
        /// </summary>
        [Fact]
        public void TestIllPacket()
        {
            // Arrange.
            // Create mock server and mock clients.
            var viewmodelMock = new Mock<IMessageListener>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            int numClients = 5;
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);

            // Act.
            // Register the clients by sending mock ill packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string illPacket = JsonSerializer.Serialize<DataPacket>(new());
                server.OnDataReceived(illPacket);
                server.OnDataReceived(Utils.RandomString(100));
            }

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers =
                server.GetPrivate<Dictionary<string, SharedClientScreen>>("_subscribers").Values.ToList();

            // Check that no client is registered.
            Assert.True(subscribers.Count == 0);

            // Check view model was notified never regarding new registration of clients.
            viewmodelMock.Verify(vm => vm.OnSubscribersChanged(It.IsAny<List<SharedClientScreen>>()),
                Times.Never(), $"Expected view model to be notified never");
            // Check view model was notified never regarding new registration of clients.
            viewmodelMock.Verify(vm => vm.OnScreenshareStart(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never(), $"Expected view model to be notified for popup never");

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            server.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests that the server is successfully disposed and de-registers all the registered clients.
        /// </summary>
        /// <param name="sleepTime">
        /// Sleep time of the thread after calling Dispose.
        /// </param>
        [Theory]
        [InlineData(5000)]
        [InlineData(4000)]
        public void TestDispose(int sleepTime)
        {
            // Arrange.
            // Create mock server and mock clients.
            var viewmodelMock = new Mock<IMessageListener>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            int numClients = 5;
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);

            // Act.
            // Register the clients to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Dispose the server and de-register all the registered clients.
            server.Dispose();
            Thread.Sleep(sleepTime);
            // Try disposing again.
            server.Dispose();

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers =
                server.GetPrivate<Dictionary<string, SharedClientScreen>>("_subscribers").Values.ToList();

            // Check that all the registered clients are successfully de-registered.
            Assert.True(subscribers.Count == 0);

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            subscribers.Clear();
        }
    }
}
