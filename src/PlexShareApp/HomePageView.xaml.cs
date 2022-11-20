/************************************************************
 * Filename    = HomaPageView.xaml.cs
 *
 * Author      = Jasir
 *
 * Product     = PlexShare
 * 
 * Project     = UX Team
 *
 * Description = Interaction Logic for HomePageView.xaml
 * 
 ************************************************************/
using Client.Models;
using Dashboard;
using PlexShareDashboard.Dashboard.Client.SessionManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
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
        string Url = "";
        string absolute_path = "";
        bool homaPageanimation = true;
        private bool sessionPageOn;
        private SessionsPage sessionPage;
        /// <summary>
        /// Constructor function which takes some arguments from 
        /// authentication page.
        /// </summary>
        /// <param name="name">Name of the User</param>
        /// <param name="email">Email Id of the User</param>
        /// <param name="url">Image URL from google authentication</param>
        /// <param name="success">success is true when directly rendered from the authentication
        /// success is false when IP and PORT is not valid for joining the meeting.</param>
        public HomePageView(string name, string email, string url)
        {
            InitializeComponent();
            // Updates the Date and time in each second in the view
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                this.Time.Text = DateTime.Now.ToString("hh:mm:ss tt");
                this.Date.Text = DateTime.Now.ToString("d MMMM yyyy, dddd");
            }, this.Dispatcher);
            Name = name;
            Email = email;
            Url = url;
            this.Name_box.Text = Name;
            this.Email_textbox.Text = Email;
            this.Email_textbox.IsEnabled = false;
            sessionPageOn = false;
            // It stores the absolute path of the profile image
            absolute_path = DownloadImage(Url);
            this.profile_picture.ImageSource = new BitmapImage(new Uri(absolute_path,UriKind.Absolute));
            this.Show();
            sessionPage = new SessionsPage(Email);
            // Function to start the animation

            Task task = new Task(() => HomePage_Animate(this));
            task.Start();
        }

        void HomePage_Animate(HomePageView obj)
        {
            int v = 0;
            int direction = 1;
            // Making animation run forever
            while (homaPageanimation)
            {
                if (v == 0)
                {
                    direction = 1;
                }
                else if (v == 100)
                {
                    direction = -1;
                }

                v += direction;
                obj.pb1.Dispatcher.Invoke(() => pb1.Value = v, System.Windows.Threading.DispatcherPriority.Background);
                obj.pb2.Dispatcher.Invoke(() => pb2.Value = v, System.Windows.Threading.DispatcherPriority.Background);
                obj.pb3.Dispatcher.Invoke(() => pb3.Value = v, System.Windows.Threading.DispatcherPriority.Background);
                obj.pb4.Dispatcher.Invoke(() => pb4.Value = v, System.Windows.Threading.DispatcherPriority.Background);
                obj.pb5.Dispatcher.Invoke(() => pb5.Value = v, System.Windows.Threading.DispatcherPriority.Background);
                //obj.pb6.Dispatcher.Invoke(() => pb6.Value = v, System.Windows.Threading.DispatcherPriority.Background);

                Thread.Sleep(20);
            }
        }


        /// <summary>
        /// On clicking new meeting button, creates the homepage view 
        /// and passing name,email and url of the image for Mainscreen
        /// </summary>
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
            HomePageViewModel viewModel = new();
            this.DataContext = viewModel;
           
            List<string> verified = viewModel.VerifyCredentials(this.Name_box.Text, "-1", "0", this.Email_textbox.Text, this.Url);
            homaPageanimation = false;
            MainScreenView mainScreenView = new MainScreenView(this.Name_box.Text, this.Email_textbox.Text, this.absolute_path, this.Url, verified[1], verified[2], true, verified[3]);
            mainScreenView.Show();
            this.Close();
        }

        /// <summary>
        /// Checks if the IP address is valid or not
        /// </summary>
        /// <param name="IP">IP address in a string format</param>
        /// <returns>true if valid else false</returns>
        private bool Validate_IP(string IP)
        {
            if (IP.Length == 0)
                return false;
            string[] IP_tokens = IP.Split('.');
            if (IP_tokens.Length != 4)
                return false;
            foreach(string token in IP_tokens)
            {
                int token_value = Int32.Parse(token);
                //System.Diagnostics.Debug.WriteLine(token_value.ToString
                if (token_value < 0 || token_value > 255)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// On clicking join meeting button, creates the homepage view 
        /// and passes the name,email and url of the image, server IP and server PORT
        /// to Mainscreen
        /// </summary>
        private void Join_Meeting_Button_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("[UX] Clicked Join Meeting");
            bool invalid = false;
            if (string.IsNullOrEmpty(this.Name_box.Text))
            {
                this.Name_box.Text = "";
                this.Name_block.Text = "Please Enter Name!!!";
                invalid = true;
            }
            if (string.IsNullOrEmpty(this.Server_IP.Text) || !Validate_IP(this.Server_IP.Text))
            {
                this.Server_IP.Text = "";
                this.Server_IP_textblock.Text = "Please Enter Valid Server IP(format)!!!";
                invalid = true;
            }
            if (string.IsNullOrEmpty(this.Server_PORT.Text))
            {
                this.Server_PORT.Text = "";
                this.Server_PORT_textblock.Text = "Please Enter Server PORT!!!";
                invalid=true;
            }
            // If invalid , the user has to enter again the IP and PORT
            if (invalid)
            {
                return;
            }
            HomePageViewModel viewModel = new();
            this.DataContext = viewModel;

            List<string> verified = viewModel.VerifyCredentials(this.Name_box.Text, this.Server_IP.Text, this.Server_PORT.Text, this.Email_textbox.Text, this.Url);
            if (verified[0]!="True")
            {
                this.Server_IP.Text = "";
                this.Server_IP_textblock.Text = "Server IP didn't matched!!!";
                this.Server_PORT.Text = "";
                this.Server_PORT_textblock.Text = "Server PORT didn't matched!!!";
                return;
            }
            homaPageanimation = false;
            MainScreenView mainScreenView = new MainScreenView(this.Name_box.Text, this.Email_textbox.Text, this.absolute_path, this.Url, verified[1], verified[2], false, verified[3]);
            mainScreenView.Show();
            this.Close();
        }

        /// <summary>
        /// Remove the current object and calls the Authentication view
        /// </summary>
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
            string dir = "./Resources/";
            dir = Environment.GetEnvironmentVariable("temp", EnvironmentVariableTarget.User);
            string absolute_path = System.IO.Path.Combine(dir, imageName);
            try
            {
                if (File.Exists(absolute_path))
                {
                    return absolute_path;
                }
                using (WebClient webClient = new())
                {
                    webClient.DownloadFile(Url, absolute_path);
                }
            }
            catch(Exception e)
            {
                absolute_path = "./Resources/AuthScreenImg.jpg";
                MessageBox.Show(e.Message);
            }

            return absolute_path;
        }


        /// <summary>
        /// Changes the theme using the toggle button. It changes the resource file 
        /// that is connected from App.xaml, using which we can dynamically change the colour 
        /// and background colour of different objects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Enables dragging using the title bar
        /// </summary>
        private void TitleBarDrag(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine("[UX] Trying to move the window");
            DragMove();
        }

        /// <summary>
        /// Close the app from the title bar
        /// </summary>
        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            Environment.Exit(0);
        }

        /// <summary>
        /// Minimize the window
        /// </summary>
        private void MinimizeApp(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal || WindowState == WindowState.Maximized)
                WindowState = WindowState.Minimized;
            else
                WindowState = WindowState.Normal;
        }

        /// <summary>
        /// Maxmize the window
        /// </summary>
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


        ///<summary>
        ///  This is used to add a border thickness in the maximised window
        ///  since window is going out of bounds
        ///</summary>
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sessionPageOn)
            {
                //Session.Background = Brushes.Transparent;
                sessionPageOn = false;
                SessionPage.Content = null;
            }
            else
            {
                //Session.Background = Brushes.DarkCyan;
                sessionPageOn = true;
                SessionPage.Content = sessionPage;
            }
        }
    }
}
