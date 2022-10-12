using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfHelloWorld
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point startPoint;
        private Rectangle rect;
        private Ellipse circ;
        private Polyline line;
        string curr_mode = "";
        public MainWindow()
        {

            InitializeComponent();
            myCanvas.Background = Brushes.LightSteelBlue;
            TextBlock txt1 = new TextBlock();
            txt1.FontSize = 20;
            txt1.Text = "My name is Deon Saji";
            Canvas.SetTop(txt1, 50);
            Canvas.SetLeft(txt1, 10);
            myCanvas.Children.Add(txt1);
        }
       
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(myCanvas);

            if (curr_mode == "Rectangle")
            {
                rect = new Rectangle { Stroke = Brushes.Black, StrokeThickness = 3 };
                Canvas.SetLeft(rect, startPoint.X);
                Canvas.SetTop(rect, startPoint.Y);
                myCanvas.Children.Add(rect);
            }
            else if (curr_mode == "Circle")
            {
                circ = new Ellipse { Stroke = Brushes.Black, StrokeThickness = 3 };
                Canvas.SetLeft(circ, startPoint.X);
                Canvas.SetTop(circ, startPoint.Y);
                myCanvas.Children.Add(circ);
            }
            else
            {
                line = new Polyline();
                line.Stroke = new SolidColorBrush(Colors.Black);
                line.StrokeThickness = 2.0;
                myCanvas.Children.Add(line);
            }
        }
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (curr_mode == "Rectangle")
            {
                if (e.LeftButton == MouseButtonState.Released || rect == null)
                    return;

                var pos = e.GetPosition(myCanvas);

                var x = Math.Min(pos.X, startPoint.X);
                var y = Math.Min(pos.Y, startPoint.Y);

                var w = Math.Max(pos.X, startPoint.X) - x;
                var h = Math.Max(pos.Y, startPoint.Y) - y;

                rect.Width = w;
                rect.Height = h;

                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);
            }
            else if (curr_mode == "Circle")
            {
                if (e.LeftButton == MouseButtonState.Released || circ == null)
                    return;
                var pos = e.GetPosition(myCanvas);
                var x = Math.Min(pos.X, startPoint.X);
                var y = Math.Min(pos.Y, startPoint.Y);

                var w = Math.Max(pos.X, startPoint.X) - x;
                var h = Math.Max(pos.Y, startPoint.Y) - y;

                circ.Height = h;
                circ.Width = w;

                Canvas.SetLeft(circ, x);
                Canvas.SetTop(circ, y);

            }
            else
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point currentPoint = e.GetPosition(myCanvas);
                    if (startPoint != currentPoint)
                    {
                        line.Points.Add(currentPoint);
                    }
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (curr_mode == "Rectangle")
            {
                rect = null;
            }
            else if (curr_mode == "Circle")
            {
                circ = null;
            }
            else
                line = null;

        }
        private void draw_shape(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(circle))
            {
                curr_mode = "Circle";
            }
            else if (sender.Equals(rectangle))
            {
                curr_mode = "Rectangle";
            }
            else
                curr_mode = "FreeHand";   

        }
        private void clear_click(object sender, RoutedEventArgs e)
        {
            for (int i = myCanvas.Children.Count - 1; i >= 0; i += -1)
            {
                UIElement Child = myCanvas.Children[i];
                if (Child is not Button)
                    myCanvas.Children.Remove(Child);
            }
        }

       
    }

}
