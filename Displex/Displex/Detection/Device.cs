using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Displex.Detection
{
    public abstract class Device : ITrackable<Device>
    {
        #region IDeviceTrackable<Device> Members

        public abstract System.Drawing.Point Center();
        public abstract int Orientation();
        public abstract int Width();
        public abstract int Height();
        public abstract bool IsSameDevice(Device obj);
        public abstract bool AttemptRemove();
        public abstract void UpdatePosition();

        #endregion
    }
}
