using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
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
            this.Show();
            HomePage_Animate(this);
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
<<<<<<< HEAD
            

            MainScreenView mainScreenView = new MainScreenView();
=======
      
            MainScreenView mainScreenView = new MainScreenView(this.Name_box.Text, this.Email_textbox.Text, absolute_path, ImageLocation, "-1", "0");

>>>>>>> 4e005eb785df81a0b96ad0d41b852ab3565e7338
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
            if (string.IsNullOrEmpty(this.Server_PORT.Text))
            {
                this.Server_PORT.Text = "";
                this.Server_PORT_textblock.Text = "Please Enter Server PORT!!!";
                invalid=true;
            }
            if (invalid)
            {
                return;
            }
            MainScreenView mainScreenView = new MainScreenView(this.Name_box.Text, this.Email_textbox.Text, absolute_path, ImageLocation, this.Server_IP.Text, this.Server_PORT.Text);
            mainScreenView.Show();
            this.Close();
        }

        private void Logout_button_Click(object sender, RoutedEventArgs e)
        {
            AuthenticationView authenticationView = new AuthenticationView();
            authenticationView.Show();
            this.Close();
        }

        string DownloadImage(string url)
        {
            string imageName = "";
            int len_email = Email.Length;
            for(int i=0;i<len_email;i++)
            {
                if(Email[i] == '@')
                    break;
                imageName+=Email[i];
            }
            string dir = Environment.GetEnvironmentVariable("temp", EnvironmentVariableTarget.User);
            string absolute_path = System.IO.Path.Combine(dir, imageName);
            if(File.Exists(absolute_path))
            {
                return absolute_path;
            }
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

        void HomePage_Animate(HomePageView obj)
        {
            int v = 0;
            while (v != 100)
            {
                v += 1;
                obj.pb1.Dispatcher.Invoke(() => pb1.Value = v, System.Windows.Threading.DispatcherPriority.Background);
                obj.pb2.Dispatcher.Invoke(() => pb2.Value = v, System.Windows.Threading.DispatcherPriority.Background);
                obj.pb3.Dispatcher.Invoke(() => pb3.Value = v, System.Windows.Threading.DispatcherPriority.Background);
                obj.pb4.Dispatcher.Invoke(() => pb4.Value = v, System.Windows.Threading.DispatcherPriority.Background);
                obj.pb5.Dispatcher.Invoke(() => pb5.Value = v, System.Windows.Threading.DispatcherPriority.Background);
                //obj.pb6.Dispatcher.Invoke(() => pb6.Value = v, System.Windows.Threading.DispatcherPriority.Background);

                Thread.Sleep(25);
            }
        }
        private void Theme_toggle_button_Click(object sender, RoutedEventArgs e)
        {
            var dict = new ResourceDictionary();
            if (Theme_toggle_button.IsChecked != true)
            {
                dict.Source = new Uri("Theme1.xaml", UriKind.Relative);
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
            else
            {
                dict.Source = new Uri("Theme2.xaml", UriKind.Relative);
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
        }
    }
}
