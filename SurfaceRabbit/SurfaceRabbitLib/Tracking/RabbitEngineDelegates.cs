using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquareTUI_Core;

namespace SurfaceRabbit.Tracking
{

  public delegate void RabbitAdded(object sender, RabbitEngineEventArgs e);
  public delegate void RabbitUpdated(object sender, RabbitEngineEventArgs e);
  public delegate void RabbitRemoved(object sender, RabbitEngineEventArgs e);

  public class RabbitEngineEventArgs : EventArgs
  {

    public Rabbit Rabbit { get; set; }
    public RabbitEngineEventType EventType { get; set; }

    public RabbitEngineEventArgs(Rabbit r, RabbitEngineEventType rEA)
    {
      Rabbit = r;
      EventType = rEA;
    }

  }

  public enum RabbitEngineEventType 
  {
    Added, 
    Updated, 
    Removed
  }

}
