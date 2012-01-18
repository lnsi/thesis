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
    /// Interaction logic for MinimizedControl.xaml
    /// </summary>
    public partial class MinimizedControl : SurfaceUserControl
    {
        public virtual event DeviceRemoved Disconnected;
        public virtual event ControlRestored Restored;

        protected IDevice device { get; set; }

        public MinimizedControl(IDevice d)
        {
            device = d;
            InitializeComponent();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            device.Control.rdfWPF.Disconnect();
            Disconnected(this, new TrackerEventArgs(device, TrackerEventType.Removed));
        }

        private void restoreButton_Click(object sender, RoutedEventArgs e)
        {
            Restored(this, new MinimizeEventArgs(device, MinimizeEventType.Restored, ((ScatterViewItem)this.Parent).Center));
        }
    }

    public delegate void ControlMinimized(object sender, MinimizeEventArgs e);
    public delegate void ControlRestored(object sender, MinimizeEventArgs e);

    public class MinimizeEventArgs : EventArgs
    {
        public IDevice Device { get; set; }
        public MinimizeEventType EventType { get; set; }
        public System.Windows.Point Position { get; set; }

        public MinimizeEventArgs(IDevice d, MinimizeEventType t, Point p)
        {
            Device = d;
            EventType = t;
            Position = p;
        }
    }

    public enum MinimizeEventType
    {
        Minimized,
        Restored
    }
}
