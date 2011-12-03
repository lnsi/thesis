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

namespace Displex
{
    /// <summary>
    /// Interaction logic for DisplexControl.xaml
    /// </summary>
    public partial class DisplexControl : SurfaceUserControl
    {
        public DisplexControl()
        {
            InitializeComponent();
        }

        public void Connect()
        {
            Connect("10.1.1.198");
        }

        public void Connect(String ip)
        {
            rdfWPF.Connect(ip);
        }

        protected override void OnContactDown(Microsoft.Surface.Presentation.ContactEventArgs e)
        {
            Console.WriteLine("contact down");
            base.OnContactDown(e);
            if (!e.Contact.IsFingerRecognized)
                return;
            if (e.Contact.DirectlyOver != rdfWPF.ImageRDF)
                return;

            //Point touchPoint = e.GetPosition(rdfWPF.ImageRDF);
            //rdfWPF.ContactDown(touchPoint);
            //Console.Write("ContactDown({0:00.00}, {1:00.00})", touchPoint.X, touchPoint.Y);
        }

        protected override void OnContactUp(Microsoft.Surface.Presentation.ContactEventArgs e)
        {
            Console.WriteLine("contact up");
            base.OnContactUp(e);
            if (!e.Contact.IsFingerRecognized)
                return;
            if (e.Contact.DirectlyOver != rdfWPF.ImageRDF)
                return;

            //Point touchPoint = e.GetPosition(rdfWPF.ImageRDF);
            //rdfWPF.ContactUp(touchPoint);
            //Console.Write("ContactUp({0:00.00}, {1:00.00})\n", touchPoint.X, touchPoint.Y);
        }

        protected override void OnContactChanged(Microsoft.Surface.Presentation.ContactEventArgs e)
        {
            Console.WriteLine("contact changed");
            base.OnContactChanged(e);
            if (!e.Contact.IsFingerRecognized)
                return;
            if (e.Contact.DirectlyOver != rdfWPF.ImageRDF)
                return;

            //Point touchPoint = e.GetPosition(rdfWPF.ImageRDF);
            //rdfWPF.ContactChange(touchPoint);
            //Console.Write(".");
        }
    }

    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToDouble(value) *
                   System.Convert.ToDouble(parameter);
        }

        public object ConvertBack(object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
