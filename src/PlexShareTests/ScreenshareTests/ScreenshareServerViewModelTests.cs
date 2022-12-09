/// <author>Mayank Singla</author>
/// <summary>
/// Defines the "ScreenshareServerViewModelTests" class which contains tests for
/// methods defined in "ScreenshareServerViewModel" class.
/// </summary>

using Moq;
using PlexShareNetwork.Communication;
using PlexShareScreenshare.Server;
using System.ComponentModel;
using System.Windows.Threading;

using SSUtils = PlexShareScreenshare.Utils;
using Timer = System.Timers.Timer;

namespace PlexShareTests.ScreenshareTests
{
    /// <summary>
    /// Contains tests for methods defined in "ScreenshareServerViewModel" class.
    /// </summary>
    [Collection("Sequential")]
    public class ScreenshareServerViewModelTests
    {
        /// <summary>
        /// Different number of client values used for testing.
        /// </summary>
        public static IEnumerable<object[]> NumberOfClients =>
            new List<object[]>
            {
                new object[] { 1 },
                new object[] { 5 },
                new object[] { 15 },
                new object[] { 25 }
            };

        /// <summary>
        /// Different values used for testing page change when no client is pinned.
        /// This data is based on that ScreenshareServerViewModel.MaxTiles = 9.
        /// </summary>
        /// <remarks>
        /// Different fields are:
        /// Number of clients, Switched page, Total pages, Number of clients on the switched page.
        /// </remarks>
        public static IEnumerable<object[]> PageChangeNoPinnedClients =>
            new List<object[]>
            {
                new object[] { 1, 1, 1, 1 },
                new object[] { 5, 1, 1, 5 },
                new object[] { 15, 2, 2, 6 },
                new object[] { 25, 2, 3, 9 },
                new object[] { 25, 3, 3, 7 }
            };

        /// <summary>
        /// Different values used for testing page change when one client is pinned.
        /// This data is based on that ScreenshareServerViewModel.MaxTiles = 9.
        /// </summary>
        /// <remarks>
        /// Different fields are:
        /// Number of clients, Switched page, Total pages, Number of clients on the switched page.
        /// </remarks>
        public static IEnumerable<object[]> PageChangePinnedClients =>
            new List<object[]>
            {
                new object[] { 1, 1, 1, 1 },
                new object[] { 5, 2, 2, 4 },
                new object[] { 15, 2, 3, 9 },
                new object[] { 15, 3, 3, 5 },
                new object[] { 25, 2, 4, 9 },
                new object[] { 25, 3, 4, 9 },
                new object[] { 25, 4, 4, 6 }
            };

        /// <summary>
        /// Tests the successful registration of the clients when clients start screen sharing.
        /// </summary>
        /// <param name="numClients">
        /// Number of clients who registered.
        /// </param>
        [Theory]
        [MemberData(nameof(NumberOfClients))]
        public void TestRegisterClient(int numClients)
        {
            // Arrange.
            // Create view model and mock communicator.
            ScreenshareServerViewModel viewModel = ScreenshareServerViewModel.GetInstance(isDebugging: true);
            ScreenshareServer server = viewModel.GetPrivate<ScreenshareServer>("_model");
            var communicatorMock = new Mock<ICommunicator>();
            server.SetPrivate("_communicator", communicatorMock.Object);

            // Create mock clients.
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);
            List<string> clientIds = clients.Select(client => client.Id).ToList();

            // Add custom handler to the PropertyChanged event.
            int invokedCount = 0;
            PropertyChangedEventHandler handler = new((_, _) => ++invokedCount);
            viewModel.PropertyChanged += handler;

            // Act.
            // Register the clients by sending mock register packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Disable timers for the clients for testing.
            DisableTimer(server.GetPrivate<Dictionary<string, SharedClientScreen>>("_subscribers").Values.ToList());

            // Get the main thread dispatcher operations from "BeginInvoke".
            DispatcherOperation? updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            DispatcherOperation? displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            // Assert that they are being set.
            Assert.True(updateViewOp != null);
            Assert.True(displayPopupOp != null);

            // Wait for the operations to finish.
            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers = viewModel.GetPrivate<List<SharedClientScreen>>("_subscribers");
            List<SharedClientScreen> currentWindowClients = viewModel.CurrentWindowClients.ToList();

            // Check that each client registered is still present.
            Assert.True(subscribers.Count == numClients);

            // Check that all the fields for the clients are properly set.
            foreach (SharedClientScreen client in clients)
            {
                int clientIdx = subscribers.FindIndex(c => c.Id == client.Id);
                Assert.True(clientIdx != -1);
                Assert.True(subscribers[clientIdx].Name == client.Name);
                Assert.True(!subscribers[clientIdx].Pinned);
                Assert.True(subscribers[clientIdx].TileHeight >= 0);
                Assert.True(subscribers[clientIdx].TileWidth >= 0);
            }

            // Check that the subscribers are stored in sorted order.
            Assert.True(!subscribers.Zip(subscribers.Skip(1), (a, b) => String.Compare(a.Name, b.Name) != 1).Contains(false));

            // Check various fields of view model based on initial view rendering logic.
            Assert.True(currentWindowClients.Count == Math.Min(ScreenshareServerViewModel.MaxTiles, numClients));
            Assert.True(viewModel.CurrentPage == ScreenshareServerViewModel.InitialPageNumber);
            Assert.True(viewModel.TotalPages == (numClients / ScreenshareServerViewModel.MaxTiles) + 1);
            Assert.True((numClients > ScreenshareServerViewModel.MaxTiles) ? !viewModel.IsLastPage : viewModel.IsLastPage);
            Assert.True(viewModel.CurrentPageRows >= 0);
            Assert.True(viewModel.CurrentPageColumns >= 0);
            Assert.True(viewModel.IsPopupOpen == true);
            Assert.True(viewModel.PopupText != "");

            // Check that custom handler was invoked at least as many times the number of clients registered.
            Assert.True(invokedCount > 0);
            communicatorMock.Verify(communicator =>
                communicator.Send(It.IsAny<string>(), SSUtils.ModuleIdentifier, It.IsIn<string>(clientIds)),
                Times.AtLeast(numClients));

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            viewModel.PropertyChanged -= handler;
            viewModel.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests the successful registration of the clients when clients stop screen sharing.
        /// </summary>
        /// <param name="numClients">
        /// Number of clients who registered.
        /// </param>
        [Theory]
        [MemberData(nameof(NumberOfClients))]
        public void TestDeregisterClient(int numClients)
        {
            // Arrange.
            // Create view model and mock communicator.
            ScreenshareServerViewModel viewModel = ScreenshareServerViewModel.GetInstance(isDebugging: true);
            ScreenshareServer server = viewModel.GetPrivate<ScreenshareServer>("_model");
            var communicatorMock = new Mock<ICommunicator>();
            server.SetPrivate("_communicator", communicatorMock.Object);

            // Create mock clients.
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);
            List<string> clientIds = clients.Select(client => client.Id).ToList();

            // Add custom handler to the PropertyChanged event.
            int invokedCount = 0;
            PropertyChangedEventHandler handler = new((_, _) => ++invokedCount);
            viewModel.PropertyChanged += handler;

            // Act.
            // Register the clients by sending mock register packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Disable timers for the clients for testing.
            DisableTimer(server.GetPrivate<Dictionary<string, SharedClientScreen>>("_subscribers").Values.ToList());

            // Deregister the clients by sending mock deregister packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockDeregisterPacket = Utils.GetMockDeregisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockDeregisterPacket);
            }

            // Get the main thread dispatcher operations from "BeginInvoke".
            DispatcherOperation? updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            DispatcherOperation? displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            // Assert that they are being set.
            Assert.True(updateViewOp != null);
            Assert.True(displayPopupOp != null);

            // Wait for the operations to finish.
            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers = viewModel.GetPrivate<List<SharedClientScreen>>("_subscribers");
            List<SharedClientScreen> currentWindowClients = viewModel.CurrentWindowClients.ToList();

            // Check that each client registered is de-registered.
            Assert.True(subscribers.Count == 0);
            Assert.True(currentWindowClients.Count == 0);

            // Check various fields of view model based on initial view rendering logic.
            Assert.True(viewModel.CurrentPage == ScreenshareServerViewModel.InitialPageNumber);
            Assert.True(viewModel.TotalPages == ScreenshareServerViewModel.InitialTotalPages);
            Assert.True(viewModel.IsPopupOpen == true);
            Assert.True(viewModel.PopupText != "");

            // Check that custom handler was invoked at least as many times the number of clients registered.
            Assert.True(invokedCount > 0);
            communicatorMock.Verify(communicator =>
                communicator.Send(It.IsAny<string>(), SSUtils.ModuleIdentifier, It.IsIn<string>(clientIds)),
                Times.AtLeast(numClients));

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            viewModel.PropertyChanged -= handler;
            viewModel.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests the successful marking of a single client as pinned.
        /// </summary>
        /// <param name="numClients">
        /// Number of clients who registered.
        /// </param>
        [Theory]
        [MemberData(nameof(NumberOfClients))]
        public void TestOnPinSingle(int numClients)
        {
            // Arrange.
            // Create view model and mock communicator.
            ScreenshareServerViewModel viewModel = ScreenshareServerViewModel.GetInstance(isDebugging: true);
            ScreenshareServer server = viewModel.GetPrivate<ScreenshareServer>("_model");
            var communicatorMock = new Mock<ICommunicator>();
            server.SetPrivate("_communicator", communicatorMock.Object);

            // Create mock clients.
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);
            List<string> clientIds = clients.Select(client => client.Id).ToList();

            // Choose a client to be marked as pinned.
            SharedClientScreen pinnedClient = clients[^1];

            // Add custom handler to the PropertyChanged event.
            int invokedCount = 0;
            PropertyChangedEventHandler handler = new((_, _) => ++invokedCount);
            viewModel.PropertyChanged += handler;

            // Act.
            // Register the clients by sending mock register packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Disable timers for the clients for testing.
            DisableTimer(server.GetPrivate<Dictionary<string, SharedClientScreen>>("_subscribers").Values.ToList());

            // Get the main thread dispatcher operations from "BeginInvoke".
            DispatcherOperation? updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            DispatcherOperation? displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            // Assert that they are being set.
            Assert.True(updateViewOp != null);
            Assert.True(displayPopupOp != null);

            // Wait for the operations to finish.
            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Mark the client as pinned.
            viewModel.OnPin(pinnedClient.Id);

            // Get the updated operations.
            updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            // Assert that they are being set.
            Assert.True(updateViewOp != null);
            Assert.True(displayPopupOp != null);

            // Wait for the operations to finish.
            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers = viewModel.GetPrivate<List<SharedClientScreen>>("_subscribers");
            List<SharedClientScreen> currentWindowClients = viewModel.CurrentWindowClients.ToList();

            // Check that each client registered is still present.
            Assert.True(subscribers.Count == numClients);
            foreach (SharedClientScreen client in clients)
            {
                Assert.True(subscribers.FindIndex(c => c.Id == client.Id) != -1);
            }

            // Check that the first client in the reordered subscribers list is the pinned client.
            Assert.True(subscribers[0].Id == pinnedClient.Id);
            Assert.True(subscribers[0].Pinned);

            // Check various fields of view model based on view rendering logic.
            Assert.True(viewModel.CurrentPage == 1);
            Assert.True(viewModel.IsPopupOpen == true);
            Assert.True(viewModel.PopupText != "");

            // Check that custom handler was invoked at least as many times the number of clients registered.
            Assert.True(invokedCount > 0);
            communicatorMock.Verify(communicator =>
                communicator.Send(It.IsAny<string>(), SSUtils.ModuleIdentifier, It.IsIn<string>(clientIds)),
                Times.AtLeast(numClients));

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            viewModel.PropertyChanged -= handler;
            viewModel.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests the successful marking of multiple clients as pinned.
        /// </summary>
        /// <param name="numClients">
        /// Number o
        [Theory]
        [MemberData(nameof(NumberOfClients))]
        public void TestOnPinMultiple(int numClients)
        {
            // Execute test when we have two or more clients.
            if (numClients < 2) return;

            // Arrange.
            // Create view model and mock communicator.
            ScreenshareServerViewModel viewModel = ScreenshareServerViewModel.GetInstance(isDebugging: true);
            ScreenshareServer server = viewModel.GetPrivate<ScreenshareServer>("_model");
            var communicatorMock = new Mock<ICommunicator>();
            server.SetPrivate("_communicator", communicatorMock.Object);

            // Create mock clients.
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);
            clients = clients.OrderBy(subscriber => subscriber.Name).ToList();
            List<string> clientIds = clients.Select(client => client.Id).ToList();

            // Choose any two clients to be marked as pinned.
            SharedClientScreen pinnedClient1 = clients[0], pinnedClient2 = clients[^1];

            // Add custom handler to the PropertyChanged event.
            int invokedCount = 0;
            PropertyChangedEventHandler handler = new((_, _) => ++invokedCount);
            viewModel.PropertyChanged += handler;

            // Act.
            // Register the clients by sending mock register packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Disable timers for the clients for testing.
            DisableTimer(server.GetPrivate<Dictionary<string, SharedClientScreen>>("_subscribers").Values.ToList());

            // Get the main thread dispatcher operations from "BeginInvoke".
            DispatcherOperation? updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            DispatcherOperation? displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            // Assert that they are being set.
            Assert.True(updateViewOp != null);
            Assert.True(displayPopupOp != null);

            // Wait for the operations to finish.
            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Mark the clients as pinned and wait for their operations to finish.
            viewModel.OnPin(pinnedClient1.Id);

            updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            Assert.True(updateViewOp != null);
            Assert.True(displayPopupOp != null);

            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            viewModel.OnPin(pinnedClient2.Id);

            updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            Assert.True(updateViewOp != null);
            Assert.True(displayPopupOp != null);

            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers = viewModel.GetPrivate<List<SharedClientScreen>>("_subscribers");
            List<SharedClientScreen> currentWindowClients = viewModel.CurrentWindowClients.ToList();

            // Check that each client registered is still present.
            Assert.True(subscribers.Count == numClients);
            foreach (SharedClientScreen client in clients)
            {
                Assert.True(subscribers.FindIndex(c => c.Id == client.Id) != -1);
            }

            // Check that the first two clients in the reordered subscribers list are the pinned clients.
            Assert.True(subscribers[0].Id == pinnedClient1.Id);
            Assert.True(subscribers[1].Id == pinnedClient2.Id);
            Assert.True(subscribers[0].Pinned);
            Assert.True(subscribers[1].Pinned);

            // Check various fields of view model based on view rendering logic.
            Assert.True(viewModel.CurrentPage == 2);
            Assert.True(viewModel.IsPopupOpen == true);
            Assert.True(viewModel.PopupText != "");

            // Check that custom handler was invoked at least as many times the number of clients registered.
            Assert.True(invokedCount > 0);
            communicatorMock.Verify(communicator =>
                communicator.Send(It.IsAny<string>(), SSUtils.ModuleIdentifier, It.IsIn<string>(clientIds)),
                Times.AtLeast(numClients));

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            viewModel.PropertyChanged -= handler;
            viewModel.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests the successful marking of a client as unpinned.
        /// </summary>
        /// <param name="numClients">
        /// Number of clients who registered.
        /// </param>
        [Theory]
        [MemberData(nameof(NumberOfClients))]
        public void TestOnUnpin(int numClients)
        {
            // Arrange.
            // Create view model and mock communicator.
            ScreenshareServerViewModel viewModel = ScreenshareServerViewModel.GetInstance(isDebugging: true);
            ScreenshareServer server = viewModel.GetPrivate<ScreenshareServer>("_model");
            var communicatorMock = new Mock<ICommunicator>();
            server.SetPrivate("_communicator", communicatorMock.Object);

            // Create mock clients.
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);
            List<string> clientIds = clients.Select(client => client.Id).ToList();

            // Choose a client to be marked as pinned.
            SharedClientScreen pinnedClient = clients[^1];

            // Add custom handler to the PropertyChanged event.
            int invokedCount = 0;
            PropertyChangedEventHandler handler = new((_, _) => ++invokedCount);
            viewModel.PropertyChanged += handler;

            // Act.
            // Register the clients by sending mock register packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Disable timers for the clients for testing.
            DisableTimer(server.GetPrivate<Dictionary<string, SharedClientScreen>>("_subscribers").Values.ToList());

            // Get the main thread dispatcher operations from "BeginInvoke".
            DispatcherOperation? updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            DispatcherOperation? displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            // Assert that they are being set.
            Assert.True(updateViewOp != null);
            Assert.True(displayPopupOp != null);

            // Wait for the operations to finish.
            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Mark the client as pinned and wait for their operations to be finished.
            viewModel.OnPin(pinnedClient.Id);

            updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            Assert.True(updateViewOp != null);
            Assert.True(displayPopupOp != null);

            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Mark the client as unpinned and wait for their operations to be finished.
            viewModel.OnUnpin(pinnedClient.Id);

            updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            Assert.True(updateViewOp != null);
            Assert.True(displayPopupOp != null);

            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers = viewModel.GetPrivate<List<SharedClientScreen>>("_subscribers");
            List<SharedClientScreen> currentWindowClients = viewModel.CurrentWindowClients.ToList();

            // Check that each client registered is still present.
            Assert.True(subscribers.Count == numClients);
            foreach (SharedClientScreen client in clients)
            {
                int clientIdx = subscribers.FindIndex(c => c.Id == client.Id);
                Assert.True(clientIdx != -1);
                if (client.Id == pinnedClient.Id)
                {
                    // Assert that the pinned client is successfully unpinned.
                    Assert.True(!subscribers[clientIdx].Pinned);
                }
            }

            // Check that the subscribers are stored in sorted order.
            Assert.True(!subscribers.Zip(subscribers.Skip(1), (a, b) => String.Compare(a.Name, b.Name) != 1).Contains(false));

            // Check various fields of view model based on view rendering logic.
            Assert.True(currentWindowClients.Count == Math.Min(ScreenshareServerViewModel.MaxTiles, numClients));
            Assert.True(viewModel.CurrentPage == ScreenshareServerViewModel.InitialPageNumber);
            Assert.True(viewModel.TotalPages == (numClients / ScreenshareServerViewModel.MaxTiles) + 1);
            Assert.True(viewModel.IsPopupOpen == true);
            Assert.True(viewModel.PopupText != "");

            // Check that custom handler was invoked at least as many times the number of clients registered.
            Assert.True(invokedCount > 0);
            communicatorMock.Verify(communicator =>
                communicator.Send(It.IsAny<string>(), SSUtils.ModuleIdentifier, It.IsIn<string>(clientIds)),
                Times.AtLeast(numClients));

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            viewModel.PropertyChanged -= handler;
            viewModel.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests the successful switching of the page when no client is pinned.
        /// </summary>
        /// <param name="numClients">
        /// Number of clients who registered.
        /// </param>
        /// <param name="pageNum">
        /// Page number to switch to.
        /// </param>
        /// <param name="expectedTotalPages">
        /// Expected number of total pages.
        /// </param>
        /// <param name="expectedCurrentWindowClients">
        /// Expected number of current window clients after switching to that page.
        /// </param>
        [Theory]
        [MemberData(nameof(PageChangeNoPinnedClients))]
        public void TestPageChangeNoPinnedClients(int numClients, int pageNum, int expectedTotalPages, int expectedCurrentWindowClients)
        {
            // Arrange.
            // Create view model and mock communicator.
            ScreenshareServerViewModel viewModel = ScreenshareServerViewModel.GetInstance(isDebugging: true);
            ScreenshareServer server = viewModel.GetPrivate<ScreenshareServer>("_model");
            var communicatorMock = new Mock<ICommunicator>();
            server.SetPrivate("_communicator", communicatorMock.Object);

            // Create mock clients.
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);
            List<string> clientIds = clients.Select(client => client.Id).ToList();

            // Add custom handler to the PropertyChanged event.
            int invokedCount = 0;
            PropertyChangedEventHandler handler = new((_, _) => ++invokedCount);
            viewModel.PropertyChanged += handler;

            // Act.
            // Register the clients by sending mock register packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Disable timers for the clients for testing.
            DisableTimer(server.GetPrivate<Dictionary<string, SharedClientScreen>>("_subscribers").Values.ToList());

            // Get the main thread dispatcher operations from "BeginInvoke".
            DispatcherOperation? updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            DispatcherOperation? displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            // Assert that they are being set.
            Assert.True(updateViewOp != null);
            Assert.True(displayPopupOp != null);

            // Wait for the operations to finish.
            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Switch to the desired page and wait for the operations to finish.
            viewModel.RecomputeCurrentWindowClients(pageNum);

            updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            Assert.True(updateViewOp != null);
            Assert.True(displayPopupOp != null);

            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers = viewModel.GetPrivate<List<SharedClientScreen>>("_subscribers");
            List<SharedClientScreen> currentWindowClients = viewModel.CurrentWindowClients.ToList();

            // Check that each client registered is still present.
            Assert.True(subscribers.Count == numClients);

            // Check that all the fields for the clients are properly set.
            foreach (SharedClientScreen client in clients)
            {
                int clientIdx = subscribers.FindIndex(c => c.Id == client.Id);
                Assert.True(clientIdx != -1);
                Assert.True(subscribers[clientIdx].Name == client.Name);
                Assert.True(!subscribers[clientIdx].Pinned);
                Assert.True(subscribers[clientIdx].TileHeight >= 0);
                Assert.True(subscribers[clientIdx].TileWidth >= 0);
            }

            // Check that the subscribers are stored in sorted order.
            Assert.True(!subscribers.Zip(subscribers.Skip(1), (a, b) => String.Compare(a.Name, b.Name) != 1).Contains(false));

            // Check various fields of view model based on initial view rendering logic.
            Assert.True(currentWindowClients.Count == expectedCurrentWindowClients);
            Assert.True(viewModel.CurrentPage == pageNum);
            Assert.True(viewModel.TotalPages == expectedTotalPages);
            Assert.True(viewModel.CurrentPageRows >= 0);
            Assert.True(viewModel.CurrentPageColumns >= 0);
            Assert.True(viewModel.IsPopupOpen == true);
            Assert.True(viewModel.PopupText != "");

            // Check that custom handler was invoked at least as many times the number of clients registered.
            Assert.True(invokedCount > 0);
            communicatorMock.Verify(communicator =>
                communicator.Send(It.IsAny<string>(), SSUtils.ModuleIdentifier, It.IsIn<string>(clientIds)),
                Times.AtLeast(numClients));

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            viewModel.PropertyChanged -= handler;
            viewModel.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests the successful switching of the page when one client is pinned.
        /// </summary>
        /// <param name="numClients">
        /// Number of clients who registered.
        /// </param>
        /// <param name="pageNum">
        /// Page number to switch to.
        /// </param>
        /// <param name="expectedTotalPages">
        /// Expected number of total pages.
        /// </param>
        /// <param name="expectedCurrentWindowClients">
        /// Expected number of current window clients after switching to that page.
        /// </param>
        [Theory]
        [MemberData(nameof(PageChangePinnedClients))]
        public void TestPageChangePinnedClients(int numClients, int pageNum, int expectedTotalPages, int expectedCurrentWindowClients)
        {
            // Arrange.
            // Create view model and mock communicator.
            ScreenshareServerViewModel viewModel = ScreenshareServerViewModel.GetInstance(isDebugging: true);
            ScreenshareServer server = viewModel.GetPrivate<ScreenshareServer>("_model");
            var communicatorMock = new Mock<ICommunicator>();
            server.SetPrivate("_communicator", communicatorMock.Object);

            // Create mock clients.
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);
            List<string> clientIds = clients.Select(client => client.Id).ToList();

            // Choose a client to be marked as pinned.
            SharedClientScreen pinnedClient = clients[^1];

            // Add custom handler to the PropertyChanged event.
            int invokedCount = 0;
            PropertyChangedEventHandler handler = new((_, _) => ++invokedCount);
            viewModel.PropertyChanged += handler;

            // Act.
            // Register the clients by sending mock register packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Disable timers for the clients for testing.
            DisableTimer(server.GetPrivate<Dictionary<string, SharedClientScreen>>("_subscribers").Values.ToList());

            // Get the main thread dispatcher operations from "BeginInvoke".
            DispatcherOperation? updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            DispatcherOperation? displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            // Assert that they are being set.
            Assert.True(updateViewOp != null);
            Assert.True(displayPopupOp != null);

            // Wait for the operations to finish.
            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Mark the client as pinned and wait for the updated operations to finish.
            viewModel.OnPin(pinnedClient.Id);

            updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            Assert.True(updateViewOp != null);
            Assert.True(displayPopupOp != null);

            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Switch to the desired page and wait for the updated operations to finish.
            viewModel.RecomputeCurrentWindowClients(pageNum);

            updateViewOp = viewModel.GetPrivate<DispatcherOperation?>("_updateViewOperation");
            displayPopupOp = viewModel.GetPrivate<DispatcherOperation?>("_displayPopupOperation");

            Assert.True(updateViewOp != null);
            Assert.True(displayPopupOp != null);

            updateViewOp?.Wait();
            displayPopupOp?.Wait();

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers = viewModel.GetPrivate<List<SharedClientScreen>>("_subscribers");
            List<SharedClientScreen> currentWindowClients = viewModel.CurrentWindowClients.ToList();

            // Check that each client registered is still present.
            Assert.True(subscribers.Count == numClients);

            // Check that all the fields for the clients are properly set.
            foreach (SharedClientScreen client in clients)
            {
                int clientIdx = subscribers.FindIndex(c => c.Id == client.Id);
                Assert.True(clientIdx != -1);
                Assert.True(subscribers[clientIdx].Name == client.Name);
                Assert.True((subscribers[clientIdx].Id == pinnedClient.Id) ? subscribers[clientIdx].Pinned : !subscribers[clientIdx].Pinned);
                Assert.True(subscribers[clientIdx].TileHeight >= 0);
                Assert.True(subscribers[clientIdx].TileWidth >= 0);
            }

            // Check that the first client in the reordered subscribers list is the pinned client.
            Assert.True(subscribers[0].Id == pinnedClient.Id);
            Assert.True(subscribers[0].Pinned);

            // Check various fields of view model based on initial view rendering logic.
            Assert.True(currentWindowClients.Count == expectedCurrentWindowClients);
            Assert.True(viewModel.CurrentPage == pageNum);
            Assert.True(viewModel.TotalPages == expectedTotalPages);
            Assert.True(viewModel.CurrentPageRows >= 0);
            Assert.True(viewModel.CurrentPageColumns >= 0);
            Assert.True(viewModel.IsPopupOpen == true);
            Assert.True(viewModel.PopupText != "");

            // Check that custom handler was invoked at least as many times the number of clients registered.
            Assert.True(invokedCount > 0);
            communicatorMock.Verify(communicator =>
                communicator.Send(It.IsAny<string>(), SSUtils.ModuleIdentifier, It.IsIn<string>(clientIds)),
                Times.AtLeast(numClients));

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            viewModel.PropertyChanged -= handler;
            viewModel.Dispose();
            subscribers.Clear();
        }

        /// <summary>
        /// Tests the successful disposal of the view model.
        /// </summary>
        [Fact]
        public void TestDispose()
        {
            // Arrange.
            // Create view model and mock communicator.
            ScreenshareServerViewModel viewModel = ScreenshareServerViewModel.GetInstance(isDebugging: true);
            ScreenshareServer server = viewModel.GetPrivate<ScreenshareServer>("_model");
            var communicatorMock = new Mock<ICommunicator>();
            server.SetPrivate("_communicator", communicatorMock.Object);

            // Create mock clients.
            int numClients = 5;
            List<SharedClientScreen> clients = Utils.GetMockClients(server, numClients, isDebugging: true);

            // Act.
            // Register the clients by sending mock register packets for them to the server.
            foreach (SharedClientScreen client in clients)
            {
                string mockRegisterPacket = Utils.GetMockRegisterPacket(client.Id, client.Name);
                server.OnDataReceived(mockRegisterPacket);
            }

            // Dispose of the view model.
            viewModel.Dispose();
            // Try disposing again.
            viewModel.Dispose();

            // Assert.
            // Get the private list of subscribers stored in the server.
            List<SharedClientScreen> subscribers = viewModel.GetPrivate<List<SharedClientScreen>>("_subscribers");

            // Check that each client registered is de-registered.
            Assert.True(subscribers.Count == 0);

            // Cleanup.
            foreach (SharedClientScreen client in clients)
            {
                client.Dispose();
            }
            subscribers.Clear();
        }

        /// <summary>
        /// Helper method to disable the timer for the clients on the server to prevent
        /// timeout from occurring during testing.
        /// </summary>
        /// <param name="subscribers">
        /// List of subscribers whose timeout is to be disabled.
        /// </param>
        private static void DisableTimer(List<SharedClientScreen> subscribers)
        {
            foreach (SharedClientScreen clientScreen in subscribers)
            {
                clientScreen.GetPrivate<Timer>("_timer").Enabled = false;
            }
        }
    }
}
