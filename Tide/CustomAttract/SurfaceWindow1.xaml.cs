using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Windows;
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

namespace CustomAttract
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
        private ColorPalette pal;
        private bool imageAvailable;

        private float minArea = 200;
        private float maxArea = 250;

        private bool isApple = false;

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
            EnableRawImage();
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
            if (isApple)
                return;

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
            Convert8bppBMPToGrayscale(bitmap);
            ProcessImage(bitmap);

            imageAvailable = false;
            EnableRawImage();
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

        public void ProcessImage(Bitmap cImage)
        {
            PerformDetection(new Image<Gray, byte>(cImage));
        }

        private void PerformDetection(Image<Gray, Byte> gray)
        {
            gray = gray.ThresholdBinary(new Gray(10), new Gray(400));

            Contour<System.Drawing.Point> contours = gray.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
              Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST);
            //ListCircleAreas(contours);
            DetectDevice(contours);
        }

        private void ListCircleAreas(Contour<Point> contours)
        {
            if (contours == null)
                return;
            ResetContoursNavigation(ref contours);
            int count = 0;
            for (; contours != null; contours = contours.HNext)
            {
                Console.WriteLine("area "+ (++count) + ": " +contours.Area.ToString());
            }
            Console.WriteLine();
        }

        private void DetectDevice(Contour<Point> contours)
        {
            if (contours == null)
                return;

            ResetContoursNavigation(ref contours);

            CircleF apple;
            for (; contours != null; contours = contours.HNext)
            {
                if (contours.Area >= minArea && contours.Area <= maxArea)
                {
                    apple = new CircleF(
                      new PointF(contours.BoundingRectangle.Left + contours.BoundingRectangle.Width / 2,
                        contours.BoundingRectangle.Top + contours.BoundingRectangle.Height / 2),
                        contours.BoundingRectangle.Width / 2);
                    Console.WriteLine("Apple!");
                    Process.Start("C:\\LeoThesis\\thesis\\Displex\\Displex\\bin\\Release\\Displex.exe");
                    isApple = true;
                    break;
                }
            }
        }

        private static void ResetContoursNavigation(ref Contour<Point> contours)
        {
            if (contours == null)
                return;

            //go back to the begining
            while (contours.HPrev != null)
                contours = contours.HPrev;
        }

    }
}