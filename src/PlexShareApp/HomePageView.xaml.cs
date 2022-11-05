using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;

namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for HomePageView.xaml
    /// </summary>
    public partial class HomePageView : Window
    {
        string Name = "";
        string Email = "";
        string ImageLocation = "";
        string absolute_path = "";
        public HomePageView(string name, string email, string imageLocation)
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                this.Time.Text = DateTime.Now.ToString("hh:mm:ss tt");
                this.Date.Text = DateTime.Now.ToString("d MMMM yyyy, dddd");
            }, this.Dispatcher);
            Name = name;
            Email = email;
            ImageLocation = imageLocation;
            this.Name_box.Text = Name;
            this.Email_textbox.Text = Email;
            this.Email_textbox.IsEnabled = false;
            absolute_path = DownloadImage(imageLocation);
            this.profile_picture.ImageSource = new BitmapImage(new Uri(absolute_path,UriKind.Absolute));
        }

        private void New_Meeting_Button_Click(object sender, RoutedEventArgs e)
        {
            bool invalid = false;
            if (string.IsNullOrEmpty(this.Name_box.Text)){
                this.Name_box.Text = "";
                this.Name_block.Text = "Please Enter Name!!!";
                invalid = true;
            }
            if (invalid)
            {
                return;
            }
            MainScreenView mainScreenView = new MainScreenView(Name,Email,absolute_path,"-1","0");
            mainScreenView.Show();
            this.Close();
        }

        private void Join_Meeting_Button_Click(object sender, RoutedEventArgs e)
        {
            bool invalid = false;
            if (string.IsNullOrEmpty(this.Name_box.Text))
            {
                this.Name_box.Text = "";
                this.Name_block.Text = "Please Enter Name!!!";
                invalid = true;
            }
            if (string.IsNullOrEmpty(this.Server_IP.Text))
            {
                this.Server_IP.Text = "";
                this.Server_IP_textblock.Text = "Please Enter Server IP!!!";
                invalid = true;
            }
            if (string.IsNullOrEmpty(this.Server_IP.Text))
            {
                this.Server_PORT.Text = "";
                this.Server_PORT_textblock.Text = "Please Enter Server PORT!!!";
                invalid=true;
            }
            if (invalid)
            {
                return;
            }
            MainScreenView mainScreenView = new MainScreenView(Name, Email, absolute_path, this.Server_IP.Text,this.Server_PORT.Text);
            mainScreenView.Show();
            this.Close();
        }

        string DownloadImage(string url)
        {
            string imageName = "UserProfilePicture.png";
            string localFileName = "./" + imageName;
            Debug.WriteLine("LOCAL FILE NAME: " + localFileName + " " + ImageLocation);
            string dir = Environment.GetEnvironmentVariable("temp", EnvironmentVariableTarget.User);
            string absolute_path = System.IO.Path.Combine(dir, imageName);
            using (WebClient webClient = new())
            { 
                webClient.DownloadFile(ImageLocation, absolute_path);
            }
            return absolute_path;
        }

        private void TitleBarDrag(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MinimizeApp(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal || WindowState == WindowState.Maximized)
                WindowState = WindowState.Minimized;
            else
                WindowState = WindowState.Normal;
        }

        private void MaximizeApp(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                WindowState = WindowState.Maximized;
            }
        }

        public void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.BorderThickness = new System.Windows.Thickness(8);
            }
            else
            {
                this.BorderThickness = new System.Windows.Thickness(0);
            }
        }

    }
}
