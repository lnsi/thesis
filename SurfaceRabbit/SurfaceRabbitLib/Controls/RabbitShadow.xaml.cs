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
using System.ComponentModel;
using SurfaceRabbit.Tracking;

namespace SurfaceRabbit.Controls
{
  /// <summary>
  /// Interaction logic for RabbitShadow.xaml
  /// </summary>
  public partial class RabbitShadow : UserControl, INotifyPropertyChanged
  {

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(String pName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(pName));
    }

    #endregion

    #region Routed Events
    public static readonly RoutedEvent RabbitButtonEvent = EventManager.RegisterRoutedEvent("RabbitButton", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RabbitShadow));
    public static readonly RoutedEvent RabbitObjectEvent = EventManager.RegisterRoutedEvent("RabbitObject", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RabbitShadow));

    public event RoutedEventHandler RabbitButton
    {
      add { AddHandler(RabbitShadow.RabbitButtonEvent, value); }
      remove { RemoveHandler(RabbitShadow.RabbitButtonEvent, value); }
    }

    public event RoutedEventHandler RabbitObject
    {
      add { AddHandler(RabbitShadow.RabbitObjectEvent, value); }
      remove { RemoveHandler(RabbitShadow.RabbitObjectEvent, value); }
    }
    #endregion

    private bool showObjButtons = true;
    private bool showObjInfo = false;

    public bool IsBound
    {
      get
      {
        if (DataContext == null || (DataContext as Rabbit).ObjectCode.Value == 0)
          return false;
        return true;
      }
    }

    public bool ShowButtons
    {
      get
      {
        return IsBound && showObjButtons;
      }
    }

    public bool ShowObjectInfo
    {
      get
      {
        return ShowButtons && showObjInfo;
      }
    }

    public RabbitShadow()
    {
      InitializeComponent();
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);
      if (e.Property == RabbitShadow.DataContextProperty)
      {
        OnPropertyChanged("IsBound");
        OnPropertyChanged("ShowButtons");
        OnPropertyChanged("ShowObjectInfo");
        if (DataContext == null)
          return;

        Rabbit rabbit = (Rabbit)DataContext;
        rabbit.ButtonPressed += new RabbitButtonPressed(rabbit_ButtonPressed);
        rabbit.ButtonReleased += new RabbitButtonReleased(rabbit_ButtonReleased);
        rabbit.ObjectEntered += new RabbitObjectEntered(rabbit_ObjectEntered);
        rabbit.ObjectLeft += new RabbitObjectLeft(rabbit_ObjectLeft);
      }
    }

    void rabbit_ObjectLeft(object sender, RabbitEventArgs e)
    {
      OnPropertyChanged("IsBound");
      OnPropertyChanged("ShowButtons");
      OnPropertyChanged("ShowObjectInfo");
      RaiseEvent(new RabbitObjectRoutedEventArgs(RabbitShadow.RabbitObjectEvent, (Rabbit)DataContext, e.ObjectCode, RabbitEventType.ObjectLeft));
    }

    void rabbit_ObjectEntered(object sender, RabbitEventArgs e)
    {
      showObjButtons = true;
      showObjInfo = false;

      OnPropertyChanged("IsBound");
      OnPropertyChanged("ShowButtons");
      OnPropertyChanged("ShowObjectInfo");
      RaiseEvent(new RabbitObjectRoutedEventArgs(RabbitShadow.RabbitObjectEvent, (Rabbit)DataContext, e.ObjectCode, RabbitEventType.ObjectEntered));
    }

    void rabbit_ButtonReleased(object sender, RabbitButtonEventArgs e)
    {
      if (e.ButtonType == RabbitButtonType.ButtonA && IsBound)
      {
        showObjButtons = !showObjButtons;
        OnPropertyChanged("ShowButtons");
        OnPropertyChanged("ShowObjectInfo");
      }

      RaiseEvent(new RabbitButtonRoutedEventArgs(RabbitShadow.RabbitButtonEvent, (Rabbit)DataContext, e.ButtonType, e.EventType));
    }

    void rabbit_ButtonPressed(object sender, RabbitButtonEventArgs e)
    {
      RaiseEvent(new RabbitButtonRoutedEventArgs(RabbitShadow.RabbitButtonEvent, (Rabbit)DataContext, e.ButtonType, e.EventType));
    }

    private void sButton_Click(object sender, RoutedEventArgs e)
    {
      showObjInfo = !showObjInfo;
      OnPropertyChanged("ShowObjectInfo");
    }

    private void oibBox_CloseBox(object sender, RoutedEventArgs e)
    {
      showObjInfo = false;
      OnPropertyChanged("ShowObjectInfo");
    }

  }

}
