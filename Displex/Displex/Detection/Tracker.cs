using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.ObjectModel;

namespace Displex.Detection
{
    public class Tracker
    {
        public virtual event DeviceAdded DeviceAdded;
        public virtual event DeviceUpdated DeviceUpdated;
        public virtual event DeviceRemoved DeviceRemoved;

        private SurfaceWindow1 mainWindow;
        private ObservableCollection<Device> currentDevices;
        private ColorPalette pal;
        //private bool isConnected;

        public Tracker(SurfaceWindow1 window)
        {
            mainWindow = window;
        }

        private void OnDeviceAdded(Device device)
        {
            if (DeviceAdded != null)
                DeviceAdded(this, new TrackerEventArgs(device, TrackerEventType.Added));
        }

        private void OnDeviceUpdated(Device device)
        {
            if (DeviceUpdated != null)
                DeviceUpdated(this, new TrackerEventArgs(device, TrackerEventType.Updated));
        }

        private void OnDeviceRemoved(Device device)
        {
            if (DeviceRemoved != null)
                DeviceRemoved(this, new TrackerEventArgs(device, TrackerEventType.Removed));
        }
        
        public void ProcessImage(Bitmap bitmap)
        {
            Convert8bppBMPToGrayscale(bitmap);
            PerformDetection(new Image<Gray, byte>(bitmap));     
        }

        private void Convert8bppBMPToGrayscale(Bitmap bmp)
        {
            if (pal == null) // pal is defined at module level as --- ColorPalette pal;
            {
                pal = bmp.Palette;
                for (int i = 0; i < 256; i++)
                {
                    pal.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
                }
            }
            bmp.Palette = pal;
        }

        private void PerformDetection(Image<Gray, Byte> gray)
        {
            gray = gray.ThresholdBinary(new Gray(10), new Gray(400));

            Contour<System.Drawing.Point> contours = gray.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
              Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST);

            IList<Device> foundDevices = new IphoneTracker().FindIphones(contours) as IList<Device>;
            IList<Device> devicesToBeAdded = null, devicesToBeRemoved = new List<Device>(currentDevices);

            foreach (Device fD in foundDevices)
            {
                foreach (Device cD in currentDevices)
                {
                    bool isNew = true;
                    // a found device is identified as a current device
                    if (fD.IsSameDevice(cD))
                    {
                        // the device is not new
                        isNew = false;
                        // the device should not be removed
                        devicesToBeRemoved.Remove(cD);

                        cD.UpdatePosition();
                        OnDeviceUpdated(cD);
                    }
                    // a found device is identified as a new device
                    if (isNew)
                    {
                        devicesToBeAdded.Add(fD);
                    }
                }
            }
            // add new devices
            foreach (Device d in devicesToBeAdded)
            {
                currentDevices.Add(d);
                OnDeviceAdded(d);
            }
            // attempt removing lost devices
            foreach (Device d in devicesToBeRemoved)
            {
                if (d.AttemptRemove())
                {
                    currentDevices.Remove(d);
                    OnDeviceRemoved(d);
                }
            }
        }

        private void TrackDevice(Device device)
        {       
//            Console.WriteLine("orientation: " + device.Orientation);
            mainWindow.showDisplex(device);
            
            // connect only once
            //if (!isConnected)
            //{
            //    Console.WriteLine("attempting connection..");
            //    mainWindow.Connect("10.1.1.198");
            //    isConnected = true;
            //}
        }

        // Utility method to list all found circle areas.
        //private void ListCircleAreas(Contour<Point> contours)
        //{
        //    if (contours == null)
        //        return;
        //    ResetContoursNavigation(ref contours);
        //    int count = 0;
        //    for (; contours != null; contours = contours.HNext)
        //    {
        //        Console.WriteLine("area " + (++count) + ": " + contours.Area.ToString());
        //        if (contours.HNext == null) break;
        //    }
        //    Console.WriteLine();
        //}
    }
}
