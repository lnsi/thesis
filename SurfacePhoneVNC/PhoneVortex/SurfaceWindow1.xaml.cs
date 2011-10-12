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
using PhoneVortex.Properties;

namespace PhoneVortex
{
  /// <summary>
  /// Interaction logic for SurfaceWindow1.xaml
  /// </summary>
  public partial class SurfaceWindow1 : SurfaceWindow
  {

    private Dictionary<byte, String> vncAddress = new Dictionary<byte, String>();

    /// <summary>
    /// Default constructor.
    /// </summary>
    public SurfaceWindow1()
    {
      InitializeComponent();
      InitializeDefinitions();
      // Add handlers for Application activation events
      AddActivationHandlers();
    }

    private void InitializeDefinitions()
    {
      ByteTagVisualizationDefinition def = new ByteTagVisualizationDefinition();
      def.Value = 0xCF;
      def.Source = new Uri("PhoneVortexVisualization.xaml", UriKind.Relative);
      def.MaxCount = 1;
      def.LostTagTimeout = 500;
      def.OrientationOffsetFromTag = -92;
      def.PhysicalCenterOffsetFromTag = new Vector(3.95, 3.35);
      def.TagRemovedBehavior = TagRemovedBehavior.Fade;
      def.UsesTagOrientation = true;
      tVisualizer.Definitions.Add(def);

      vncAddress.Add(0xCF, Settings.Default.PhoneIP);
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
    }

    private void tVisualizer_VisualizationAdded(object sender, TagVisualizerEventArgs e)
    {
      String vncIp = vncAddress[e.TagVisualization.VisualizedTag.Byte.Value];
      (e.TagVisualization as PhoneVortexVisualization).VNCIP = vncIp;
      (e.TagVisualization as PhoneVortexVisualization).Connect();
    }

    private void tVisualizer_VisualizationRemoved(object sender, TagVisualizerEventArgs e)
    {
      (e.TagVisualization as PhoneVortexVisualization).Disconnect();
    }
  }
}