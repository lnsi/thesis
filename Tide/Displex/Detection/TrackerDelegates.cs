using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation;

namespace Displex.Detection
{

    public delegate void DeviceAdded(object sender, TrackerEventArgs e);
    public delegate void DeviceUpdated(object sender, TrackerEventArgs e);
    public delegate void DeviceRemovedEvent(object sender, TrackerEventArgs e);

    public class TrackerEventArgs : EventArgs
    {
        public IDevice Device { get; set; }
        public TrackerEventType EventType { get; set; }

        public TrackerEventArgs(IDevice d, TrackerEventType t)
        {
            Device = d;
            EventType = t;
        }
    }

    public enum TrackerEventType
    {
        Added,
        Updated,
        Removed
    }
}
