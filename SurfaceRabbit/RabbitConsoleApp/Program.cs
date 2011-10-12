using System;
using TUIO.SquareTUI;
using TUIO;


namespace RabbitConsoleApp
{

  public class TuioDump : TuioListener, SquareTuioListener
  {

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

    public static void Main(String[] argv)
    {
      TuioDump demo = new TuioDump();
      SquareTuioClient client = null;

      switch (argv.Length)
      {
        case 1:
          int port = 0;
          port = int.Parse(argv[0], null);
          if (port > 0) client = new SquareTuioClient(port);
          break;
        case 0:
          client = new SquareTuioClient(3333);
          break;
      }

      if (client != null)
      {
        client.addTuioListener(demo);
        client.addSquareTuioListener(demo);
        client.connect();
        Console.WriteLine("listening to TUIO messages at port " + client.getPort());

      }
      else Console.WriteLine("usage: java TuioDump [port]");
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

  }

}
