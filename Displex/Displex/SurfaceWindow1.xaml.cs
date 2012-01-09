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
using System.Windows.Data;

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

            //AddDebugSVI(new DebugDeviceControl());
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
            AddSVI(e.Device.Control);
            //e.Device.Control.Disconnected += new DeviceControl.DCEventHandler(Control_Disconnected);
            e.Device.Control.Disconnected += new DeviceRemoved(tracker_DeviceRemoved);
            e.Device.Control.Minimized += new ControlMinimized(Control_Minimized);
        }

        void tracker_DeviceRemoved(object sender, TrackerEventArgs e)
        {
            Console.WriteLine("Device removed by " + sender.GetType().Name);

            if (sender.GetType().Name.Equals("MinimizedControl"))
            {
                MainSV.Items.Remove(((MinimizedControl)sender).Parent);
            }

            MainSV.Items.Remove(e.Device.Control.Parent);
            MainSV.UpdateLayout();
            EnableRawImage();
        }

        void tracker_DeviceUpdated(object sender, TrackerEventArgs e)
        {
            //Status.Content = "Device Updated!";
            //DisplayExtension.Center = new System.Windows.Point(e.Device.Center().X, e.Device.Center().Y);
            //DisplayExtension.Orientation = e.Device.Orientation();
        }

        void Control_Disconnected(object sender)
        {
            //((DeviceControl)sender).Visibility = Visibility.Hidden;
        }

        void Control_Minimized(object sender, MinimizeEventArgs e)
        {
            ((ScatterViewItem)e.Device.Control.Parent).Visibility = Visibility.Hidden;

            MinimizedControl mControl = new MinimizedControl(e.Device);
            AddSVI(mControl, e.Position);

            mControl.Disconnected += new DeviceRemoved(tracker_DeviceRemoved);
            mControl.Restored += new ControlRestored(Control_Restored);
        }

        void Control_Restored(object sender, MinimizeEventArgs e)
        {
            ((ScatterViewItem)e.Device.Control.Parent).Center = e.Position;
            ((ScatterViewItem)e.Device.Control.Parent).Visibility = Visibility.Visible;
            MainSV.Items.Remove(((MinimizedControl)sender).Parent);
            MainSV.UpdateLayout();
        }

        private void AddSVI(MinimizedControl control, System.Windows.Point position)
        {
            ScatterViewItem svi = new ScatterViewItem();
            svi.Center = position;
            svi.Orientation = 0;
            svi.Height = 148;
            svi.Width = 75;
            svi.CanRotate = false;
            svi.CanScale = false;
            svi.Background = System.Windows.Media.Brushes.Transparent;
            svi.BorderBrush = System.Windows.Media.Brushes.Transparent;
            svi.SetResourceReference(StyleProperty, "ScatterViewItemStyleInvisible");

            svi.Content = control;
            MainSV.Items.Add(svi);
        }

        private void AddSVI(DeviceControl control)
        {
            ScatterViewItem svi = new ScatterViewItem();
            svi.Center = new System.Windows.Point(500, 350);
            svi.Orientation = 0;
            svi.Height = 515;
            svi.Width = 260;
            svi.Background = System.Windows.Media.Brushes.Transparent;
            svi.BorderBrush = System.Windows.Media.Brushes.Transparent;
            svi.SetResourceReference(StyleProperty, "ScatterViewItemStyleInvisible");

            svi.ContactDown += new ContactEventHandler(control._ContactDown);
            svi.ContactUp += new ContactEventHandler(control._ContactUp);
            svi.ContactChanged += new ContactEventHandler(control._ContactChanged);
            svi.ContactTapGesture += new ContactEventHandler(control._ContactTap);
            svi.ContactHoldGesture += new ContactEventHandler(control._ContactHold);

            svi.ScatterManipulationCompleted += new ScatterManipulationCompletedEventHandler(ScatterViewItem_ScatterManipulationCompleted);

            svi.Content = control;
            MainSV.Items.Add(svi);

        }

        private void AddDebugSVI(DebugDeviceControl control)
        {
            ScatterViewItem svi = new ScatterViewItem();
            svi.Center = new System.Windows.Point(500, 350);
            svi.Orientation = 0;
            svi.Height = 515;
            svi.Width = 260;
            svi.Background = System.Windows.Media.Brushes.Transparent;
            svi.BorderBrush = System.Windows.Media.Brushes.Transparent;
            svi.SetResourceReference(StyleProperty, "ScatterViewItemStyleInvisible");

            svi.Content = control;
            MainSV.Items.Add(svi);

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
            if (!tracker.TrackingDisabled)
                EnableRawImage();
        }

        private void SurfaceWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ScatterViewItem_ScatterManipulationCompleted
            (object sender, ScatterManipulationCompletedEventArgs e)
        {
            Console.WriteLine("MANIPUIELI");
            // Get a reference to the ScatterViewItem item.
            ScatterViewItem item = (ScatterViewItem)e.Source;
            if (item.Center.X < -5 || item.Center.X > 1035 || item.Center.Y < -5 || item.Center.Y > 780)
            {
                Control_Minimized(this, new MinimizeEventArgs(((DeviceControl)item.Content).device, MinimizeEventType.Minimized, CalculateEdgePosition(item.Center)));
                //Console.WriteLine(item.Content.GetType().Name);
            }
            e.Handled = true;
        }

        private System.Windows.Point CalculateEdgePosition(System.Windows.Point p)
        {
            if (p.X < -5)
            {
                return new System.Windows.Point(33, p.Y);
            }
            if (p.X > 1035)
            {
                return new System.Windows.Point(992, p.Y);
            }
            if (p.Y < -5)
            {
                return new System.Windows.Point(p.X, 70);
            }
            if (p.Y > 780)
            {
                return new System.Windows.Point(p.X, 700);
            }
            else return p;
        }
    }
}