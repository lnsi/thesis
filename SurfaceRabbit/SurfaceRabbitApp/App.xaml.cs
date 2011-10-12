using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace JuanTestApp
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    private void Application_Startup(object sender, StartupEventArgs e)
    {
      if (e.Args.Length == 0 || e.Args[0] == "-l")
        StartupUri = new Uri("SWLive.xaml", UriKind.Relative);
      else
        StartupUri = new Uri("SWCapture.xaml", UriKind.Relative);
    }
  }

}