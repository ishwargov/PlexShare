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
using Microsoft.AspNetCore.Hosting.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for HomePageView.xaml
    /// </summary>
    public partial class HomePageView : Page
    {
        string imageUrl = "";
        string absolutePath = "";
        bool homePageAnimation = true;
        private bool sessionPageOn;
        private SessionsPage sessionPage;

        /// <summary>
        /// Intializing HomePageView which takes three arguments passed from the Authentication Page
        /// </summary>
        /// <param name="name">Name of the User</param>
        /// <param name="email">Email Id of the user</param>
        /// <param name="url">Public URL of the user</param>
        public HomePageView(string name, string email, string url)
        {
            InitializeComponent();

            // Updates the Date and time in each second in the view
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                this.Time.Text = DateTime.Now.ToString("hh:mm:ss tt");
                this.Date.Text = DateTime.Now.ToString("d MMMM yyyy, dddd");
            }, this.Dispatcher);

            // Storing it in a global variable
            imageUrl = url;
            // Updating the textbox using the arguments passed
            this.nameBox.Text = name;
            this.emailTextBox.Text = email;
            this.emailTextBox.IsEnabled = false;
            sessionPageOn = false;

            // Using ViewModel for downloading the profile image
            HomePageViewModel _viewModel = new();
            this.DataContext = _viewModel;
            // It stores the absolute path of the profile image
            absolutePath = _viewModel.DownloadImage(imageUrl, email);
            this.profilePicture.ImageSource = new BitmapImage(new Uri(absolutePath, UriKind.Absolute));

            // To initialize the session page
            sessionPage = new SessionsPage(this.emailTextBox.Text);

            // Function to start the animation
            Task task = new Task(() => HomePageAnimation(this));
            task.Start();
        }

        /// <summary>
        /// This function will make the animation in the homescreen to run forever until the user clicks the button
        /// </summary>
        /// <param name="obj">HomePageView Object</param>
        void HomePageAnimation(HomePageView obj)
        {
            int count = 0;
            int direction = 1;
            Trace.WriteLine("[UX] HomePageAnimation started");
            // Making animation run forever
            while (homePageAnimation)
            {
                if (count == 0)
                {
                    direction = 1;
                }
                else if (count == 100)
                {
                    direction = -1;
                }
                count += direction;
                obj.pb1.Dispatcher.Invoke(() => pb1.Value = count, System.Windows.Threading.DispatcherPriority.Background);
                obj.pb2.Dispatcher.Invoke(() => pb2.Value = count, System.Windows.Threading.DispatcherPriority.Background);
                obj.pb3.Dispatcher.Invoke(() => pb3.Value = count, System.Windows.Threading.DispatcherPriority.Background);
                obj.pb4.Dispatcher.Invoke(() => pb4.Value = count, System.Windows.Threading.DispatcherPriority.Background);
                obj.pb5.Dispatcher.Invoke(() => pb5.Value = count, System.Windows.Threading.DispatcherPriority.Background);
                Thread.Sleep(20);
            }
            Trace.WriteLine("[UX] HomePageAnimation stopped");
        }


        /// <summary>
        /// On clicking new meeting button, creates the homepage view 
        /// and passing name,email and url of the image,IP, PORT and server details to the Mainscreen
        /// </summary>
        private void NewMeetingButtonClick(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("[UX] Clicked New Meeting button");
            HomePageViewModel viewModel = new();
            this.DataContext = viewModel;
            //verified return = ip, port, isValidUserName, isValidIpAddress, isValidPort, isServer, isVerified, SessionID 
            List<string> verified = viewModel.VerifyCredentials(this.nameBox.Text, "-1", "0", this.emailTextBox.Text, this.imageUrl);
            if (verified[6] == "False")
            {
                if (verified[2] == "False")
                {
                    this.nameBox.Text = "";
                    this.nameBlock.Text = "Please Enter Name!!!!";
                }
                return;
            }
            homePageAnimation = false;
            MainScreenView mainScreenView = new MainScreenView(this.nameBox.Text, this.emailTextBox.Text, this.absolutePath, this.imageUrl, verified[0], verified[1], true, verified[7]);
            //mainScreenView.Show();
            this.NavigationService.Navigate(mainScreenView);
            //this.Close();
        }



        /// <summary>
        /// On clicking join meeting button, creates the homepage view 
        /// and passes the name,email and url of the image, server IP and server PORT
        /// to Mainscreen
        /// </summary>
        private void JoinButtonMeetingButton(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("[UX] Clicked Join Meeting");
            HomePageViewModel viewModel = new();
            this.DataContext = viewModel;
            //verified return = ip, port, isValidUserName, isValidIpAddress, isValidPort, isServer, isVerified, SessionID 
            List<string> verified = viewModel.VerifyCredentials(this.nameBox.Text, this.serverIP.Text, this.serverPort.Text, this.emailTextBox.Text, this.imageUrl);
            if (verified[6] == "False")
            {
                if (verified[2] == "False")
                {
                    this.nameBox.Text = "";
                    this.nameBlock.Text = "Please Enter Name!!!!";
                }
                this.serverIP.Text = "";
                this.serverIpTextBlock.Text = "Server IP not Valid !!!";
                this.serverPort.Text = "";
                this.serverPortTextBlock.Text = "Server PORT not Valid!!!";
                if (verified[3] == "False")
                {
                    this.serverIpTextBlock.Text = "Server IP didn't matched!!!";
                }
                if (verified[4] == "False")
                {
                    this.serverPortTextBlock.Text = "Server Port didn't matched!!!";
                }
                return;
            }
            homePageAnimation = false;
            MainScreenView mainScreenView = new MainScreenView(this.nameBox.Text, this.emailTextBox.Text, this.absolutePath, this.imageUrl, verified[0], verified[1], false, verified[7]);
            //mainScreenView.Show();
            this.NavigationService.Navigate(mainScreenView);

            //this.Close();
        }


        /// <summary>
        /// Changes the theme using the toggle button. It changes the resource file 
        /// that is connected from App.xaml, using which we can dynamically change the colour 
        /// and background colour of different objects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThemeToggleButtonClick(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("[UX] Toggle Theme Button Clicked");
            var dict = new ResourceDictionary();
            if (themeToggleButton.IsChecked != true)
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
        /// SessionButtonClick to open the Cloud session frame
        /// </summary>
        /// <param name="sender">Session button</param>
        /// <param name="e">Onclick</param>
        private void SessionButtonClick(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("[UX] Session Button Clicked");
            if (sessionPageOn)
            {
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