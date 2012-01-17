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

        public IDevice device { get; set; }

        public DeviceControl(IDevice device)
        {
            this.device = device;
            InitializeComponent();
            ContactDown += new ContactEventHandler(_ContactDown);
            ContactUp += new ContactEventHandler(_ContactUp);
            ContactChanged += new ContactEventHandler(_ContactChanged);
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

        //public void Connect(String ip, int port)
        //{
        //    rdfWPF.VncPort = port;
        //    rdfWPF.Connect(ip);
        //}

        public void closeButton_Click(object sender, RoutedEventArgs e)
        {
            rdfWPF.Disconnect();
            Disconnected(this, new TrackerEventArgs(device, TrackerEventType.Removed));
        }

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
                if (DateTime.Now.Subtract(lastTapTime).Seconds <= 1)
                {
                    Minimized(this, new MinimizeEventArgs(device, MinimizeEventType.Minimized, e.Contact.GetPosition(null)));
                    e.Handled = true;
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
            //base.OnContactDown(e);
            if (!e.Contact.IsFingerRecognized)
                return;
            if (IsMetaContact(e))
                return;

            Point touchPoint = MapPosition(e.GetPosition(rdfWPF.ImageRDF));
            rdfWPF.ContactDown(touchPoint);
            Console.WriteLine("ContactDown({0:00.00}, {1:00.00})", touchPoint.X, touchPoint.Y);
            e.Handled = true;
        }

        public void _ContactUp(object sender, ContactEventArgs e)
        {
            //if (((ScatterViewItem)MainGrid.Parent).ActualCenter.X <= 0.0 || ((ScatterViewItem)MainGrid.Parent).ActualCenter.Y <= 0.0)
            //{
            //    Minimized(this, new MinimizeEventArgs(device, MinimizeEventType.Minimized, e.Contact.GetPosition(null)));
            //}



            //base.OnContactUp(e);
            if (!e.Contact.IsFingerRecognized)
                return;
            if (IsMetaContact(e))
                return;

            Point touchPoint = MapPosition(e.GetPosition(rdfWPF.ImageRDF));
            rdfWPF.ContactUp(touchPoint);
            Console.WriteLine("ContactUp({0:00.00}, {1:00.00})\n", touchPoint.X, touchPoint.Y);

            

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
            Console.WriteLine(".");
            e.Handled = true;
        }

        public void _ContactHold(object sender, ContactEventArgs e)
        {
            if (!e.Contact.IsFingerRecognized)
                return;
            if (IsMetaContact(e))
            {
                Minimized(this, new MinimizeEventArgs(device, MinimizeEventType.Minimized, e.Contact.GetPosition(null)));
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
            return new Point(
                (currentPosition.X * rdfWPF.serverWidth) / rdfWPF.ImageRDF.ActualWidth,
                (currentPosition.Y * rdfWPF.serverHeight) / rdfWPF.ImageRDF.ActualHeight);
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
