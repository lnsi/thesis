using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Displex.Detection
{

    public delegate void DeviceAdded(object sender, TrackerEventArgs e);
    public delegate void DeviceUpdated(object sender, TrackerEventArgs e);
    public delegate void DeviceRemoved(object sender, TrackerEventArgs e);

    public class TrackerEventArgs : EventArgs
    {
        public ITrackable<Device> Device { get; set; }
        public TrackerEventType EventType { get; set; }

        public TrackerEventArgs(ITrackable<Device> d, TrackerEventType t)
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
