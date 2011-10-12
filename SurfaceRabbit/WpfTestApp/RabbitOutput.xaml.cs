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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using WpfTestApp.Properties;
using SurfaceRabbit;
using System.ComponentModel;
using Emgu.CV;
using Emgu.CV.Structure;
using SurfaceRabbit.Tracking;

namespace WpfTestApp
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class RabbitOutput : Window, INotifyPropertyChanged
  {

    public static RabbitOutput Instance { get; private set; }

    private int currentImageIndex = -1;
    private IList<Bitmap> capturedImages;
    private String[] files;

    private bool anyImageProcessed = false;
    private bool anyImageAvailable = false;

    public bool AnyImageProcessed {
      get { return anyImageProcessed; }
      private set 
      {
        anyImageProcessed = value;
        OnPropertyChanged("AnyImageProcessed");
      }
    }
    public bool AnyImageAvailable {
      get { return anyImageAvailable; }
      private set 
      {
        anyImageAvailable = value;
        OnPropertyChanged("AnyImageAvailable");
      }
    }

    private RabbitEngine engine;

    public RabbitOutput()
    {
      InitializeComponent();

      Instance = this;

      AnyImageProcessed = false;
      AnyImageAvailable = false;
      capturedImages = new List<Bitmap>();

      engine = new RabbitEngine(Settings.Default);
      engine.RabbitAdded += new RabbitAdded(engine_RabbitAdded);
      engine.RabbitRemoved += new RabbitRemoved(engine_RabbitRemoved);
      engine.RabbitUpdated += new RabbitUpdated(engine_RabbitUpdated);

      lRabbits.ItemsSource = engine.Rabbits;
    }

    void engine_RabbitAdded(object sender, RabbitEngineEventArgs e)
    {
      Console.WriteLine("Rabbit Added - Id: {0}, Location: ({1},{2}), Angle: {3}, ObjectCode: {4}, ButtonA: {5}, ButtonB: {6}",
        e.Rabbit.RabbitCode, e.Rabbit.X, e.Rabbit.Y, e.Rabbit.AngleDegrees,
        e.Rabbit.ObjectCode.Value, e.Rabbit.ButtonA, e.Rabbit.ButtonB);
    }

    void engine_RabbitUpdated(object sender, RabbitEngineEventArgs e)
    {
      Console.WriteLine("Rabbit Updated - Id: {0}, Location: ({1},{2}), Angle: {3}, ObjectCode: {4}, ButtonA: {5}, ButtonB: {6}",
        e.Rabbit.RabbitCode, e.Rabbit.X, e.Rabbit.Y, e.Rabbit.AngleDegrees, 
        e.Rabbit.ObjectCode.Value, e.Rabbit.ButtonA, e.Rabbit.ButtonB);
    }

    void engine_RabbitRemoved(object sender, RabbitEngineEventArgs e)
    {
      Console.WriteLine("Rabbit Deleted - Id: {0}", e.Rabbit.RabbitCode);
    }

    private void rabbitOutput_Loaded(object sender, RoutedEventArgs e)
    {
      if (!Directory.Exists(Settings.Default.PhotosPath))
        return;

      files = Directory.GetFiles(Settings.Default.PhotosPath, "*.bmp");
      if (files == null || files.Count() == 0)
        return;

      foreach (String file in files)
        capturedImages.Add((Bitmap)Bitmap.FromFile(file));
      AnyImageAvailable = true;
    }

    private void bNextImage_Click(object sender, RoutedEventArgs e)
    {
      currentImageIndex = (currentImageIndex + 1) % capturedImages.Count;

      Console.WriteLine("Processing File: {0}", files[currentImageIndex]);
      engine.ProcessImage(capturedImages[currentImageIndex]);
      SetThresholdImage();

      AnyImageProcessed = true;
    }

    private void bReprocess_Click(object sender, RoutedEventArgs e)
    {
      if (currentImageIndex == -1)
        return;

      Console.WriteLine("Processing File: {0}", files[currentImageIndex]);
      engine.ProcessImage(capturedImages[currentImageIndex]);
      SetThresholdImage();
    }

    private void SetThresholdImage()
    {
      Image<Gray, Byte> image = new Image<Gray, byte>(capturedImages[currentImageIndex]);
      image = image.ThresholdBinary(new Gray(Settings.Default.pContourThreshold), new Gray(400));
      iThresholdImage.Source = Bitmap2BitmapImage(image.Bitmap);
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

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(String pName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(pName));
    }

    #endregion

    #region Helper Properties
    /// <summary>
    /// The CoordConverter is used for both X and , so this property is to help CoordConverter to determine which conversion to return.
    /// </summary>
    public String BindingHelpX
    {
      get { return "X"; }
      set { }
    }

    /// <summary>
    /// The CoordConverter is used for both X and , so this property is to help CoordConverter to determine which conversion to return.
    /// </summary>
    public String BindingHelpY
    {
      get { return "Y"; }
      set { }
    }
    #endregion

    private void RabbitShadow_RabbitButton(object sender, RoutedEventArgs e)
    {
      RabbitButtonRoutedEventArgs eArgs = (RabbitButtonRoutedEventArgs)e;
      if(eArgs.EventType == RabbitButtonEventType.Pressed)
        Console.WriteLine("Rabbit {0} - {1} has been pressed", eArgs.Rabbit.RabbitCode, eArgs.ButtonType);
      else if (eArgs.EventType == RabbitButtonEventType.Released)
        Console.WriteLine("Rabbit {0} - {1} has been released", eArgs.Rabbit.RabbitCode, eArgs.ButtonType);
    }

    private void RabbitShadow_RabbitObject(object sender, RoutedEventArgs e)
    {
      RabbitObjectRoutedEventArgs eArgs = (RabbitObjectRoutedEventArgs)e;
      if (eArgs.EventType == RabbitEventType.ObjectEntered)
        Console.WriteLine("Rabbit {0} - Object Entered: {1}", eArgs.Rabbit.RabbitCode, eArgs.ObjectCode.Value);
      else if(eArgs.EventType == RabbitEventType.ObjectLeft)
        Console.WriteLine("Rabbit {0} - Object Left: {1}", eArgs.Rabbit.RabbitCode, eArgs.ObjectCode.Value);
    }

  }

}
