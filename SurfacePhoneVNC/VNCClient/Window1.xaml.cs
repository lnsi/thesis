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
using VncSharp;

namespace VNCClient
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class Window1 : Window
  {

    private WriteableBitmap wBitmap;

    public Window1()
    {
      InitializeComponent();
      //wBitmap = new WriteableBitmap((int)ActualWidth, (int)ActualHeight, 96, 96, PixelFormats.Bgra32, null);
      //iWITest.Source = wBitmap;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      rdfWPF.ConnectComplete += new ConnectCompleteHandler(rdf_ConnectComplete);
      rdfWPF.VncPort = 5900;
      rdfWPF.Connect("10.27.227.244");
    }

    void rdf_ConnectComplete(object sender, ConnectEventArgs e)
    {
      this.Width = e.DesktopWidth + 16;
      this.Height = e.DesktopHeight + 36;
    }

  }
}
