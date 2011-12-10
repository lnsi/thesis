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

        private ObservableCollection<IDevice> currentDevices;
        private IphoneTracker iphoneTracker;
        private ColorPalette pal;
        public bool TrackingDisabled;

        public Tracker() 
        {
            currentDevices = new ObservableCollection<IDevice>();
            iphoneTracker = new IphoneTracker();
            Console.WriteLine("tracker instantiated");
        }

        private void OnDeviceAdded(IDevice device)
        {
            if (DeviceAdded != null)
                DeviceAdded(this, new TrackerEventArgs(device, TrackerEventType.Added));
        }

        private void OnDeviceUpdated(IDevice device)
        {
            if (DeviceUpdated != null)
                DeviceUpdated(this, new TrackerEventArgs(device, TrackerEventType.Updated));
        }

        private void OnDeviceRemoved(IDevice device)
        {
            if (DeviceRemoved != null)
                DeviceRemoved(this, new TrackerEventArgs(device, TrackerEventType.Removed));
        }
        
        public void ProcessImage(Bitmap bitmap)
        {
            Convert8bppBMPToGrayscale(bitmap);
            //PerformDetection(new Image<Gray, byte>(bitmap));
            PerformOneTimeDetection(new Image<Gray, byte>(bitmap));
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

        /// <summary>
        /// process image to find all present devices and disable tracking
        /// if any devices are found
        /// </summary>
        /// <param name="gray"></param>
        private void PerformOneTimeDetection(Image<Gray, Byte> gray)
        {
            if (TrackingDisabled) return;

            gray = gray.ThresholdBinary(new Gray(30), new Gray(255));

            Contour<System.Drawing.Point> contours = gray.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
              Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST);

            IList<IDevice> foundDevices = new List<IDevice>();

            // each specific type of device must be tracked independently with specific implementation
            List<Iphone> foundIphones = iphoneTracker.FindIphones(contours);
            if (foundIphones != null)
            {
                foreach (Iphone i in foundIphones)
                    foundDevices.Add(i);
            }

            if (foundDevices.Count == 0)
            {
                TrackingDisabled = false;
                Console.WriteLine("empty");
                return;
            }
            else
            {
                foreach (IDevice device in foundDevices)
                {
                    OnDeviceAdded(device); 
                }
            TrackingDisabled = true;
            }
        }

        /// <summary>
        /// process image to continuously track all present devices
        /// </summary>
        /// <param name="gray"></param>
        private void PerformDetection(Image<Gray, Byte> gray)
        {
            gray = gray.ThresholdBinary(new Gray(30), new Gray(255));

            Contour<System.Drawing.Point> contours = gray.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
              Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST);

            IList<IDevice> foundDevices = new List<IDevice>();

            // each specific type of device must be tracked independently with specific implementation
            List<Iphone> foundIphones = iphoneTracker.FindIphones(contours);
            if (foundIphones != null)
            {
                foreach (Iphone i in foundIphones)
                    foundDevices.Add(i);
            }

            IList<IDevice> devicesToBeAdded = new List<IDevice>();
            // keeping track of indexes of current devices and whether they have disappeared or not
            bool[] notToBeRemoved = new bool[currentDevices.Count];

            if (foundDevices == null) return;

            foreach (IDevice fD in foundDevices)
            {
                bool isNew = true;
                foreach (IDevice cD in currentDevices)
                {        
                    // a found device is identified as a current device
                    if (fD.IsSameDevice(cD))
                    {
                        // the device is not new
                        isNew = false;
                        // the device should not be removed
                        notToBeRemoved[currentDevices.IndexOf(cD)] = true;

                        cD.UpdatePosition();
                        OnDeviceUpdated(cD);
                        Console.WriteLine("device updated");
                    }          
                }
                // a found device is identified as a new device
                if (isNew)
                {
                    devicesToBeAdded.Add(fD);
                }
            }    
            // attempt removing lost devices
            for (int i = 0; i < notToBeRemoved.Length; i++)
            {
                if (!notToBeRemoved[i])
                {
                    IDevice d = currentDevices.ElementAt(i);
                    if (d.CanBeRemoved())
                    {
                        currentDevices.Remove(d);
                        OnDeviceRemoved(d);
                    }
                    Console.WriteLine("attempted removal");
                }
            }
            // add new devices
            if (devicesToBeAdded != null)
            {
                foreach (IDevice d in devicesToBeAdded)
                {
                    currentDevices.Add(d);
                    OnDeviceAdded(d);
                }
            }
        }

        private void TrackDevice(IDevice device)
        {       
//            Console.WriteLine("orientation: " + device.Orientation);
            //mainWindow.showDisplex(device);
            
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
