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

namespace Displex
{
    /// <summary>
    /// Interaction logic for DeviceControl.xaml
    /// </summary>
    public partial class DeviceControl : SurfaceUserControl
    {
        //public delegate void DCEventHandler(object sender);
        //public event DCEventHandler Disconnected;
        public virtual event DeviceRemoved Disconnected;

        protected IDevice device { get; set; }

        public DeviceControl(IDevice device)
        {
            this.device = device;
            InitializeComponent();
            ContactDown += new ContactEventHandler(_ContactDown);
            ContactUp += new ContactEventHandler(_ContactUp);
            ContactChanged += new ContactEventHandler(_ContactChanged);
            ContactTapGesture += new ContactEventHandler(_ContactTap);
            Connect("10.1.1.198");
        }

        public void Connect(String ip)
        {
            rdfWPF.Connect(ip);
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            rdfWPF.Disconnect();
            Disconnected(this, new TrackerEventArgs(device, TrackerEventType.Removed));
        }

        private void homeButton_Click(object sender, RoutedEventArgs e)
        {
            rdfWPF.OnRightClick();
        }

        void OnRender()
        {

        }

        protected void _ContactTap(object sender, ContactEventArgs e)
        {
            Console.WriteLine("CONTACT TAP");

            //base.OnContactDown(e);

            if (!e.Contact.IsFingerRecognized)
                return;
            if (IsMetaContact(e))
                return;

            Point touchPoint = MapPosition(e.GetPosition(rdfWPF.ImageRDF));
            rdfWPF.ContactDown(touchPoint);
            rdfWPF.ContactUp(touchPoint);
            Console.WriteLine("ContactTap({0:00.00}, {1:00.00})", touchPoint.X, touchPoint.Y);
        }

        protected void _ContactDown(object sender, ContactEventArgs e)
        {
            Console.WriteLine("CONTACT DOWN");
            
            //base.OnContactDown(e);

            if (!e.Contact.IsFingerRecognized)
                return;
            if (IsMetaContact(e))
                return;

            Point touchPoint = MapPosition(e.GetPosition(rdfWPF.ImageRDF));
            rdfWPF.ContactDown(touchPoint);
            Console.WriteLine("ContactDown({0:00.00}, {1:00.00})", touchPoint.X, touchPoint.Y);
        }

        protected void _ContactUp(object sender, ContactEventArgs e)
        {
            //base.OnContactUp(e);
            Console.WriteLine("CONTACT UP");
            if (!e.Contact.IsFingerRecognized)
                return;
            if (IsMetaContact(e))
                return;

            Point touchPoint = MapPosition(e.GetPosition(rdfWPF.ImageRDF));
            rdfWPF.ContactUp(touchPoint);
            Console.WriteLine("ContactUp({0:00.00}, {1:00.00})\n", touchPoint.X, touchPoint.Y);
        }

        protected void _ContactChanged(object sender, ContactEventArgs e)
        {
            Console.WriteLine("CONTACT CHANGED");

            //base.OnContactChanged(e);
            
            if (!e.Contact.IsFingerRecognized)
                return;
            if (IsMetaContact(e))
                return;

            Point touchPoint = MapPosition(e.GetPosition(rdfWPF.ImageRDF));
            rdfWPF.ContactChange(touchPoint);
            Console.WriteLine(".");
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
