using System.Threading;
using System.Windows;

namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
            // MainBox.Text = "PlexShare";
            this.Show();
            Thread.Sleep(750);
            this.MainBox.Dispatcher.Invoke(() => MainBox.Text = "P   S   e", System.Windows.Threading.DispatcherPriority.Render);
            Thread.Sleep(500);

            this.MainBox.Dispatcher.Invoke(() => MainBox.Text = "Pl  Sh  e", System.Windows.Threading.DispatcherPriority.Render);
            Thread.Sleep(500);

            this.MainBox.Dispatcher.Invoke(() => MainBox.Text = "Ple Sha e", System.Windows.Threading.DispatcherPriority.Render);
            Thread.Sleep(500);

            this.MainBox.Dispatcher.Invoke(() => MainBox.Text = "PlexShare", System.Windows.Threading.DispatcherPriority.Background);
            Thread.Sleep(500);

        }
    }
}
