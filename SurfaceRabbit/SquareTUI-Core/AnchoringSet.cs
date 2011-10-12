using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SquareTUI_Core.Properties;

namespace SquareTUI_Core
{

  public class AnchoringSet
  {

    public double Angle { get; set; }
    public TUICircle Pivot { get; set; }
    public TUICircle RightAnchor { get; set; }
    public TUICircle BottomAnchor { get; set; }
    public PointF VerifyAnchor { get; set; }

    public double AngleRadians
    {
      get
      {
        return Angle * (Math.PI / 180.0);
      }
    }

    public PointF CalculatePoint(int row, int col, float axisLenght)
    {
      double stepXY = axisLenght / 7.0;
      double lenghtToRightAnchor = col * stepXY;
      double lenghtToBottonAnchor = row * stepXY;
      return CalculatePointFromLenghts(lenghtToRightAnchor, lenghtToBottonAnchor);
    }

    public PointF CalculateCenter(float axisLenght)
    {
      double lenghtToRightAnchor = axisLenght / 2;
      double lenghtToBottonAnchor = axisLenght / 2;
      return CalculatePointFromLenghts(lenghtToRightAnchor, lenghtToBottonAnchor);
    }

    private PointF CalculatePointFromLenghts(double lenghtToRightAnchor, double lenghtToBottonAnchor)
    {
      double alpha = 180 - Angle;
      double beta = 90 - alpha;

      double rX, bX;
      double rY, bY;

      rX = lenghtToRightAnchor * Math.Cos(TUICircle.DegreeToRadian(alpha));
      rY = lenghtToRightAnchor * Math.Sin(TUICircle.DegreeToRadian(alpha));

      bX = lenghtToBottonAnchor * Math.Cos(TUICircle.DegreeToRadian(beta));
      bY = lenghtToBottonAnchor * Math.Sin(TUICircle.DegreeToRadian(beta));

      double calculatedX = Pivot.Circle.Center.X + rX - bX;
      double calculatedY = Pivot.Circle.Center.Y - rY - bY;

      return new PointF((float)calculatedX, (float)calculatedY);
    }

  }

}
