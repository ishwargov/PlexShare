using System;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PlexShareApp
{
    /// <summary>
    /// Interaction logic for HomePageView.xaml
    /// </summary>
    public partial class HomePageView : Window
    {
        //string Name = "";
        //string Email = "";
        //string ImageLocation = "";
        public HomePageView()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                this.Time.Text = DateTime.Now.ToString("hh:mm:ss tt");
                this.Date.Text = DateTime.Now.ToString("d MMMM yyyy, dddd");
            }, this.Dispatcher);
            //Name = name;
            //Email = email;
            //ImageLocation = imageLocation;
        }

        private void New_Meeting_Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.Name_box.Text)){
                this.Name_box.Text = "";
                this.Name_block.Text = "Name!!!";
                this.Name_block.Foreground = Brushes.Red;
            }
        }
    }
}
