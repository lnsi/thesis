using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace SquareTUI_Core
{

  public class SquareTUI : INotifyPropertyChanged
  {

    public event PropertyChangedEventHandler PropertyChanged;

    private Int32 value;

    private bool stateBitA = false;
    private bool stateBitB = false;
    private bool stateBitC = false;
    private bool stateBitD = false;
    private bool stateBitE = false;
    private bool stateBitF = false;
    private bool stateBitG = false;
    private bool stateBitH = false;

    private AnchoringSet anchor;
    private bool[,] matrixTUI;

    public SquareTUIType Type { get; set; }
    public float AxisLenght { get; set; }
    public float LocationThreshold { get; set; }

    public SquareTUI()
    {
      matrixTUI = new bool[8, 8];
      //Adds the anchoring points
      matrixTUI[0, 0] = true;
      matrixTUI[0, 7] = true;
      matrixTUI[7, 0] = true;
    }

    protected void OnPropertyChanged(String pName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(pName));
    }

    public Int32 Value
    {
      get { return this.value; }
    }

    public bool StateBitA
    {
      get { return stateBitA; }
      private set
      {
        bool oldValue = stateBitA;
        stateBitA = value;
        if (oldValue != value)
          OnPropertyChanged("StateBitA");
      }
    }

    public bool StateBitB
    {
      get { return stateBitB; }
      private set
      {
        bool oldValue = stateBitB;
        stateBitB = value;
        if (oldValue != value)
          OnPropertyChanged("StateBitB");
      }
    }

    public bool StateBitC
    {
      get { return stateBitC; }
      private set
      {
        bool oldValue = stateBitC;
        stateBitC = value;
        if (oldValue != value)
          OnPropertyChanged("StateBitC");
      }
    }

    public bool StateBitD
    {
      get { return stateBitD; }
      private set
      {
        bool oldValue = stateBitD;
        stateBitD = value;
        if (oldValue != value)
          OnPropertyChanged("StateBitD");
      }
    }

    public bool StateBitE
    {
      get { return stateBitE; }
      private set
      {
        bool oldValue = stateBitE;
        stateBitE = value;
        if (oldValue != value)
          OnPropertyChanged("StateBitE");
      }
    }

    public bool StateBitF
    {
      get { return stateBitF; }
      private set
      {
        bool oldValue = stateBitF;
        stateBitF = value;
        if (oldValue != value)
          OnPropertyChanged("StateBitF");
      }
    }

    public bool StateBitG
    {
      get { return stateBitG; }
      private set
      {
        bool oldValue = stateBitG;
        stateBitG = value;
        if (oldValue != value)
          OnPropertyChanged("StateBitG");
      }
    }

    public bool StateBitH
    {
      get { return stateBitH; }
      private set
      {
        bool oldValue = stateBitH;
        stateBitH = value;
        if (oldValue != value)
          OnPropertyChanged("StateBitH");
      }
    }

    public AnchoringSet Anchor
    {
      get { return anchor; }
      set { anchor = value; }
    }

    public bool[,] MatrixTUI
    {
      get { return matrixTUI; }
    }

    public PointF[] MatrixPoints
    {
      get
      {
        IList<PointF> matrixPoints = new List<PointF>();
        for (int row = 0; row < 8; row++)
        {
          for (int col = 0; col < 8; col++)
          {
            if (matrixTUI[row, col])
              matrixPoints.Add(Anchor.CalculatePoint(row, col, AxisLenght));
          }
        }
        return matrixPoints.ToArray();
      }
    }

    public void CalculateValues()
    {
      Int32 mask = 1;

      for (int row = 0; row < 2; row++)
      {
        for (int col = 2; col < 6; col++)
        {
          if (matrixTUI[row, col])
            value = value | mask;
          mask <<= 1;
        }
      }

      for (int row = 2; row < 5; row++)
      {
        for (int col = 0; col < 8; col++)
        {
          if (matrixTUI[row, col])
            value = value | mask;
          mask <<= 1;
        }
      }

      if (matrixTUI[6, 2])
        stateBitA = true;
      if (matrixTUI[6, 3])
        stateBitB = true;
      if (matrixTUI[6, 4])
        stateBitC = true;
      if (matrixTUI[6, 5])
        stateBitD = true;
      if (matrixTUI[7, 2])
        stateBitE = true;
      if (matrixTUI[7, 3])
        stateBitF = true;
      if (matrixTUI[7, 4])
        stateBitG = true;
      if (matrixTUI[7, 5])
        stateBitH = true;
    }

    public override string ToString()
    {
      return String.Format("{0}-{1}{2}{3}{4}{5}{6}{7}{8}", value,
        stateBitA ? "A" : "",
        stateBitB ? "B" : "",
        stateBitC ? "C" : "",
        stateBitD ? "D" : "",
        stateBitE ? "E" : "",
        stateBitF ? "F" : "",
        stateBitG ? "G" : "",
        stateBitH ? "H" : "");
    }

    public float GetDistance(float x2, float y2)
    {
      //pythagoras theorem c^2 = a^2 + b^2
      //thus c = square root(a^2 + b^2)
      float x1 = Anchor.Pivot.Circle.Center.X;
      float y1 = Anchor.Pivot.Circle.Center.Y;

      double a = (double)(x2 - x1);
      double b = (double)(y2 - y1);

      return (float)Math.Sqrt(a * a + b * b);
    }

    public float GetDistance(SquareTUI foundTUI)
    {
      return GetDistance(foundTUI.Anchor.Pivot.Circle.Center.X, foundTUI.Anchor.Pivot.Circle.Center.Y);
    }

    public PointF CalculateCenter()
    {
      return Anchor.CalculateCenter(AxisLenght);
    }
  }

}
