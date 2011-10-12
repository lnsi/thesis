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

namespace RabbitTestApp.Controls
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

    public String BindingHelpX
    {
      get { return "X"; }
      set { }
    }
    public String BindingHelpY
    {
      get { return "Y"; }
      set { }
    }

    public bool IsBound
    {
      get
      {
        if (DataContext == null)
          return false;
        return true;
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
        OnPropertyChanged("IsBound");
    }

  }

}
