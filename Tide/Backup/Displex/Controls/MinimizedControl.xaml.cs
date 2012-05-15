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
            //device.Control.rdfWPF.Disconnect();
            Disconnected(this, new TrackerEventArgs(device, TrackerEventType.Removed));
            Logger.Log("exit", "minimized button");
        }

        private void restoreButton_Click(object sender, RoutedEventArgs e)
        {
            Restored(this, new MinimizeEventArgs(device, MinimizeEventType.Restored, ((ScatterViewItem)this.Parent).Center));
        }

        public void CheckPosition()
        {
            int minX = 0 + 35;
            int minY = 0 + 70;
            int maxX = 1024 - 35;
            int maxY = 768 - 70;

            bool IsInCorner = false;
            ScatterViewItem item = (ScatterViewItem)this.Parent;

            double newX = item.Center.X, newY = item.Center.Y;
            if (item.Center.X < minX) // left edge
            {
                newX = minX;
                if (item.Center.Y < minY) // top left corner
                {
                    newY = minY;
                    IsInCorner = true;
                }
                if (item.Center.Y > maxY) // bottom left corner
                {
                    newY = maxY;
                    IsInCorner = true;
                }
            }
            else if (item.Center.X > maxX) //right edge
            {
                newX = maxX;
                if (item.Center.Y < minY) // top right corner
                {
                    newY = minY;
                    IsInCorner = true;
                }
                if (item.Center.Y > maxY) // bottom right corner
                {
                    newY = maxY;
                    IsInCorner = true;
                }
            }
            else if (item.Center.Y < minY) // top edge
            {
                newY = minY;
            }
            else if (item.Center.Y > maxY) // bottom edge
            {
                newY = maxY;
            }

            item.Center = new Point(newX, newY);
            if (IsInCorner)
            {
                Disconnected(this, new TrackerEventArgs(device, TrackerEventType.Removed));
                Logger.Log("exit", "off corner");
            }
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
