using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
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
using Displex.Controls;
using System.Windows.Media.Animation;
using System.Windows.Resources;

namespace Displex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : SurfaceWindow
    {
        private ContactTarget contactTarget;
        private IntPtr hwnd;
        private byte[] normalizedImage;
        private ImageMetrics imageMetrics;
        private bool imageAvailable;
        private Tracker tracker;

        private ScatterViewItem DialogConnectSvi, DialogExitSvi;
        private bool DialogConnectActive, DialogExitActive;
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            InitializeSurfaceInput();
            InitializeDialogs();
            
            // Add handlers for Application activation events
            AddActivationHandlers();
        }

        private void InitializeSurfaceInput()
        {
            hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            contactTarget = new Microsoft.Surface.Core.ContactTarget(hwnd);
            contactTarget.EnableInput();

            tracker = new Tracker();
            tracker.DeviceAdded += tracker_DeviceAdded;
            tracker.DeviceRemoved += tracker_DeviceRemoved;
            tracker.DeviceUpdated += tracker_DeviceUpdated;
            
            EnableRawImage();
        }

        private void InitializeDialogs()
        {
            

                       
        }

        void tracker_DeviceAdded(object sender, TrackerEventArgs e)
        {
            if (!DialogConnectActive)
            {
                DialogConnectActive = true;

                DialogConnectSvi = new ScatterViewItem();
                DialogConnectSvi.CanMove = false;
                DialogConnectSvi.CanRotate = false;
                DialogConnectSvi.CanScale = false;
                DialogConnectSvi.Orientation = 0;
                DialogConnectSvi.Width = 120;
                DialogConnectSvi.Height = 65;
                //DialogConnectSvi.Center = new System.Windows.Point(200, 200);
                DialogConnectSvi.Background = System.Windows.Media.Brushes.Transparent;
                DialogConnectSvi.BorderBrush = System.Windows.Media.Brushes.Transparent;
                DialogConnectSvi.SetResourceReference(StyleProperty, "ScatterViewItemStyleInvisible");
                MainSV.Items.Add(DialogConnectSvi);
                //DialogConnectSvi.Visibility = Visibility.Hidden;

                Dialog dialog = new Dialog();
                DialogConnectSvi.Content = dialog;
                dialog.Question = "Connect to device?";
                DialogConnectSvi.Center = new System.Windows.Point(e.Device.Center().X, e.Device.Center().Y);
                //  DialogConnectSvi.Visibility = Visibility.Visible;

                Storyboard story = DialogConnectSvi.Center.X > 824 ? SetStoryTarget("showDialogLeft", DialogConnectSvi) : SetStoryTarget("showDialog", DialogConnectSvi);
                story.Begin(this);

                Action hideItem = delegate
                {
                    dialog.Freeze();
                    Storyboard endStory = SetStoryTarget("hideDialog", DialogConnectSvi);
                    endStory.Completed += delegate { MainSV.Items.Remove(DialogConnectSvi); DialogConnectActive = false; };
                    endStory.Begin(this);
                };

                dialog.No += delegate { hideItem(); };

                dialog.Yes += delegate
                {
                    hideItem();
                    AddDevice(sender, e);
                };
            }
        }

        /// <summary>
        /// Sets the control to whom the animation will be played.
        /// </summary>
        /// <param name="storyName">The name of the animation</param>
        /// <param name="target">animated object</param>
        /// <returns>Object animation class</returns>
        protected Storyboard SetStoryTarget(string storyName, DependencyObject target)
        {
            Storyboard story = FindResource(storyName) as Storyboard;
            foreach (var child in story.Children)
                Storyboard.SetTarget(child, target);
            return story;
        }

        void AddDevice(object sender, TrackerEventArgs e)
        {
            e.Device.Control.Connect();
            //e.Device.Control.Disconnected += new DeviceControl.DCEventHandler(Control_Disconnected);
            e.Device.Control.Disconnected += tracker_DeviceRemoved;
            e.Device.Control.Minimized += Control_Minimized;

            AddSVI(e.Device.Control);
            AddSVI(e.Device.MinControl);
        }

        void tracker_DeviceRemoved(object sender, TrackerEventArgs e)
        {
            if (!DialogExitActive)
            {
                DialogExitActive = true;

                ScatterViewItem control;
                if (sender.GetType().Name.Equals("MinimizedControl"))
                    control = ((ScatterViewItem)e.Device.MinControl.Parent);
                else
                    control = ((ScatterViewItem)e.Device.Control.Parent);
                control.ContactLeave += new ContactEventHandler(control_ContactLeave);

                DialogExitSvi = new ScatterViewItem();
                DialogExitSvi.CanMove = false;
                DialogExitSvi.CanRotate = false;
                DialogExitSvi.CanScale = false;
                DialogExitSvi.Orientation = 0;
                DialogExitSvi.Width = 120;
                DialogExitSvi.Height = 65;
                //DialogExitSvi.Center = new System.Windows.Point(200, 200);
                DialogExitSvi.Background = System.Windows.Media.Brushes.Transparent;
                DialogExitSvi.BorderBrush = System.Windows.Media.Brushes.Transparent;
                DialogExitSvi.SetResourceReference(StyleProperty, "ScatterViewItemStyleInvisible");
                MainSV.Items.Add(DialogExitSvi);
                //DialogExitSvi.Visibility = Visibility.Hidden; 

                Dialog dialog = new Dialog();
                DialogExitSvi.Content = dialog;
                dialog.Question = "Exit application?";
                DialogExitSvi.Center = new System.Windows.Point(control.Center.X, control.Center.Y);
                //DialogExitSvi.Visibility = Visibility.Visible;

                Storyboard story = DialogExitSvi.Center.X > 924 ? SetStoryTarget("showDialogLeft", DialogExitSvi) : SetStoryTarget("showDialog", DialogExitSvi);
                story.Begin(this);

                Action hideItem = delegate
                {
                    dialog.Freeze();
                    Storyboard endStory = SetStoryTarget("hideDialog", DialogExitSvi);
                    endStory.Completed += delegate { MainSV.Items.Remove(DialogExitSvi); DialogExitActive = false; };
                    endStory.Begin(this);
                };

                dialog.No += delegate { hideItem(); };

                dialog.Yes += delegate
                {
                    hideItem();
                    RemoveDevice(sender, e);
                };
            }
        }

        void control_ContactLeave(object sender, Microsoft.Surface.Presentation.ContactEventArgs e)
        {
            ((ScatterViewItem)sender).SetRelativeZIndex(RelativeScatterViewZIndex.Bottommost);
        }

        void RemoveDevice(object sender, TrackerEventArgs e)
        {
            e.Device.Control.rdfWPF.Disconnect();

            if (sender.GetType().Name.Equals("MinimizedControl"))
                MainSV.Items.Remove(((MinimizedControl)sender).Parent);
            
            MainSV.Items.Remove(e.Device.Control.Parent);
            MainSV.UpdateLayout();
        }

        void tracker_DeviceUpdated(object sender, TrackerEventArgs e)
        {
            //Status.Content = "Device Updated!";
            //DisplayExtension.Center = new System.Windows.Point(e.Device.Center().X, e.Device.Center().Y);
            //DisplayExtension.Orientation = e.Device.Orientation();
        }

        void Control_Minimized(object sender, MinimizeEventArgs e)
        {
            e.Device.Control.Disconnected -= tracker_DeviceRemoved;
            e.Device.Control.Minimized -= Control_Minimized;

            ScatterViewItem item = ((ScatterViewItem)e.Device.Control.Parent);
            item.Visibility = Visibility.Hidden;
            item.ScatterManipulationDelta -= ScatterViewItem_ScatterManipulationDelta;

            ScatterViewItem MinItem = ((ScatterViewItem)e.Device.MinControl.Parent);
            MinItem.Visibility = Visibility.Visible;
            MinItem.Center = e.Position;

            e.Device.MinControl.Disconnected += tracker_DeviceRemoved;
            e.Device.MinControl.Restored += Control_Restored;
            e.Device.MinControl.CheckPosition();
        }

        void Control_Restored(object sender, MinimizeEventArgs e)
        {
            e.Device.Control.Disconnected += tracker_DeviceRemoved;
            e.Device.Control.Minimized += Control_Minimized;

            e.Device.MinControl.Disconnected -= tracker_DeviceRemoved;
            e.Device.MinControl.Restored -= Control_Restored;
            ((ScatterViewItem)e.Device.MinControl.Parent).Visibility = Visibility.Hidden;

            ScatterViewItem item = ((ScatterViewItem)e.Device.Control.Parent);
            item.Center = e.Position;
            item.Visibility = Visibility.Visible;
            item.ScatterManipulationDelta += ScatterViewItem_ScatterManipulationDelta;
        }

        private void ScatterViewItem_ScatterManipulationCompleted
            (object sender, ScatterManipulationCompletedEventArgs e)
        {
            Console.WriteLine("logging");
            Logger.Log("testCommand","testAction");

        }

        private void ScatterViewItem_ScatterManipulationDelta
            (object sender, ScatterManipulationDeltaEventArgs e)
        {
            // Get a reference to the ScatterViewItem item.
            ScatterViewItem item = (ScatterViewItem)e.Source;

            //Console.WriteLine("width: " + item.ActualWidth);
            //Console.WriteLine("heigth: " + item.ActualHeight);

            if (item.ActualWidth <= 100)
            {
                Control_Minimized(this, new MinimizeEventArgs(((DeviceControl)item.Content).device, MinimizeEventType.Minimized, item.Center));
                item.Width = 156;
                item.Height = 309;
                e.Handled = true;
                return;
            }

            if (item.ActualWidth >= 600)
            {
                Maximize(item);
            }

            int dist = 10;
            int minX = 0 + dist;
            int minY = 0 + dist;
            int maxX = 1024 - dist;
            int maxY = 768 - dist;

            if (item.Center.X < minX || item.Center.X > maxX || item.Center.Y < minY || item.Center.Y > maxY)
            {
                Control_Minimized(this, new MinimizeEventArgs(((DeviceControl)item.Content).device, MinimizeEventType.Minimized, CalculateEdgePosition(item.Center)));
                e.Handled = true;
                return;
            }
            e.Handled = true;
        }

        private void AddSVI(MinimizedControl control)
        {
            ScatterViewItem svi = new ScatterViewItem();
            svi.Center = new System.Windows.Point(200,200);
            svi.Orientation = 0;
            svi.Height = 148;
            svi.Width = 75;
            svi.CanRotate = false;
            svi.CanScale = false;
            svi.Background = System.Windows.Media.Brushes.Transparent;
            svi.BorderBrush = System.Windows.Media.Brushes.Transparent;
            svi.SetResourceReference(StyleProperty, "ScatterViewItemStyleInvisible");
            svi.Content = control;
            svi.Visibility = Visibility.Hidden;
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

            svi.ContactDown += control._ContactDown;
            svi.ContactUp += control._ContactUp;
            svi.ContactChanged += control._ContactChanged;
            svi.ContactTapGesture += control._ContactTap;
            svi.ContactHoldGesture += control._ContactHold;
            
            svi.ScatterManipulationCompleted += ScatterViewItem_ScatterManipulationCompleted;
            svi.ScatterManipulationDelta += ScatterViewItem_ScatterManipulationDelta;

            svi.Content = control;
            MainSV.Items.Add(svi);
        }

        public void Maximize(ScatterViewItem item)
        {
            Maximize(item, 0);
        }

        public void Maximize(ScatterViewItem item, double rotation)
        {
            if (!((DeviceControl)item.Content).maximized)
            {
                ((DeviceControl)item.Content).Demaximized += Demaximize;
                ((DeviceControl)item.Content).maximized = true;
                ((DeviceControl)item.Content).addRotateButtons();
            }
            
            double o, oo = item.Orientation + rotation;
            if (oo < 0)
                o = 360 + oo;
            else if (oo >= 360)
                o = oo - 360;
            else
                o = oo;

            if (o < 45 || o > 315)
            {
                Console.WriteLine("1: " + o);
                item.Orientation = 0;
                item.Height = 1032;
                item.Width = 521;
                item.Center = new System.Windows.Point(512, 344);
            }
            else if (o >= 45 && o <= 135)
            {
                Console.WriteLine("2: " + o);
                item.Orientation = 90;
                item.Height = 1365;
                item.Width = 690;
                item.Center = new System.Windows.Point(562, 384);
            }
            else if (o > 135 && o < 225)
            {
                Console.WriteLine("3: " + o);
                item.Orientation = 180;
                item.Height = 1032;
                item.Width = 521;
                item.Center = new System.Windows.Point(512, 424);
            }
            else if (o >= 225 && o <= 315)
            {
                Console.WriteLine("4: " + o);
                item.Orientation = 270;
                item.Height = 1365;
                item.Width = 690;
                item.Center = new System.Windows.Point(462, 384);
            }

            item.CanMove = false;
            item.CanScale = false;
            item.CanRotate = false;
        }


        private void Demaximize(object sender, RoutedEventArgs e)
        {
            DeviceControl control = (DeviceControl)sender;
            control.Demaximized -= Demaximize;
            control.maximized = false;

            ScatterViewItem item = (ScatterViewItem)control.Parent;
            item.Height = 515;
            item.Width = 260;

            item.CanMove = true;
            item.CanScale = true;
            item.CanRotate = true;
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

        private int count = 0;
        void target_FrameReceived(object sender, FrameReceivedEventArgs e)
        {
            if (++count < 30)
            {
                return;
            }
            else
            {
                count = 0;
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
                //if (!tracker.TrackingDisabled)
                EnableRawImage();
            }
        }

        private void SurfaceWindow_Loaded(object sender, RoutedEventArgs e)
        {
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