using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Shapes;
//using System.Windows.Threading;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Core;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Diagnostics;
using VncSharp;
using Displex.Detection;

namespace Displex
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        private ContactTarget contactTarget;
        private IntPtr hwnd;
        private byte[] normalizedImage;
        private ImageMetrics imageMetrics;
        private bool imageAvailable;
        private Tracker tracker;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();
            InitializeSurfaceInput();

            // Add handlers for Application activation events
            AddActivationHandlers();
        }

        private void InitializeSurfaceInput()
        {
            hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            contactTarget = new Microsoft.Surface.Core.ContactTarget(hwnd);
            contactTarget.EnableInput();

            tracker = new Tracker();
            tracker.DeviceAdded += new DeviceAdded(tracker_DeviceAdded);
            tracker.DeviceRemoved += new DeviceRemoved(tracker_DeviceRemoved);
            tracker.DeviceUpdated += new DeviceUpdated(tracker_DeviceUpdated);

            EnableRawImage();
        }

        void tracker_DeviceAdded(object sender, TrackerEventArgs e)
        {
            Status.Content = "Device added!";
            DisplayExtension.Center = new System.Windows.Point(e.Device.Center().X, e.Device.Center().Y);
            DisplayExtension.Orientation = e.Device.Orientation();
            DisplayExtension.Visibility = Visibility.Visible;

        }

        void tracker_DeviceRemoved(object sender, TrackerEventArgs e)
        {
            Status.Content = "Device Removed!";
            DisplayExtension.Visibility = Visibility.Hidden;
        }

        void tracker_DeviceUpdated(object sender, TrackerEventArgs e)
        {
            Status.Content = "Device Updated!";
            DisplayExtension.Center = new System.Windows.Point(e.Device.Center().X, e.Device.Center().Y);
            DisplayExtension.Orientation = e.Device.Orientation();
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for Application activation events
            RemoveActivationHandlers();
        }

        /// <summary>
        /// Adds handlers for Application activation events.
        /// </summary>
        private void AddActivationHandlers()
        {
            // Subscribe to surface application activation events
            ApplicationLauncher.ApplicationActivated += OnApplicationActivated;
            ApplicationLauncher.ApplicationPreviewed += OnApplicationPreviewed;
            ApplicationLauncher.ApplicationDeactivated += OnApplicationDeactivated;
        }

        /// <summary>
        /// Removes handlers for Application activation events.
        /// </summary>
        private void RemoveActivationHandlers()
        {
            // Unsubscribe from surface application activation events
            ApplicationLauncher.ApplicationActivated -= OnApplicationActivated;
            ApplicationLauncher.ApplicationPreviewed -= OnApplicationPreviewed;
            ApplicationLauncher.ApplicationDeactivated -= OnApplicationDeactivated;
        }

        /// <summary>
        /// This is called when application has been activated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationActivated(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when application is in preview mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationPreviewed(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled
            //TODO: optionally enable animations here
        }

        /// <summary>
        ///  This is called when application has been deactivated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationDeactivated(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }
        private void EnableRawImage()
        {
            contactTarget.EnableImage(ImageType.Normalized);
            contactTarget.FrameReceived += new EventHandler<FrameReceivedEventArgs>(target_FrameReceived);
        }

        private void DisableRawImage()
        {
            contactTarget.DisableImage(ImageType.Normalized);
            contactTarget.FrameReceived -= new EventHandler<FrameReceivedEventArgs>(target_FrameReceived);
        }

        void target_FrameReceived(object sender, FrameReceivedEventArgs e)
        {
            imageAvailable = false;
            int paddingLeft, paddingRight;
            if (null == normalizedImage)
            {
                imageAvailable = e.TryGetRawImage(ImageType.Normalized,
                  Microsoft.Surface.Core.InteractiveSurface.DefaultInteractiveSurface.Left,
                  Microsoft.Surface.Core.InteractiveSurface.DefaultInteractiveSurface.Top,
                  Microsoft.Surface.Core.InteractiveSurface.DefaultInteractiveSurface.Width,
                  Microsoft.Surface.Core.InteractiveSurface.DefaultInteractiveSurface.Height,
                  out normalizedImage, out imageMetrics, out paddingLeft, out paddingRight);
            }
            else
            {
                imageAvailable = e.UpdateRawImage(ImageType.Normalized, normalizedImage,
                  Microsoft.Surface.Core.InteractiveSurface.DefaultInteractiveSurface.Left,
                  Microsoft.Surface.Core.InteractiveSurface.DefaultInteractiveSurface.Top,
                  Microsoft.Surface.Core.InteractiveSurface.DefaultInteractiveSurface.Width,
                  Microsoft.Surface.Core.InteractiveSurface.DefaultInteractiveSurface.Height);
            }

            if (!imageAvailable)
                return;

            DisableRawImage();

            //System.IO.MemoryStream stream = new System.IO.MemoryStream(normalizedImage);
            //BmpBitmapDecoder decoder = new BmpBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            //ImageSource source = decoder.Frames[0];
            //source.Freeze();
            //iCapturedFrame.Source = source;

            GCHandle h = GCHandle.Alloc(normalizedImage, GCHandleType.Pinned);
            IntPtr ptr = h.AddrOfPinnedObject();
            Bitmap bitmap = new Bitmap(imageMetrics.Width,
                                  imageMetrics.Height,
                                  imageMetrics.Stride,
                                  System.Drawing.Imaging.PixelFormat.Format8bppIndexed,
                                  ptr);
            
            
            tracker.ProcessImage(bitmap);

            imageAvailable = false;
            EnableRawImage();
        }
        
        public void showDisplex(Device device)
        {
            //DisplayExtension.Center = new System.Windows.Point(iphone.Center.X, iphone.Center.Y);
            //Console.WriteLine("center placed at: " + DisplayExtension.Center.X + ", " + DisplayExtension.Center.Y);
            //DisplayExtension.Orientation = iphone.Orientation;
            //DisplayExtension.Visibility = Visibility.Visible;
            //phoneShadow.Center = DisplayExtension.ActualCenter;
            //phoneShadow.Orientation = DisplayExtension.ActualOrientation;
        }

        public void Connect(String ip)
        {
            //rdfWPF.Connect(ip);
        }

        private void SurfaceWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}