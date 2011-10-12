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

namespace SurfaceRabbit.Controls
{
  /// <summary>
  /// Interaction logic for ObjectInfoBox.xaml
  /// </summary>
  public partial class ObjectInfoBox : UserControl
  {

    #region Routed Events
    public static readonly RoutedEvent CloseBoxEvent = EventManager.RegisterRoutedEvent("CloseBox", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ObjectInfoBox));

    public event RoutedEventHandler CloseBox
    {
      add { AddHandler(ObjectInfoBox.CloseBoxEvent, value); }
      remove { RemoveHandler(ObjectInfoBox.CloseBoxEvent, value); }
    }
    #endregion

    public ObjectInfoBox()
    {
      InitializeComponent();
    }

    private void SurfaceButton_Click(object sender, RoutedEventArgs e)
    {
      RaiseEvent(new RoutedEventArgs(ObjectInfoBox.CloseBoxEvent, this));
    }

  }

}
