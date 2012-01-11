using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Displex.Detection
{
    public interface IDevice
    {
        Point Center();
        int Orientation();
        int Width();
        int Height();
        string Ip();
        bool IsSameDevice(IDevice device);
        bool CanBeRemoved();
        void UpdatePosition();
        DeviceControl Control { get; set; }
    }
}
