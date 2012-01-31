using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Displex.Detection;

namespace Displex.Controls
{
    /// <summary>
    /// Interaction logic for DeviceControl.xaml
    /// </summary>
    public partial class DeviceControl : SurfaceUserControl
    {
        //public delegate void DCEventHandler(object sender);
        //public event DCEventHandler Disconnected;
        public virtual event DeviceRemoved Disconnected;
        public virtual event ControlMinimized Minimized;
        public virtual event RoutedEventHandler Demaximized;

        public bool maximized;

        public IDevice device { get; set; }

        public DeviceControl(IDevice device)
        {
            this.device = device;
            InitializeComponent();
            ContactDown += _ContactDown;
            ContactUp += _ContactUp;
            ContactChanged += _ContactChanged;
            //ContactTapGesture += new ContactEventHandler(_ContactTap);
        }

        public void Connect()
        {
            Connect(device.Ip());
        }

        public void Connect(String ip)
        {
            rdfWPF.Connect(ip);
        }

        //public void closeButton_Click(object sender, RoutedEventArgs e)
        //{
        //    //rdfWPF.Disconnect();
        //    Disconnected(this, new TrackerEventArgs(device, TrackerEventType.Removed));
        //}

        void OnRender()
        {

        }

        private DateTime lastTapTime = DateTime.Now;

        public void _ContactTap(object sender, ContactEventArgs e)
        {
            //base.OnContactTapGesture(e);
            if (!e.Contact.IsFingerRecognized)
                return;
            if (IsMetaContact(e))
            {
                DateTime now = DateTime.Now;
                if (now.Subtract(lastTapTime).Seconds < 1 && now.Subtract(lastTapTime).Milliseconds < 300)
                {
                    if (maximized)
                    {
                        removeRotateButtons();
                        Demaximized(this, new RoutedEventArgs());
                        e.Handled = true;
                    }
                    else
                    {
                        Minimized(this, new MinimizeEventArgs(device, MinimizeEventType.Minimized, e.Contact.GetPosition(null)));
                        e.Handled = true;
                    }
                }
                lastTapTime = DateTime.Now;
                return;
            }

            //Point touchPoint = MapPosition(e.GetPosition(rdfWPF.ImageRDF));
            //rdfWPF.ContactDown(touchPoint);
            //rdfWPF.ContactUp(touchPoint);
            //Console.WriteLine("ContactTap({0:00.00}, {1:00.00})", touchPoint.X, touchPoint.Y);
            //e.Handled = true;
        }

        public void _ContactDown(object sender, ContactEventArgs e)
        {
            Console.WriteLine();

            if (!e.Contact.IsFingerRecognized)
            {
                //Console.WriteLine("blob width " + e.Contact.BoundingRect.Width);
                //Console.WriteLine("blob height " + e.Contact.BoundingRect.Height);
                if (e.Contact.BoundingRect.Width > 15 && e.Contact.BoundingRect.Height > 15)
                {
                    Minimized(this, new MinimizeEventArgs(device, MinimizeEventType.Minimized, e.Contact.GetPosition(null)));
                    e.Handled = true;
                }
                return;
            }
            if (IsMetaContact(e))
                return;

            Point touchPoint = MapPosition(e.GetPosition(rdfWPF.ImageRDF));
            rdfWPF.ContactDown(touchPoint);
            //Console.WriteLine("ContactDown({0:00.00}, {1:00.00})", touchPoint.X, touchPoint.Y);
            e.Handled = true;
        }

        public void _ContactUp(object sender, ContactEventArgs e)
        {
            if (!e.Contact.IsFingerRecognized)
                return;
            if (IsMetaContact(e))
                return;

            Point touchPoint = MapPosition(e.GetPosition(rdfWPF.ImageRDF));
            rdfWPF.ContactUp(touchPoint);
            //Console.WriteLine("ContactUp({0:00.00}, {1:00.00})\n", touchPoint.X, touchPoint.Y);

            e.Handled = true;
        }

        public void _ContactChanged(object sender, ContactEventArgs e)
        {
            //base.OnContactChanged(e);
            if (!e.Contact.IsFingerRecognized)
                return;
            if (IsMetaContact(e))
                return;

            Point touchPoint = MapPosition(e.GetPosition(rdfWPF.ImageRDF));
            rdfWPF.ContactChange(touchPoint);
            //Console.WriteLine(".");
            e.Handled = true;
        }

        public void _ContactHold(object sender, ContactEventArgs e)
        {
            if (!e.Contact.IsFingerRecognized || maximized)
                return;
            if (IsMetaContact(e))
            {
                //Minimized(this, new MinimizeEventArgs(device, MinimizeEventType.Minimized, e.Contact.GetPosition(null)));
                Disconnected(this, new TrackerEventArgs(device, TrackerEventType.Removed));
            }
            
            e.Handled = true;
        }

        private bool IsMetaContact(ContactEventArgs e)
        {
            //if (e.Contact.DirectlyOver != rdfWPF.ImageRDF)

            ScatterViewItem parentSVI = this.Parent as ScatterViewItem;

            if (e.Contact.GetCenterPosition(rdfWPF.ImageRDF).X >= 0
                && e.Contact.GetCenterPosition(rdfWPF.ImageRDF).X <= rdfWPF.ImageRDF.ActualWidth
                && e.Contact.GetCenterPosition(rdfWPF.ImageRDF).Y >= 0
                && e.Contact.GetCenterPosition(rdfWPF.ImageRDF).Y <= rdfWPF.ImageRDF.ActualHeight)
            {
                parentSVI.CanMove = false;
                parentSVI.CanScale = false;
                parentSVI.CanRotate = false;
                return false;
            }
            else
            {
                parentSVI.CanMove = true;
                parentSVI.CanScale = true;
                parentSVI.CanRotate = true;
                return true;
            }   
        }

        private Point MapPosition(Point currentPosition)
        {
            //Console.WriteLine("WIDTH: " + rdfWPF.ImageRDF.ActualWidth);
            //Console.WriteLine("HEIGHT: " + rdfWPF.ImageRDF.ActualHeight);

            return new Point(
                (currentPosition.X * rdfWPF.serverWidth) / rdfWPF.ImageRDF.ActualWidth,
                (currentPosition.Y * rdfWPF.serverHeight) / rdfWPF.ImageRDF.ActualHeight);
        }

        private SurfaceButton leftButton, rightButton;

        public void addRotateButtons()
        {
            string packUri = "pack://application:,,,/Resources/rotateLeft.png";
            Image img = new Image();
            img.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
            leftButton = new SurfaceButton();
            leftButton.Content = img;
            leftButton.Click += new RoutedEventHandler(rotateLeft_Click);
            leftButton.Background = System.Windows.Media.Brushes.Transparent;
            leftButton.BorderBrush = System.Windows.Media.Brushes.Transparent;
            leftButton.Padding = new Thickness(0);
            leftButton.Margin = new Thickness(3,0,0,0);
            leftButton.VerticalAlignment = VerticalAlignment.Center;
            leftButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            leftButton.SetResourceReference(FrameworkElement.StyleProperty, "SurfaceButtonStyleInv");
            MainGrid.Children.Add(leftButton);
            leftButton.SetValue(Grid.ColumnProperty, 0);
            leftButton.SetValue(Grid.RowProperty, 1);

            string packUri2 = "pack://application:,,,/Resources/rotateRight.png";
            Image img2 = new Image();
            img2.Source = new ImageSourceConverter().ConvertFromString(packUri2) as ImageSource;
            rightButton = new SurfaceButton();
            rightButton.Content = img2;
            rightButton.Click += new RoutedEventHandler(rotateRight_Click);
            rightButton.Background = System.Windows.Media.Brushes.Transparent;
            rightButton.BorderBrush = System.Windows.Media.Brushes.Transparent;
            rightButton.Padding = new Thickness(0);
            rightButton.Margin = new Thickness(0,0 ,3,0);
            rightButton.VerticalAlignment = VerticalAlignment.Center;
            rightButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            rightButton.SetResourceReference(FrameworkElement.StyleProperty, "SurfaceButtonStyleInv");
            MainGrid.Children.Add(rightButton);
            rightButton.SetValue(Grid.ColumnProperty, 2);
            rightButton.SetValue(Grid.RowProperty, 1);
        }

        public void removeRotateButtons()
        {
            MainGrid.Children.Remove(leftButton);
            MainGrid.Children.Remove(rightButton);
            MainGrid.UpdateLayout();
        }

        public void rotateLeft_Click(object sender, RoutedEventArgs e)
        {
            //Displex.MainWindow window = Application.Current.MainWindow as Displex.MainWindow;
            //window.Maximize((ScatterViewItem)this.Parent, 90);
            ((Displex.MainWindow)Application.Current.MainWindow).Maximize((ScatterViewItem)this.Parent, 90);
        }

        public void rotateRight_Click(object sender, RoutedEventArgs e)
        {
            ((Displex.MainWindow)Application.Current.MainWindow).Maximize((ScatterViewItem)this.Parent, -90);
        }

    }

    //public class PercentageConverter : IValueConverter
    //{
    //    public object Convert(object value,
    //        Type targetType,
    //        object parameter,
    //        System.Globalization.CultureInfo culture)
    //    {
    //        return System.Convert.ToDouble(value) *
    //               System.Convert.ToDouble(parameter);
    //    }

    //    public object ConvertBack(object value,
    //        Type targetType,
    //        object parameter,
    //        System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
