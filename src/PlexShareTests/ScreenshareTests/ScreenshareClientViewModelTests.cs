/// <author> Rudr Tiwari </author>
/// <summary>
/// Contains test for view model for screenshare client.
/// </summary>

using PlexShareScreenshare;
using PlexShareScreenshare.Client;
using System.Text.Json;
using System.Windows.Threading;

namespace PlexShareTests.ScreenshareTests
{
    [Collection("Sequential")]
    public class ScreenshareClientViewModelTests
    {
        [Fact]
        public void TestSetTrueSharingScreen()
        {
            ScreenshareClientViewModel screenshareClientViewModel = new ScreenshareClientViewModel();
            Assert.False(screenshareClientViewModel.SharingScreen);

            var model = screenshareClientViewModel.GetPrivate<ScreenshareClient>("_model");
            model.SetUser("-1", "test");
            screenshareClientViewModel.SharingScreen = true;

            DispatcherOperation? sharingScreenOp = screenshareClientViewModel.GetPrivate<DispatcherOperation?>("_sharingScreenOp");
            sharingScreenOp!.Wait();
            Assert.True(screenshareClientViewModel.SharingScreen);
            Assert.NotNull(sharingScreenOp);
        }

        [Fact]
        public void TestSetFalseSharingScreen()
        {
            ScreenshareClientViewModel screenshareClientViewModel = new ScreenshareClientViewModel();
            Assert.False(screenshareClientViewModel.SharingScreen);

            var model = screenshareClientViewModel.GetPrivate<ScreenshareClient>("_model");
            model.SetUser("-1", "test");
            screenshareClientViewModel.SharingScreen = true;
            DispatcherOperation? sharingScreenOp = screenshareClientViewModel.GetPrivate<DispatcherOperation?>("_sharingScreenOp");
            sharingScreenOp!.Wait();

            DataPacket packet = new("-1", "test", ServerDataHeader.Send.ToString(), "10");
            string serializedData = JsonSerializer.Serialize(packet);
            model.OnDataReceived(serializedData);

            screenshareClientViewModel.SharingScreen = false;
            sharingScreenOp = screenshareClientViewModel.GetPrivate<DispatcherOperation?>("_sharingScreenOp");
            sharingScreenOp!.Wait();
            Assert.False(screenshareClientViewModel.SharingScreen);
            Assert.NotNull(sharingScreenOp);
        }
    }
}
