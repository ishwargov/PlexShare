/// <author>Mayank Singla</author>
/// <summary>
/// Defines the "SharedClientScreenTests" class which contains tests for
/// methods defined in "SharedClientScreen" class.
/// </summary>

using Moq;
using PlexShareScreenshare;
using PlexShareScreenshare.Server;
using System.Drawing;
using System.Text.Json;
using System.Timers;

using SSUtils = PlexShareScreenshare.Utils;

namespace PlexShareTests.ScreenshareTests
{
    /// <summary>
    /// Defines the "SharedClientScreenTests" class which contains tests for
    /// methods defined in "SharedClientScreen" class.
    /// </summary>
    /// <remarks>
    /// It is marked sequential to run the tests sequentially so that each test
    /// can do their cleanup work in case they are using the singleton server class.
    /// </remarks>
    [Collection("Sequential")]
    public class SharedClientScreenTests
    {
        /// <summary>
        /// Tests the successful execution of timer callback once the timeout occurs.
        /// </summary>
        /// <param name="timeOfUpdation">
        /// The time after which the timer got reset. Should be greater than the timer
        /// TIMEOUT for this test.
        /// </param>
        [Theory]
        [InlineData(5100)]
        [InlineData(6000)]
        [InlineData(7000)]
        public void TestSuccessfulTimeout(int timeOfUpdation)
        {
            // Arrange
            // Create a client which will start the underlying timer
            var serverMock = new Mock<ITimerManager>();
            SharedClientScreen client = Utils.GetMockClient(serverMock.Object);

            // Act
            // Sleep for time more than the timer TIMEOUT time
            Thread.Sleep(timeOfUpdation);

            // Assert
            // Check if the timeout callback was executed exactly once
            serverMock.Verify(server => server.OnTimeOut(It.IsAny<object>(), It.IsAny<ElapsedEventArgs>(), client.Id),
                                    Times.Once(), "OnTimeOut was never invoked");

            // Cleanup
            client.Dispose();
        }

        /// <summary>
        /// Tests the successful reset of the timer.
        /// </summary>
        /// <param name="timeLeft">
        /// Time left for the timeout when the timer was reset. For this test,
        /// it should be less than the TIMEOUT time value
        /// </param>
        [Theory]
        [InlineData(2000)]
        [InlineData(1000)]
        [InlineData(100)]
        public void TestSuccessfulTimerReset(int timeLeft)
        {
            // Arrange
            // Create a client which will start the underlying timer
            var serverMock = new Mock<ITimerManager>();
            SharedClientScreen client = Utils.GetMockClient(serverMock.Object);
            int timeOfUpdation = (int)SharedClientScreen.Timeout - timeLeft;

            // Act
            // Sleep for time less than the timer TIMEOUT time
            Thread.Sleep(timeOfUpdation);
            client.UpdateTimer();
            Thread.Sleep(timeLeft);

            // Assert
            // Check if the timeout callback was never executed
            serverMock.Verify(server => server.OnTimeOut(It.IsAny<object>(), It.IsAny<ElapsedEventArgs>(), client.Id),
                                    Times.Never(), "OnTimeOut was invoked unexpectedly");

            // Cleanup
            client.Dispose();
        }

        /// <summary>
        /// Tests that client is successfully Disposed and also its underlying timer.
        /// </summary>
        /// <param name="sleepTime">
        /// Sleep time of the thread after calling Dispose
        /// </param>
        [Theory]
        [InlineData(5000)]
        [InlineData(4000)]
        public void TestDispose(int sleepTime)
        {
            // Arrange
            // Create a client which will start the underlying timer
            var serverMock = new Mock<ITimerManager>();
            SharedClientScreen client = Utils.GetMockClient(serverMock.Object);

            // Act
            // Dispose of the client and its underlying timer
            client.Dispose();
            Thread.Sleep(sleepTime);

            // Assert
            // Check if the timeout callback was never executed
            serverMock.Verify(server => server.OnTimeOut(It.IsAny<object>(), It.IsAny<ElapsedEventArgs>(), client.Id),
                                    Times.Never(), "OnTimeOut was invoked unexpectedly");
        }

        /// <summary>
        /// Tests proper serialization and deserialization of the Image packet.
        /// </summary>
        [Fact]
        public void TestSerialization()
        {
            // Arrange
            // Create a mock client and the server
            var viewmodelMock = new Mock<IMessageListener>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            SharedClientScreen client = Utils.GetMockClient(server, isDebugging: true);

            // Create a mock serialized Image packet received by the server
            var (mockImagePacket, mockFrame) = Utils.GetMockImagePacket(client.Id, client.Name);

            // Act
            // Deserialize the received packet and image inside it
            DataPacket? imagePacket = JsonSerializer.Deserialize<DataPacket>(mockImagePacket);
            Frame? frame = JsonSerializer.Deserialize<Frame>(imagePacket!.Data);

            // Assert
            // Check if we are able to retrieve the data packet and image back correctly
            Assert.True(imagePacket != null);
            Assert.True(imagePacket!.Id == client.Id);
            Assert.True(imagePacket!.Name == client.Name);
            Assert.True(Enum.Parse<ClientDataHeader>(imagePacket!.Header) == ClientDataHeader.Image);
            Assert.True(frame != null);
            Assert.True(mockFrame == frame);

            // Cleanup
            client.Dispose();
            server.Dispose();
        }

        /// <summary>
        /// Tests the proper enqueue and dequeue of the image in the client's image queue.
        /// </summary>
        [Fact]
        public void TestPutAndGetImage()
        {
            // Arrange
            // Create a mock client and the server
            var viewmodelMock = new Mock<IMessageListener>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            SharedClientScreen client = Utils.GetMockClient(server, isDebugging: true);

            // Act
            // Put mock images into the client's image queue
            int numFrames = 10;
            List<Frame> clientFrames = Utils.GetMockFrames(numFrames);
            for (int i = 0; i < numFrames; ++i)
            {
                client.PutImage(clientFrames[i]);
            }

            // Assert
            // Check if the retrieved images are same and in order
            for (int i = 0; i < numFrames; ++i)
            {
                Frame? receivedFrame = client.GetImage(CancellationToken.None);
                Assert.NotNull(receivedFrame);
                Assert.True(clientFrames[i] == receivedFrame);
            }

            // Cleanup
            client.Dispose();
            server.Dispose();
        }

        /// <summary>
        /// Tests the proper enqueue and dequeue of the image in the client's final image queue.
        /// </summary>
        [Fact]
        public void TestPutAndGetFinalImage()
        {
            // Arrange
            // Create a mock client and the server
            var viewmodelMock = new Mock<IMessageListener>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            SharedClientScreen client = Utils.GetMockClient(server, isDebugging: true);

            // Act
            // Put mock final images into the client's final image queue
            int numImages = 10;
            List<Bitmap> clientImages = new();
            for (int i = 0; i < numImages; ++i)
            {
                Bitmap mockImage = Utils.GetMockBitmap();
                client.PutFinalImage(mockImage);
                clientImages.Add(mockImage);
            }

            // Assert
            // Check if the retrieved final images are same and in order
            for (int i = 0; i < numImages; ++i)
            {
                Bitmap? receivedImage = client.GetFinalImage(CancellationToken.None);
                Assert.NotNull(receivedImage);
                Assert.True(clientImages[i] == receivedImage);
            }

            // Cleanup
            client.Dispose();
            server.Dispose();
        }

        /// <summary>
        /// Tests that the processor task for the client starts and stops successfully.
        /// </summary>
        [Fact]
        public void TestStartAndStopProcessing()
        {
            // Arrange
            // Create a mock client and the server
            var viewmodelMock = new Mock<IMessageListener>();
            ScreenshareServer server = ScreenshareServer.GetInstance(viewmodelMock.Object, isDebugging: true);
            SharedClientScreen client = Utils.GetMockClient(server, isDebugging: true);

            // Act
            // Put mock images into the client's image queue
            int numFrames = 10;
            List<Frame> clientFrames = Utils.GetMockFrames(numFrames);
            for (int i = 0; i < numFrames; ++i)
            {
                client.PutImage(clientFrames[i]);
            }

            // Start the processing of the images for the client.
            // The task will take the image from the final image queue
            // of the client and will update its "CurrentImage" variable
            client.StartProcessing(new Action<CancellationToken>((token) =>
            {
                // If the task was already canceled
                token.ThrowIfCancellationRequested();

                // Loop till the task is not canceled
                while (!token.IsCancellationRequested)
                {
                    // End the task when cancellation is requested
                    token.ThrowIfCancellationRequested();

                    Bitmap? finalImage = client.GetFinalImage(token);

                    if (finalImage != null)
                    {
                        client.CurrentImage = SSUtils.BitmapToBitmapImage(finalImage);
                    }

                }
            }));

            // Keep the tasks running for some time
            // The stitcher will process all the images in the queue
            Thread.Sleep(10000);

            // Stop the processing of the images for the client
            client.StopProcessing().Wait();

            // Assert
            // The "CurrentImage" variable of the client should not be null at the end
            Assert.True(client.CurrentImage != null);

            // Cleanup
            client.Dispose();
            server.Dispose();
        }
    }
}
