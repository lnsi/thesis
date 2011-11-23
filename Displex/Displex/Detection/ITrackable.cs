using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Displex.Detection
{
    public interface ITrackable<T>
    {
        Point Center();
        int Orientation();
        int Width();
        int Height();
        bool IsSameDevice(T obj);
        bool AttemptRemove();
        void UpdatePosition();
    }
}
