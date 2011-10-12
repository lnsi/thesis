using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquareTUI_Core;
using System.Windows;

namespace SurfaceRabbit.Tracking
{

  public delegate void RabbitButtonPressed(object sender, RabbitButtonEventArgs e);
  public delegate void RabbitButtonReleased(object sender, RabbitButtonEventArgs e);

  public delegate void RabbitObjectEntered(object sender, RabbitEventArgs e);
  public delegate void RabbitObjectLeft(object sender, RabbitEventArgs e);

  public class RabbitEventArgs : EventArgs
  {

    public Rabbit Rabbit { get; private set; }
    public SquareTUI ObjectCode { get; private set; }
    public RabbitEventType EventType { get; private set; }

    public RabbitEventArgs(Rabbit r, SquareTUI oC, RabbitEventType rET)
    {
      Rabbit = r;
      ObjectCode = oC;
      EventType = rET;
    }

  }

  public class RabbitObjectRoutedEventArgs : RoutedEventArgs
  {
    public Rabbit Rabbit { get; private set; }
    public SquareTUI ObjectCode { get; private set; }
    public RabbitEventType EventType { get; private set; }

    public RabbitObjectRoutedEventArgs(RoutedEvent rEvent, Rabbit r, SquareTUI oC, RabbitEventType rET)
    {
      this.RoutedEvent = rEvent;
      Rabbit = r;
      ObjectCode = oC;
      EventType = rET;
    }
  }

  public enum RabbitEventType { ObjectEntered, ObjectLeft };

  public class RabbitButtonEventArgs : EventArgs
  {

    public RabbitButtonType ButtonType { get; private set; }
    public RabbitButtonEventType EventType { get; private set; }

    public RabbitButtonEventArgs(RabbitButtonType rBT, RabbitButtonEventType rBEA)
    {
      ButtonType = rBT;
      EventType = rBEA;
    }

  }

  public class RabbitButtonRoutedEventArgs : RoutedEventArgs
  {
    public Rabbit Rabbit { get; private set; }
    public RabbitButtonType ButtonType { get; private set; }
    public RabbitButtonEventType EventType { get; private set; }

    public RabbitButtonRoutedEventArgs(RoutedEvent rEvent, Rabbit r, RabbitButtonType rBT, RabbitButtonEventType rBEA)
    {
      this.RoutedEvent = rEvent;
      Rabbit = r;
      ButtonType = rBT;
      EventType = rBEA;
    }
  }

  public enum RabbitButtonType { ButtonA, ButtonB };
  public enum RabbitButtonEventType { Pressed, Released };

}
