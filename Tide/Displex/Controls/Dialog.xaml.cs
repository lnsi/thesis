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

namespace Displex.Controls
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class Dialog : SurfaceUserControl
    {
        /// <summary>
        /// The event generated after accepting questions.
        /// </summary>
        public event EventHandler Yes;

        /// <summary>
        /// The event generated after the rejection of the questions.
        /// </summary>
        public event EventHandler No;

        /// <summary>
        /// The constructor initializes the dialog box.
        /// </summary>
        public Dialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Question dialog box displays.
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Freeze buttons.
        /// </summary>
        /// <returns></returns>
        public Dialog Freeze()
        {
            yesButton.IsEnabled = false;
            noButton.IsEnabled = false;
            return this;
        }

        private void OnYesButtonClick(object sender, RoutedEventArgs e)
        {
            Freeze();
            if (Yes != null)
                Yes(this, new EventArgs());
        }

        private void OnNoButtonClick(object sender, RoutedEventArgs e)
        {
            Freeze();
            if (No != null)
                No(this, new EventArgs());
        }
    }
}
