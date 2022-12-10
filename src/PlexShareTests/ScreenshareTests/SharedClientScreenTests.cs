/// <author>Mayank Singla</author>
/// <summary>
/// Defines the "SharedClientScreenTests" class which contains tests for
/// methods defined in "SharedClientScreen" class.
/// </summary>

using Moq;
using PlexShareScreenshare;
using PlexShareScreenshare.Server;
using System.ComponentModel;
using System.Drawing;
using System.Text.Json;
using System.Timers;

using SSUtils = PlexShareScreenshare.Utils;

namespace PlexShareTests.ScreenshareTests
{
    /// <summary>
    /// Contains tests for methods defined in "SharedClientScreen" class.
    /// </summary>
    /// <remarks>
    /// It is marked sequential to run the tests sequentially so that each test
    /// can do their cleanup work in case they are using the singleton server class.
    /// </remarks>
    [Collection("Sequential")]
    public class SharedClientScreenTests
    {
        /// <summary>
        /// Update time values after the timeout time.
        /// </summary>
        public static IEnumerable<object[]> PostTimeoutTime =>
            new List<object[]>
            {
                new object[] { SharedClientScreen.Timeout + 3000 },
                new object[] { SharedClientScreen.Timeout + 4000 },
                new object[] { SharedClientScreen.Timeout + 6000 },
            };

        /// <summary>
        /// Update time values before the timeout time.
        /// </summary>
        public static IEnumerable<object[]> PreTimeoutTime =>
            new List<object[]>
            {
                new object[] { SharedClientScreen.Timeout - 2000 },
                new object[] { SharedClientScreen.Timeout - 1000 },
                new object[] { SharedClientScreen.Timeout - 100 },
            };

        /// <summary>
        /// Tests the successful execution of timer callback once the timeout occurs.
        /// </summary>
        /// <param name="timeOfUpdation">
        /// The time after which the timer got reset. Should be greater than the timer
        /// TIMEOUT for this test.
        /// </param>
        [Theory]
        [MemberData(nameof(PostTimeoutTime))]
        public void TestSuccessfulTimeout(int timeOfUpdation)
        {
            // Arrange.
            // Create a client which will start the underlying timer.
            var serverMock = new Mock<ITimerManager>();
            SharedClientScreen client = Utils.GetMockClient(serverMock.Object);

            // Act.
            // Sleep for time more than the timer TIMEOUT time.
            Thread.Sleep(timeOfUpdation);

            // Assert.
            // Check if the timeout callback was executed exactly once.
            serverMock.Verify(server => server.OnTimeOut(It.IsAny<object>(), It.IsAny<ElapsedEventArgs>(), client.Id),
                                    Times.Once(), "OnTimeOut was never invoked");

            // Cleanup.
            client.Dispose();
        }

        /// <summary>
        /// Tests the successful reset of the timer.
        /// </summary>
        /// <param name="timeLeft">
        /// Time left for the timeout when the timer was reset. For this test,
        /// it should be less than the TIMEOUT time value.
        /// </param>
        [Theory]
        [MemberData(nameof(PreTimeoutTime))]
        public void TestSuccessfulTimerReset(int timeOfUpdation)
        {
            // Arrange.
            // Create a client which will start the underlying timer.
            var serverMock = new Mock<ITimerManager>();
            SharedClientScreen client = Utils.GetMockClient(serverMock.Object);
            int timeLeft = (int)SharedClientScreen.Timeout - timeOfUpdation;

            // Act.
            // Sleep for time less than the timer TIMEOUT time.
            Thread.Sleep(timeOfUpdation);
            client.UpdateTimer();
            Thread.Sleep(timeLeft);

            // Assert.
            // Check if the timeout callback was never executed.
            serverMock.Verify(server => server.OnTimeOut(It.IsAny<object>(), It.IsAny<ElapsedEventArgs>(), client.Id),
                                    Times.Never(), "OnTimeOut was invoked unexpectedly");

            // Cleanup.
            client.Dispose();
        }

        /// <summary>
        /// Tests that client is successfully Disposed and also its underlying timer.
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
            // Create a client which will start the underlying timer.
            var serverMock = new Mock<ITimerManager>();
            SharedClientScreen client = Utils.GetMockClient(serverMock.Object);

            // Act.
            // Dispose of the client and its underlying timer.
            client.Dispose();
            Thread.Sleep(sleepTime);
            // Try disposing again.
            client.Dispose();
            // Try accessing client again.
            client.StopProcessing();

            // Assert.
            // Check if the timeout callback was never executed.
            serverMock.Verify(server => server.OnTimeOut(It.IsAny<object>(), It.IsAny<ElapsedEventArgs>(), client.Id),
                                    Times.Never(), "OnTimeOut was invoked unexpectedly");
        }

        /// <summary>
        /// Tests proper serialization and deserialization of the Image packet.
        /// </summary>
        [Fact]
        public void TestSerialization()
        {
            // Arrange.
            // Create a mock client and the server.
            var viewmodelMock = new Mock<IMessageListener>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            SharedClientScreen client = Utils.GetMockClient(server, isDebugging: true);

            // Create a mock serialized Image packet received by the server.
            var (mockImagePacket, mockImage) = Utils.GetMockImagePacket(client.Id, client.Name);

            // Act.
            // Deserialize the received packet and image inside it.
            DataPacket? imagePacket = JsonSerializer.Deserialize<DataPacket>(mockImagePacket);

            // Assert.
            // Check if we are able to retrieve the data packet and image back correctly.
            Assert.True(imagePacket != null);
            Assert.True(imagePacket!.Id == client.Id);
            Assert.True(imagePacket!.Name == client.Name);
            Assert.True(Enum.Parse<ClientDataHeader>(imagePacket!.Header) == ClientDataHeader.Image);
            Assert.True(imagePacket!.Data != null);
            Assert.True(mockImage == imagePacket!.Data);

            // Cleanup.
            client.Dispose();
            server.Dispose();
        }

        /// <summary>
        /// Tests the proper enqueue and dequeue of the image in the client's image queue.
        /// </summary>
        [Fact]
        public void TestPutAndGetImage()
        {
            // Arrange.
            // Create a mock client and the server.
            var viewmodelMock = new Mock<IMessageListener>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            SharedClientScreen client = Utils.GetMockClient(server, isDebugging: true);

            // Act.
            // Put mock images into the client's image queue.
            int numImages = 10;
            List<string> clientImages = new();
            for (int i = 0; i < numImages; ++i)
            {
                clientImages.Add(Utils.RandomString(i + 100));
                client.PutImage(clientImages[i], client.TaskId);
            }

            // Assert.
            // Check if the retrieved images are same and in order.
            for (int i = 0; i < numImages; ++i)
            {
                string? receivedImage = client.GetImage(client.TaskId);
                Assert.NotNull(receivedImage);
                Assert.True(clientImages[i] == receivedImage);
            }

            // Cleanup.
            client.Dispose();
            server.Dispose();
        }

        /// <summary>
        /// Tests the proper enqueue and dequeue of the image in the client's final image queue.
        /// </summary>
        [Fact]
        public void TestPutAndGetFinalImage()
        {
            // Arrange.
            // Create a mock client and the server.
            var viewmodelMock = new Mock<IMessageListener>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            SharedClientScreen client = Utils.GetMockClient(server, isDebugging: true);

            // Act.
            // Put mock final images into the client's final image queue.
            int numImages = 10;
            List<Bitmap> clientImages = new();
            for (int i = 0; i < numImages; ++i)
            {
                Bitmap mockImage = Utils.GetMockBitmap();
                client.PutFinalImage(mockImage, client.TaskId);
                clientImages.Add(mockImage);
            }

            // Assert.
            // Check if the retrieved final images are same and in order.
            for (int i = 0; i < numImages; ++i)
            {
                Bitmap? receivedImage = client.GetFinalImage(client.TaskId);
                Assert.NotNull(receivedImage);
                Assert.True(clientImages[i] == receivedImage);
            }

            // Cleanup.
            client.Dispose();
            server.Dispose();
        }

        /// <summary>
        /// Tests that the processor task for the client starts and stops successfully.
        /// </summary>
        [Fact]
        public void TestStartAndStopProcessing()
        {
            // Arrange.
            // Create a mock client and the server.
            var viewmodelMock = new Mock<IMessageListener>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            SharedClientScreen client = Utils.GetMockClient(server, isDebugging: true);

            // Act.
            // Put mock images into the client's image queue.
            int numImages = 10;
            List<string> clientImages = new();
            for (int i = 0; i < numImages; ++i)
            {
                clientImages.Add(Utils.GetMockImage());
                client.PutImage(clientImages[i], client.TaskId);
            }

            // Trying to stop a task which was never started.
            client.StopProcessing();

            // Start the processing of the images for the client.
            // The task will take the image from the final image queue
            // of the client and will update its "CurrentImage" variable.
            client.StartProcessing(new((taskId) =>
            {
                // Loop till the task is not canceled.
                while (taskId == client.TaskId)
                {
                    Bitmap? finalImage = client.GetFinalImage(taskId);

                    if (finalImage != null)
                    {
                        client.CurrentImage = SSUtils.BitmapToBitmapImage(finalImage);
                    }
                }
            }));

            // Keep the tasks running for some time.
            // The stitcher will process all the images in the queue.
            Thread.Sleep(10000);

            // Trying to start an already started task.
            client.StartProcessing(new(_ => { return; }));

            // Stop the processing of the images for the client.
            client.StopProcessing();

            // Trying to stop the processing again.
            client.StopProcessing();

            // Assert.
            // The "CurrentImage" variable of the client should not be null at the end.
            Assert.True(client.CurrentImage != null);

            // Cleanup.
            client.Dispose();
            server.Dispose();
        }


        /// <summary>
        /// Tests that the OnPropertyChanged() is invoked successfully when changing the
        /// properties for the SharedClientScreen object.
        /// </summary>
        [Fact]
        public void TestOnPropertyChanged()
        {
            // Arrange.
            // Create a mock client and the server.
            var viewmodelMock = new Mock<IMessageListener>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            SharedClientScreen client = Utils.GetMockClient(server, isDebugging: true);

            // Add the handler to the property changed event.
            int invokedCount = 0;
            PropertyChangedEventHandler handler = new((_, _) => ++invokedCount);
            client.PropertyChanged += handler;

            // Act.
            // Update the properties which are supposed to raise on property changed event.
            int numPropertiesChanged = 4;
            client.CurrentImage = SSUtils.BitmapToBitmapImage(Utils.GetMockBitmap());
            client.Pinned = true;
            client.TileHeight = 100;
            client.TileWidth = 100;

            // Assert.
            // Check if the property changed event was raised as many times the properties
            // of the client was changed.
            Assert.True(invokedCount == numPropertiesChanged);

            // Cleanup.
            client.PropertyChanged -= handler;
            client.Dispose();
            server.Dispose();
        }
    }
}
