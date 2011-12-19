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
    /// Interaction logic for DebugDeviceControl.xaml
    /// </summary>
    public partial class DebugDeviceControl : SurfaceUserControl
    {
        public DebugDeviceControl()
        {
            InitializeComponent();
            PreviewContactDown += new ContactEventHandler(_ContactDown);
            PreviewContactUp += new ContactEventHandler(_ContactUp);
            PreviewContactChanged += new ContactEventHandler(_ContactChanged);

            //MainSVI.ApplyTemplate();
            //MainSVI.Background = new SolidColorBrush(Colors.Transparent);
            //MainSVI.ShowsActivationEffects = false;
            //Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome ssc;
            //ssc = MainSVI.Template.FindName("shadow", MainSVI) as Microsoft.Surface.Presentation.Generic.SurfaceShadowChrome;
            //ssc.Visibility = Visibility.Hidden;
        }

        public Image Screen
        {
            get { return iphoneScreen; }
        }

        protected void _ContactDown(object sender, ContactEventArgs e)
        {
            
            //base.OnContactDown(e);
            Console.WriteLine("down");

            if (!e.Contact.IsFingerRecognized)
                return;
            if (IsMetaContact(e))
                return;
        }

        protected void _ContactUp(object sender, ContactEventArgs e)
        {
            //base.OnContactUp(e);
            Console.WriteLine("up");

            if (!e.Contact.IsFingerRecognized)
                return;
            if (IsMetaContact(e))
                return;
        }

        protected void _ContactChanged(object sender, ContactEventArgs e)
        {
            //base.OnContactChanged(e);
            Console.WriteLine("changed");

            if (!e.Contact.IsFingerRecognized)
                return;
            if (IsMetaContact(e))
                return;
        }

        private bool IsMetaContact(ContactEventArgs e)
        {
            //if (e.Contact.DirectlyOver != rdfWPF.ImageRDF)

            if (e.Contact.GetCenterPosition(Screen).X >= 0
                && e.Contact.GetCenterPosition(Screen).X <= Screen.ActualWidth
                && e.Contact.GetCenterPosition(Screen).Y >= 0
                && e.Contact.GetCenterPosition(Screen).Y <= Screen.ActualHeight)
            {
                //MainSVI.CanMove = false;
                //MainSVI.CanScale = false;
                //MainSVI.CanRotate = false;
                return false;
            }
            else
            {
                //MainSVI.CanMove = true;
                //MainSVI.CanScale = true;
                //MainSVI.CanRotate = true;
                return true;
            }
        }

        private void homeButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
