using Moq;
using PlexShareNetwork.Communication;
using PlexShareScreenshare;
using PlexShareScreenshare.Client;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using SSUtils = PlexShareScreenshare.Utils;

namespace PlexShareTests.ScreenshareTests
{
    [Collection("Sequential")]
    public class ScreenshareClientTest
    {
        [Fact]
        public void TestSingleton()
        {
            ScreenshareClient screenshareClient = ScreenshareClient.GetInstance(true);
            Debug.Assert(screenshareClient != null);
        }

        [Fact]
        public void TestRegisterPacketSend()
        {
            ScreenshareClient screenshareClient = ScreenshareClient.GetInstance(true);

            var communicatorMock = new Mock<ICommunicator>();
            screenshareClient.SetPrivate("_communicator", communicatorMock.Object);

            string argString = "";
            communicatorMock.Setup(p => p.Send(It.IsAny<string>(), SSUtils.ModuleIdentifier, null))
                .Callback((string s, string s2, string s3) => { if (argString == "") argString = s; });

            screenshareClient.SetUser("id", "name");
            screenshareClient.StartScreensharing();

            DataPacket? packet = JsonSerializer.Deserialize<DataPacket>(argString);

            Assert.True(packet?.Header == ClientDataHeader.Register.ToString());
            //communicatorMock.Verify(communicator =>
            //    communicator.Send(It.IsAny<string>(), SSUtils.ModuleIdentifier, null),
            //    Times.AtLeastOnce);
        }

        [Fact]
        public void TestSendPacketReceive()
        {
            ScreenshareClient screenshareClient = ScreenshareClient.GetInstance(true);

            var communicatorMock = new Mock<ICommunicator>();
            screenshareClient.SetPrivate("_communicator", communicatorMock.Object);

            screenshareClient.SetUser("id", "name");
            screenshareClient.StartScreensharing();

            bool isImagePacketSent = false;
            communicatorMock.Setup(p => p.Send(It.IsAny<string>(), SSUtils.ModuleIdentifier, null))
                .Callback((string s, string s2, string s3) =>
                {
                    DataPacket? packet = JsonSerializer.Deserialize<DataPacket>(s);
                    if (packet?.Header == ClientDataHeader.Image.ToString())
                    {
                        isImagePacketSent = true;
                    }
                });

            DataPacket packet = new("id", "name", ServerDataHeader.Send.ToString(), "10");
            string serializedData = JsonSerializer.Serialize(packet);

            screenshareClient.OnDataReceived(serializedData);

            Thread.Sleep(1000);

            Assert.True(isImagePacketSent);
            screenshareClient.StopScreensharing();
        }

        [Fact]
        public void TestStopPacketReceive()
        {
            ScreenshareClient screenshareClient = ScreenshareClient.GetInstance(true);

            var communicatorMock = new Mock<ICommunicator>();
            screenshareClient.SetPrivate("_communicator", communicatorMock.Object);

            screenshareClient.SetUser("id", "name");
            screenshareClient.StartScreensharing();

            bool isImagePacketSent = false;
            communicatorMock.Setup(p => p.Send(It.IsAny<string>(), SSUtils.ModuleIdentifier, null))
                .Callback((string s, string s2, string s3) =>
                {
                    DataPacket? packet = JsonSerializer.Deserialize<DataPacket>(s);
                    if (packet?.Header == ClientDataHeader.Image.ToString())
                    {
                        isImagePacketSent = true;
                    }
                });

            DataPacket packet = new("id", "name", ServerDataHeader.Send.ToString(), "10");
            string serializedData = JsonSerializer.Serialize(packet);

            screenshareClient.OnDataReceived(serializedData);

            Thread.Sleep(1000);

            Assert.True(isImagePacketSent);

            packet = new("id", "name", ServerDataHeader.Stop.ToString(), "10");
            serializedData = JsonSerializer.Serialize(packet);

            screenshareClient.OnDataReceived(serializedData);

            Thread.Sleep(1000);

            bool? _imageCancellationToken = (bool?)typeof(ScreenshareClient)
                            .GetField("_imageCancellationToken", BindingFlags.NonPublic | BindingFlags.Instance)!
                            .GetValue(screenshareClient);

            Assert.True(_imageCancellationToken);
            screenshareClient.StopScreensharing();
        }


    }
}

// start -> stop -> start
// moq start processing sends packets to communicator (register + communication)
// using moq test on data receive both things
// using moq test start image sending
// test stop ss
// test StopConfirmationSending
// 