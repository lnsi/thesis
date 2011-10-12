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

namespace RabbitConsoleWPF
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class Window1 : Window, SquareTuioListener, TuioListener
  {

    public static Window1 Instance
    { get; private set; }

    public Window1()
    {
      InitializeComponent();
      Instance = this;
    }

    public void addTuioObject(TuioObject tobj)
    {
      Console.WriteLine("add obj " + tobj.SymbolID + " " + tobj.SessionID + " " + tobj.X + " " + tobj.Y + " " + tobj.Angle);
    }

    public void updateTuioObject(TuioObject tobj)
    {
      Console.WriteLine("set obj " + tobj.SymbolID + " " + tobj.SessionID + " " + tobj.X + " " + tobj.Y + " " + tobj.Angle + " " + tobj.MotionSpeed + " " + tobj.RotationSpeed + " " + tobj.MotionAccel + " " + tobj.RotationAccel);
    }

    public void removeTuioObject(TuioObject tobj)
    {
      Console.WriteLine("del obj " + tobj.SymbolID + " " + tobj.SessionID);
    }

    public void addTuioCursor(TuioCursor tcur)
    {
      Console.WriteLine("add cur " + tcur.CursorID + " (" + tcur.SessionID + ") " + tcur.X + " " + tcur.Y);
    }

    public void updateTuioCursor(TuioCursor tcur)
    {
      Console.WriteLine("set cur " + tcur.CursorID + " (" + tcur.SessionID + ") " + tcur.X + " " + tcur.Y + " " + tcur.MotionSpeed + " " + tcur.MotionAccel);
    }

    public void removeTuioCursor(TuioCursor tcur)
    {
      Console.WriteLine("del cur " + tcur.CursorID + " (" + tcur.SessionID + ")");
    }

    public void refresh(TuioTime frameTime)
    {
      //Console.WriteLine("refresh "+frameTime.getTotalMilliseconds());
    }

    public void addSquareTuio(SquareTuioObject squareTui)
    {
      Console.WriteLine("add squareTui " + squareTui.SymbolID + " " + squareTui.SessionID + " " + squareTui.X + " " + squareTui.Y + " " + squareTui.Angle);
    }

    public void updateSquareTuio(SquareTuioObject squareTui)
    {
      Console.WriteLine("set squareTui " + squareTui.SymbolID + " " + squareTui.SessionID + " " + squareTui.X + " " + squareTui.Y + " " + squareTui.Angle + " " + squareTui.MotionSpeed + " " + squareTui.RotationSpeed + " " + squareTui.MotionAccel + " " + squareTui.RotationAccel);
    }

    public void removeSquareTuio(SquareTuioObject squareTui)
    {
      Console.WriteLine("del squareTui " + squareTui.SymbolID + " " + squareTui.SessionID);
    }

    public void buttonPressed(SquareTuioObject squareTui, SquareTUIButton button)
    {
      Console.WriteLine("button pressed " + squareTui.SymbolID + " " + squareTui.SessionID + " " + button);
    }

    public void buttonReleased(SquareTuioObject squareTui, SquareTUIButton button)
    {
      Console.WriteLine("button released " + squareTui.SymbolID + " " + squareTui.SessionID + " " + button);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      SquareTuioClient client = null;
      client = new SquareTuioClient(3333);
      client.addTuioListener(this);
      client.addSquareTuioListener(this);
      client.connect();
      Console.WriteLine("listening to TUIO messages at port " + client.getPort());
    }

  }

}
