<<<<<<< HEAD
﻿using PlexShareScreenshare;
using PlexShareScreenshare.Server;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
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
>>>>>>> main

namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for ScreenshareServerView.xaml
    /// </summary>
    public partial class ScreenshareServerView : Page
    {
<<<<<<< HEAD
        /// <summary>
        /// Constructor for the ScreenshareServerView Class
        /// </summary>
        public ScreenshareServerView()
        {
            //Initialise the component and the ViewModel
            InitializeComponent();
            ScreenshareServerViewModel viewModel = ScreenshareServerViewModel.GetInstance();
            this.DataContext = viewModel;

            Trace.WriteLine(Utils.GetDebugMessage("Created the ScreenshareServerView Component\n", withTimeStamp: true));


            Debug.WriteLine(viewModel.CurrentWindowClients.Count);
        }

        /// <summary>
        /// This function calls the OnPin function of the viewModel which pins the tile on which the user has clicked 
        /// The argument given to OnPin is the ClientID of user which has to be pinned, stored in Command Parameter 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPinButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button pinButton)
            {
                ScreenshareServerViewModel? viewModel = this.DataContext as ScreenshareServerViewModel;

                Debug.Assert(pinButton != null, Utils.GetDebugMessage("Pin Button is not created properly"));
                Debug.Assert(pinButton.CommandParameter != null, "ClientId received to pin does not exist");
                Debug.Assert(viewModel != null, Utils.GetDebugMessage("View Model could not be created"));

                viewModel.OnPin(pinButton.CommandParameter.ToString()!);
            }

            Trace.WriteLine(Utils.GetDebugMessage("Pin Button Clicked\n", withTimeStamp: true));
        }

        /// <summary>
        /// This function increases the current page number by 1
        /// If on the last page, next page button is not accessible and so is this function 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNextPageButtonClicked(object sender, RoutedEventArgs e)
        {
            ScreenshareServerViewModel? viewModel = this.DataContext as ScreenshareServerViewModel;
            Debug.Assert(viewModel != null, Utils.GetDebugMessage("View Model could not be created"));
            viewModel.CurrentPage += 1;

            Trace.WriteLine(Utils.GetDebugMessage("Next Page Button Clicked\n", withTimeStamp: true));
        }

        /// <summary>
        /// This function decreases the current page number by 1
        /// If on the first page, previous button is not accessible and so is this function 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviousPageButtonClicked(object sender, RoutedEventArgs e)
        {
            ScreenshareServerViewModel? viewModel = this.DataContext as ScreenshareServerViewModel;
            Debug.Assert(viewModel != null, Utils.GetDebugMessage("View Model could not be created"));
            viewModel.CurrentPage -= 1;

            Trace.WriteLine(Utils.GetDebugMessage("Previous Page Button Clicked\n", withTimeStamp: true));
        }

        /// <summary>
        /// This function calls the OnUnpin function of the ViewModel which will unpin the tile the user clicked on
        /// The argument given to Unpin function is the Client ID which has to be unpinned, stored in the Command Parameter 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnUnpinButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button someButton)
            {
                ScreenshareServerViewModel? viewModel = this.DataContext as ScreenshareServerViewModel;

                Debug.Assert(someButton != null, Utils.GetDebugMessage("Unpin Button is not created properly"));
                Debug.Assert(someButton.CommandParameter != null, "ClientId received to unpin does not exist");
                Debug.Assert(viewModel != null, Utils.GetDebugMessage("View Model could not be created"));
                viewModel.OnUnpin(someButton.CommandParameter.ToString()!);
            }

            Trace.WriteLine(Utils.GetDebugMessage("Unpin Button Clicked\n", withTimeStamp: true));
=======
        public ScreenshareServerView()
        {
            InitializeComponent();
>>>>>>> main
        }
    }
}
