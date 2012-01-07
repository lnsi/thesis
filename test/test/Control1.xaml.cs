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

namespace test
{
    /// <summary>
    /// Interaction logic for Control1.xaml
    /// </summary>
    public partial class Control1 : SurfaceUserControl
    {
        public Control1()
        {
            InitializeComponent();
        }

        protected override void OnContactDown(ContactEventArgs e)
        {
            Console.WriteLine("contact down");
            base.OnContactDown(e);

            if (!e.Contact.IsFingerRecognized)
                return;
        }

        protected override void OnContactUp(ContactEventArgs e)
        {
            Console.WriteLine("contact up");
            base.OnContactUp(e);

            if (!e.Contact.IsFingerRecognized)
                return;
        }

        protected override void OnContactChanged(ContactEventArgs e)
        {
            Console.WriteLine("contact changed ");
            base.OnContactChanged(e);

            if (!e.Contact.IsFingerRecognized)
                return;
        }
    }
}
