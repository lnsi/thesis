using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Core;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using SurfaceRabbit.Capture;
using SurfaceRabbitApp.Properties;

namespace JuanTestApp
{
  /// <summary>
  /// Interaction logic for SurfaceWindow1.xaml
  /// </summary>
  public partial class SWCapture : SurfaceWindow
  {

    private ContactTarget contactTarget;
    private IntPtr hwnd;
    private byte[] normalizedImage;
    private ImageMetrics imageMetrics;
    private ColorPalette pal;
    private bool imageAvailable;

    private VideoCaptureAviWriter captureAW;
    private Object videoLock = new Object();
    private bool videoCapturing = false;
    private int fileCounter = 0;
    private Bitmap frame = null;
    private int frameCounter = 0;
    private String currentVideoFileName;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public SWCapture()
    {
      InitializeComponent();

      // Add handlers for Application activation events
      AddActivationHandlers();
      InitializeSurfaceInput();
    }

    private void InitializeSurfaceInput()
    {
      hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
      contactTarget = new Microsoft.Surface.Core.ContactTarget(hwnd);
      contactTarget.EnableInput();
      EnableRawImage();
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
      contactTarget.EnableImage(ImageType.Normalized);
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
      contactTarget.DisableImage(ImageType.Normalized);
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

      ShowInWPF();
      CaptureVideo();

      imageAvailable = false;
      EnableRawImage();
    }

    private void ShowInWPF()
    {
      GCHandle h = GCHandle.Alloc(normalizedImage, GCHandleType.Pinned);
      IntPtr ptr = h.AddrOfPinnedObject();
      frame = new Bitmap(imageMetrics.Width,
                            imageMetrics.Height,
                            imageMetrics.Stride,
                            System.Drawing.Imaging.PixelFormat.Format8bppIndexed,
                            ptr);
      Convert8bppBMPToGrayscale(frame);
      iCapturedFrame.Source = Bitmap2BitmapImage(frame);
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

    private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
    {
      MemoryStream ms = new MemoryStream();
      bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
      BitmapImage bImg = new System.Windows.Media.Imaging.BitmapImage();
      bImg.BeginInit();
      bImg.StreamSource = new MemoryStream(ms.ToArray());
      bImg.EndInit();
      return bImg;
    }

    private void bCapture_Click(object sender, RoutedEventArgs e)
    {
      // Stop processing raw images so normalizedImage 
      // is not changed while it is saved to a file.
      DisableRawImage();

      // Copy the normalizedImage byte array into a Bitmap object.
      GCHandle h = GCHandle.Alloc(normalizedImage, GCHandleType.Pinned);
      IntPtr ptr = h.AddrOfPinnedObject();
      Bitmap imageBitmap = new Bitmap(imageMetrics.Width,
                            imageMetrics.Height,
                            imageMetrics.Stride,
                            System.Drawing.Imaging.PixelFormat.Format8bppIndexed,
                            ptr);

      // The preceding code converts the bitmap to an 8-bit indexed color image. 
      // The following code creates a grayscale palette for the bitmap.
      Convert8bppBMPToGrayscale(imageBitmap);

      // The bitmap is now available to work with 
      // (such as, save to a file, send to a processing API, and so on).
      imageBitmap.Save(GetFileName("bmp"), System.Drawing.Imaging.ImageFormat.Bmp);

      fileCounter++;
      imageAvailable = false;
      EnableRawImage();
    }

    private void bCaptureVideo_Click(object sender, RoutedEventArgs e)
    {
      lock (videoLock)
      {
        videoCapturing = !videoCapturing;
        if (videoCapturing)
        {
          // Creates the video capture object
          captureAW = new VideoCaptureAviWriter();

          frameCounter = 0;
          currentVideoFileName = GetFileName("avi");
          captureAW.Open(currentVideoFileName, 25);
        }
        else
        {
          bCaptureVideo.Content = String.Format("Capture Video (10s)");
          captureAW.Close();
          try { File.Delete(currentVideoFileName); }
          catch { }
        }
      }
    }

    private void CaptureVideo()
    {
      lock (videoLock)
      {
        if (!videoCapturing)
          return;
      }

      if (frame == null || !captureAW.Started)
        return;

      frameCounter++;
      bCaptureVideo.Content = String.Format("{0:F0}s (Click To Cancel)", (Settings.Default.TotalFrames - frameCounter)/25);
      captureAW.AddFrame(frame);

      lock (videoLock)
      {
        if (frameCounter != Settings.Default.TotalFrames)
          return;
        videoCapturing = false;
        bCaptureVideo.Content = String.Format("Capture Video (10s)");
        captureAW.Close();
      }
    }

    private String GetFileName(String extension)
    {
      String fileName = String.Format(@"C:\Users\pitlab\Desktop\temp-{0}.{1}", fileCounter, extension);
      while (File.Exists(fileName))
      {
        fileCounter++;
        fileName = String.Format(@"C:\Users\pitlab\Desktop\temp-{0}.{1}", fileCounter, extension);
      }
      return fileName;
    }

  }

}