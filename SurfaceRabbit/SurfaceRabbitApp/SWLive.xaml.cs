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
using SurfaceRabbit.Tracking;
using SurfaceRabbitApp.Properties;
using Microsoft.Surface.Core;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing;

namespace JuanTestApp
{
  /// <summary>
  /// Interaction logic for SurfaceWindow.xaml
  /// </summary>
  public partial class SWLive : SurfaceWindow
  {

    public static SWLive Instance { get; private set; }

    private ContactTarget contactTarget;
    private IntPtr hwnd;
    private byte[] normalizedImage;
    private ImageMetrics imageMetrics;
    private ColorPalette pal;
    private bool imageAvailable;

    private RabbitEngine engine;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public SWLive()
    {
      InitializeComponent();

      // Add handlers for Application activation events
      AddActivationHandlers();
      // Add the handlers for the rabbit to be detected
      InitializeSurfaceInput();

      Instance = this;
      engine = new RabbitEngine(Settings.Default);
      engine.RabbitAdded += new RabbitAdded(engine_RabbitAdded);
      engine.RabbitRemoved += new RabbitRemoved(engine_RabbitRemoved);
      engine.RabbitUpdated += new RabbitUpdated(engine_RabbitUpdated);
      lRabbits.ItemsSource = engine.Rabbits;
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

    void engine_RabbitAdded(object sender, RabbitEngineEventArgs e)
    {
      Console.WriteLine("Rabbit Added - Id: {0}, Location: ({1},{2}), Angle: {3}, ObjectCode: {4}, ButtonA: {5}, ButtonB: {6}",
        e.Rabbit.RabbitCode, e.Rabbit.X, e.Rabbit.Y, e.Rabbit.AngleDegrees,
        e.Rabbit.ObjectCode.Value, e.Rabbit.ButtonA, e.Rabbit.ButtonB);
    }

    void engine_RabbitUpdated(object sender, RabbitEngineEventArgs e)
    {
    }

    void engine_RabbitRemoved(object sender, RabbitEngineEventArgs e)
    {
      Console.WriteLine("Rabbit Deleted - Id: {0}", e.Rabbit.RabbitCode);
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
      engine.ProcessImage(bitmap);

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

  }

}