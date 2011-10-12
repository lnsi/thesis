using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUIO.SquareTUI;

namespace RabbitTestApp.Entities
{

  public class LabObject : SquareTuioObject
  {

    public bool OnSurface { get; set; }

    public LabObject() : base(0, 0, 0, 0, 0, 0)
    {
    }

  }

}
