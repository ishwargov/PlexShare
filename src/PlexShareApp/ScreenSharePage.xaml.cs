using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using PlexShareScreenshare;
using PlexShareScreenshare.Client;
using PlexShareScreenshare.Server;

namespace PlexShareApp


{
    /// <summary>
    /// Interaction logic for ScreenSharePage.xaml
    /// </summary>
    public partial class ScreenSharePage : Page
    {
        public ObservableCollection<SharedClient> ImageList = new();
        readonly ScreenShareViewModel viewModel;
        private static int id = 1;
        private static int name = 100;
        private static bool pinned = false;

        /// <summary>
        /// Constructor for the ScreensharePage Class
        /// </summary>
        public ScreenSharePage()
        {
            InitializeComponent();
            string directory = @"C:\Users\HARSH\OneDrive\Documents\SE\PlexShare\src\PlexShareApp\Icons\";

            Debug.WriteLine("Inside ScreenSharePage");

            foreach (string myFile in Directory.GetFiles(directory, "*.jpg", SearchOption.AllDirectories))
            {
                Debug.WriteLine(myFile);
                Image image = new();
                BitmapImage Source = new(new Uri(myFile, UriKind.RelativeOrAbsolute));
                //Debug.WriteLine(Source.GetHashCode());
                //Source.EndInit();
                //image.Source = Source;
                if (ImageList.Count == 5)
                    break;
                Debug.WriteLine(image);
                ImageList.Add(new SharedClient(id, name.ToString(), Source));
                id++;
                name--;
            }


            viewModel = new(ImageList);
            this.DataContext = viewModel;

            Debug.WriteLine(ImageList.Count);
        }

        private void OnPinButtonClicked(object sender, RoutedEventArgs e)
        {
            //if (!string.IsNullOrEmpty(PinButton.Path))
            //{
            //    Debug.WriteLine(this.SendButton.Path);
            //    string text = this.SendTextBox.Text;
            //    this.SendTextBox.Text = string.Empty;

            //    MessengerViewModel viewModel = this.DataContext as MessengerViewModel;
            //    viewModel.OutboundMessage = text;
            //}

            //MessengerViewModel viewModel = this.DataContext as MessengerViewModel;
            //viewModel.OnPin(id);

            if (sender != null)
            {
                if (sender is Button someButton)
                {
                    Debug.WriteLine(someButton.CommandParameter);
                }

                Debug.WriteLine(sender);
            }

            
            pinned = (pinned)?false:true;
            //Image imageToFind = FindElementInItemsControlItemAtIndex(items, indexOfItemToFind, "imageToFind") as Image;
            //if (imageToFind != null)
            //{
            //    BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Logo.png"));
            //    imageToFind.Source = bitmapImage;
            //}
            Debug.WriteLine("Pin Button Clicked\n");
            //items.Items[0] = (pinned) ? "Icons/PinButton.png" : "Icons/UnpinButton.png";
            
 
        }

        private void OnNextPageButtonClicked(object sender, RoutedEventArgs e)
        {
            // viewModel.CurrentPage += 1;
            Debug.WriteLine("Next Button Clicked\n");
            viewModel.ClearList();
            Debug.WriteLine(viewModel.ImageList.Count);
            Debug.WriteLine(ImageList.Count);
        }

        private void OnPreviousPageButtonClicked(object sender, RoutedEventArgs e)
        {
            // viewModel.CurrentPage -= 1;
            Debug.WriteLine(viewModel.ImageList.Count);
            ImageList[0].Color = "Orange";
            viewModel.AddImage(ImageList[0]);
        }
    }
}
