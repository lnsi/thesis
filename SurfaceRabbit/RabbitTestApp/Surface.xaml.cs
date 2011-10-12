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
using TUIO.SquareTUI;
using TUIO;
using RabbitTestApp.Properties;
using System.Collections.ObjectModel;
using RabbitTestApp.Entities;

namespace RabbitTestApp
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class Surface : Window, TuioListener, SquareTuioListener
  {

    private SquareTuioClient client;
    private ThreadSafeObservableCollection<TuioCursor> cursors;
    private ThreadSafeSquareTuioListener tsListener;

    public static Surface Instance
    { get; private set; }

    public Surface()
    {
      InitializeComponent();
      Instance = this;

      tsListener = new ThreadSafeSquareTuioListener(this, Dispatcher);
      cursors = new ThreadSafeObservableCollection<TuioCursor>();
      lCursors.ItemsSource = cursors;
    }

    private void bConnect_Click(object sender, RoutedEventArgs e)
    {
      if (!client.isConnected())
      {
        client.connect();
        bConnect.Content = "Disconnect TUIO";
      }
      else
      {
        client.disconnect();
        bConnect.Content = "Listen TUIO";
      }
    }

    #region TuioListener Members

    public void refresh(TuioTime ftime)
    {
    }

    public void addTuioCursor(TuioCursor tcur)
    {
      Console.WriteLine("add cur " + tcur.CursorID + " (" + tcur.SessionID + ") " + tcur.X + " " + tcur.Y);
      if (cursors.FirstOrDefault<TuioCursor>(tmp => tmp.CursorID == tcur.CursorID) == null)
        cursors.Add(tcur);
    }

    public void removeTuioCursor(TuioCursor tcur)
    {
      Console.WriteLine("del cur " + tcur.CursorID + " (" + tcur.SessionID + ")");
      TuioCursor cursor = cursors.FirstOrDefault<TuioCursor>(tmp => tmp.CursorID == tcur.CursorID);
      if (cursors != null)
        cursors.Remove(cursor);
    }

    public void updateTuioCursor(TuioCursor tcur)
    {
      Console.WriteLine("set cur " + tcur.CursorID + " (" + tcur.SessionID + ") " + tcur.X + " " + tcur.Y + " " + tcur.MotionSpeed + " " + tcur.MotionAccel);
      TuioCursor cursor = cursors.FirstOrDefault<TuioCursor>(tmp => tmp.CursorID == tcur.CursorID);
      if (cursor != null)
        cursor.update(tcur);
    }

    public void addTuioObject(TuioObject tobj)
    {
      Console.WriteLine("add obj " + tobj.SymbolID + " " + tobj.SessionID + " " + tobj.X + " " + tobj.Y + " " + tobj.Angle);
    }

    public void removeTuioObject(TuioObject tobj)
    {
      Console.WriteLine("del obj " + tobj.SymbolID + " " + tobj.SessionID);
    }

    public void updateTuioObject(TuioObject tobj)
    {
      Console.WriteLine("set obj " + tobj.SymbolID + " " + tobj.SessionID + " " + tobj.X + " " + tobj.Y + " " + tobj.Angle + " " + tobj.MotionSpeed + " " + tobj.RotationSpeed + " " + tobj.MotionAccel + " " + tobj.RotationAccel);
    }

    #endregion

    #region SquareTuioListener Members

    public void addSquareTuio(SquareTuioObject squareTui)
    {
      Console.WriteLine("add squareTui " + squareTui.SymbolID + " " + squareTui.SessionID + " " + squareTui.X + " " + squareTui.Y + " " + squareTui.Angle);
      if (rabbit.DataContext == null)
        rabbit.DataContext = squareTui;
    }

    public void buttonPressed(SquareTuioObject squareTui, SquareTUIButton button)
    {
      Console.WriteLine("button pressed " + squareTui.SymbolID + " " + squareTui.SessionID + " " + button);
    }

    public void buttonReleased(SquareTuioObject squareTui, SquareTUIButton button)
    {
      Console.WriteLine("button released " + squareTui.SymbolID + " " + squareTui.SessionID + " " + button);
    }

    public void removeSquareTuio(SquareTuioObject squareTui)
    {
      Console.WriteLine("del squareTui " + squareTui.SymbolID + " " + squareTui.SessionID);
      if (rabbit.DataContext != null)
      {
        SquareTuioObject rabbitObj = (SquareTuioObject)rabbit.DataContext;
        if (rabbitObj.SessionID == squareTui.SessionID)
          rabbit.DataContext = null;
      }
    }

    public void updateSquareTuio(SquareTuioObject squareTui)
    {
      Console.WriteLine("set squareTui " + squareTui.SymbolID + " " + squareTui.SessionID + " " + squareTui.X + " " + squareTui.Y + " " + squareTui.Angle + " " + squareTui.MotionSpeed + " " + squareTui.RotationSpeed + " " + squareTui.MotionAccel + " " + squareTui.RotationAccel);
    }

    #endregion

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (client.isConnected())
        client.disconnect();
    }

    private void wSurface_Loaded(object sender, RoutedEventArgs e)
    {
      client = new SquareTuioClient(Settings.Default.TUIO_PORT);
      client.addTuioListener(this);
      client.addSquareTuioListener(tsListener);
      bConnect_Click(null, null);
      Console.WriteLine("listening to TUIO messages at port " + client.getPort());
    }

  }

}
