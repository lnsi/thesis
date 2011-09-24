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
using VncSharp;

namespace PhoneVortex
{
  /// <summary>
  /// Interaction logic for PhoneVortexVisualization.xaml
  /// </summary>
  public partial class PhoneVortexVisualization : TagVisualization
  {

    public String VNCIP { get; set; }

    public PhoneVortexVisualization()
    {
      InitializeComponent();
      InitializeRDF();
    }

    private void InitializeRDF()
    {
      rdfWPF.ConnectComplete += new ConnectCompleteHandler(rdf_ConnectComplete);
      rdfWPF.VncPort = 5900;
    }

    internal void Connect()
    {
      rdfWPF.Connect(VNCIP);
    }

    void rdf_ConnectComplete(object sender, ConnectEventArgs e)
    {
      //Fix the visualization size
    }

    internal void Disconnect()
    {
      rdfWPF.Disconnect();
    }

    protected override void OnContactDown(ContactEventArgs e)
    {
      base.OnContactDown(e);
      if (!e.Contact.IsFingerRecognized)
        return;
      if (e.Contact.DirectlyOver != rdfWPF.ImageRDF)
        return;

      Point touchPoint = e.GetPosition(rdfWPF.ImageRDF);
      rdfWPF.ContactDown(touchPoint);
      Console.Write("ContactDown({0:00.00}, {1:00.00})", touchPoint.X, touchPoint.Y);
    }

    protected override void OnContactUp(ContactEventArgs e)
    {
      base.OnContactUp(e);
      if (!e.Contact.IsFingerRecognized)
        return;
      if (e.Contact.DirectlyOver != rdfWPF.ImageRDF)
        return;

      Point touchPoint = e.GetPosition(rdfWPF.ImageRDF);
      rdfWPF.ContactUp(touchPoint);
      Console.Write("ContactUp({0:00.00}, {1:00.00})\n", touchPoint.X, touchPoint.Y);
    }

    protected override void OnContactChanged(ContactEventArgs e)
    {
      base.OnContactChanged(e);
      if (!e.Contact.IsFingerRecognized)
        return;
      if (e.Contact.DirectlyOver != rdfWPF.ImageRDF)
        return;

      Point touchPoint = e.GetPosition(rdfWPF.ImageRDF);
      rdfWPF.ContactChange(touchPoint);
      Console.Write(".");
    }

  }

}
