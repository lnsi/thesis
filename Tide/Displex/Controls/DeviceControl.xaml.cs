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
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Displex.Detection;
using Microsoft.Surface.Presentation.Input;

namespace Displex.Controls
{
  /// <summary>
  /// Interaction logic for DeviceControl.xaml
  /// </summary>
  public partial class DeviceControl : UserControl
  {

    public virtual event DeviceRemovedEvent Disconnected;
    public virtual event ControlMinimizedEvent Minimized;
    public virtual event RoutedEventHandler Demaximized;

    public bool maximized;

    public IDevice device { get; set; }

    public DeviceControl(IDevice device)
    {
      this.device = device;
      InitializeComponent();
      TouchDown += _TouchDown;
      TouchUp += _TouchUp;
      TouchMove += _TouchMoved;
    }

    public void Connect()
    {
      Connect(device.Ip());
    }

    public void Connect(String ip)
    {
      rdfWPF.Connect(ip);
    }

    private DateTime lastTapTime = DateTime.Now;

    public void _TouchDown(object sender, TouchEventArgs e)
    {
      Console.WriteLine();
      if (!e.Device.GetIsFingerRecognized())
      {
        if (e.Device.GetBounds(this).Width > 15 && e.Device.GetBounds(this).Height > 15)
        {
          Minimized(this, new MinimizeEventArgs(device, MinimizeEventType.Minimized, e.Device.GetPosition(this)));
          Logger.Log("minimize", "blob touch");
          e.Handled = true;
        }
        return;
      }

      if (IsMetaContact(e))
        return;

      Point touchPoint = MapPosition(e.GetTouchPoint(rdfWPF.ImageRDF));
      rdfWPF.ContactDown(touchPoint);
      e.Handled = true;
    }

    public void _TouchUp(object sender, TouchEventArgs e)
    {
      if (!e.Device.GetIsFingerRecognized())
        return;
      if (IsMetaContact(e))
        return;

      Point touchPoint = MapPosition(e.GetTouchPoint(rdfWPF.ImageRDF));
      rdfWPF.ContactUp(touchPoint);

      e.Handled = true;
    }

    public void _TouchMoved(object sender, TouchEventArgs e)
    {
      if (!e.Device.GetIsFingerRecognized())
        return;
      if (IsMetaContact(e))
        return;

      Point touchPoint = MapPosition(e.GetTouchPoint(rdfWPF.ImageRDF));
      rdfWPF.ContactChange(touchPoint);

      e.Handled = true;
    }

    private bool IsMetaContact(TouchEventArgs e)
    {
      ScatterViewItem parentSVI = this.Parent as ScatterViewItem;

      if (e.Device.GetCenterPosition(rdfWPF.ImageRDF).X >= 0
          && e.Device.GetCenterPosition(rdfWPF.ImageRDF).X <= rdfWPF.ImageRDF.ActualWidth
          && e.Device.GetCenterPosition(rdfWPF.ImageRDF).Y >= 0
          && e.Device.GetCenterPosition(rdfWPF.ImageRDF).Y <= rdfWPF.ImageRDF.ActualHeight)
      {
        parentSVI.CanMove = false;
        parentSVI.CanScale = false;
        parentSVI.CanRotate = false;
        return false;
      }
      else
      {
        parentSVI.CanMove = true;
        parentSVI.CanScale = true;
        parentSVI.CanRotate = true;
        return true;
      }
    }

    private Point MapPosition(TouchPoint currentPosition)
    {
      //Console.WriteLine("WIDTH: " + rdfWPF.ImageRDF.ActualWidth);
      //Console.WriteLine("HEIGHT: " + rdfWPF.ImageRDF.ActualHeight);

      return new Point(
          (currentPosition.Position.X * rdfWPF.serverWidth) / rdfWPF.ImageRDF.ActualWidth,
          (currentPosition.Position.Y * rdfWPF.serverHeight) / rdfWPF.ImageRDF.ActualHeight);
    }

    private SurfaceButton leftButton, rightButton;

    public void addRotateButtons()
    {
      string packUri = "pack://application:,,,/Resources/rotateLeft.png";
      Image img = new Image();
      img.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
      leftButton = new SurfaceButton();
      leftButton.Content = img;
      leftButton.Click += new RoutedEventHandler(rotateLeft_Click);
      leftButton.Background = System.Windows.Media.Brushes.Transparent;
      leftButton.BorderBrush = System.Windows.Media.Brushes.Transparent;
      leftButton.Padding = new Thickness(0);
      leftButton.Margin = new Thickness(3, 0, 0, 0);
      leftButton.VerticalAlignment = VerticalAlignment.Center;
      leftButton.HorizontalAlignment = HorizontalAlignment.Stretch;
      leftButton.SetResourceReference(FrameworkElement.StyleProperty, "SurfaceButtonStyleInv");
      MainGrid.Children.Add(leftButton);
      leftButton.SetValue(Grid.ColumnProperty, 0);
      leftButton.SetValue(Grid.RowProperty, 1);

      string packUri2 = "pack://application:,,,/Resources/rotateRight.png";
      Image img2 = new Image();
      img2.Source = new ImageSourceConverter().ConvertFromString(packUri2) as ImageSource;
      rightButton = new SurfaceButton();
      rightButton.Content = img2;
      rightButton.Click += new RoutedEventHandler(rotateRight_Click);
      rightButton.Background = System.Windows.Media.Brushes.Transparent;
      rightButton.BorderBrush = System.Windows.Media.Brushes.Transparent;
      rightButton.Padding = new Thickness(0);
      rightButton.Margin = new Thickness(0, 0, 3, 0);
      rightButton.VerticalAlignment = VerticalAlignment.Center;
      rightButton.HorizontalAlignment = HorizontalAlignment.Stretch;
      rightButton.SetResourceReference(FrameworkElement.StyleProperty, "SurfaceButtonStyleInv");
      MainGrid.Children.Add(rightButton);
      rightButton.SetValue(Grid.ColumnProperty, 2);
      rightButton.SetValue(Grid.RowProperty, 1);
    }

    public void removeRotateButtons()
    {
      MainGrid.Children.Remove(leftButton);
      MainGrid.Children.Remove(rightButton);
      MainGrid.UpdateLayout();
    }

    public void rotateLeft_Click(object sender, RoutedEventArgs e)
    {
      ((Displex.MainWindow)Application.Current.MainWindow).Maximize((ScatterViewItem)this.Parent, 90);
    }

    public void rotateRight_Click(object sender, RoutedEventArgs e)
    {
      ((Displex.MainWindow)Application.Current.MainWindow).Maximize((ScatterViewItem)this.Parent, -90);
    }

  }

}
