using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV.Structure;
using System.Drawing;
using SquareTUI_Core.Properties;

namespace SquareTUI_Core
{

  public class TUICircle
  {

    public CircleF Circle { get; set; }
    public bool Grouped { get; set; }
    public bool IsPivot { get; set; }

    public TUICircle()
    {
    }

    private PointF GetPointInCircunference(PointF center, int angle, double axisLength)
    {
      double x = axisLength * Math.Cos(DegreeToRadian(angle)) + center.X;
      double y = axisLength * Math.Sin(DegreeToRadian(angle)) + center.Y;

      return new PointF((float)x, (float)y);
    }

    internal static double DegreeToRadian(double angle)
    {
      return Math.PI * angle / 180.0;
    }

    internal static double GetDistance(PointF point1, PointF point2)
    {
      //pythagoras theorem c^2 = a^2 + b^2
      //thus c = square root(a^2 + b^2)
      double a = (double)(point2.X - point1.X);
      double b = (double)(point2.Y - point1.Y);

      return Math.Sqrt(a * a + b * b);
    }

    public AnchoringSet FindAnchoringSet(IList<TUICircle> circles, float axisLenght, float locationThreshold)
    {
      float minimunLength = axisLenght - locationThreshold;
      float maximunLenght = axisLenght + locationThreshold;
      double verifyLength = Math.Sqrt(2 * Math.Pow(axisLenght, 2));

      foreach (TUICircle circle in circles)
      {
        //if (this == circle || circle.Grouped)
        if (this == circle)
          continue;

        double distance = GetDistance(this.Circle.Center, circle.Circle.Center);
        if (distance < minimunLength || distance > maximunLenght)
          continue;

        //this circle is a the expected distance, so we assume it's the RightAnchor
        // so we try to find the bottom anchor
        TUICircle rightAnchor = circle;

        double angle = GetAngle(this.Circle.Center, rightAnchor.Circle.Center);
        PointF centerBottomAnchor = GetPointInCircunference(this.Circle.Center, (int)(angle - 90), axisLenght);
        TUICircle bottomAnchor = GetCircleAt(centerBottomAnchor, circles, locationThreshold);
        if (bottomAnchor == null)
          continue;

        PointF centerVerifyAnchor = GetPointInCircunference(this.Circle.Center, (int)(angle + 45), verifyLength);
        TUICircle verifyAnchor = GetCircleAt(centerVerifyAnchor, circles, locationThreshold);
        if (verifyAnchor != null)
          continue;

        this.IsPivot = true;
        this.Grouped = true;
        rightAnchor.Grouped = true;
        bottomAnchor.Grouped = true;

        AnchoringSet set = new AnchoringSet();
        set.Angle = angle - 180;
        set.Pivot = this;
        set.RightAnchor = rightAnchor;
        set.BottomAnchor = bottomAnchor;
        set.VerifyAnchor = centerVerifyAnchor;
        return set;
      }

      return null;
    }

    private TUICircle GetCircleAt(PointF targetPoint, IList<TUICircle> circles, float LOCATION_THRESHOLD)
    {
      float minimunX = targetPoint.X - LOCATION_THRESHOLD;
      float maximunX = targetPoint.X + LOCATION_THRESHOLD;
      float minimunY = targetPoint.Y - LOCATION_THRESHOLD;
      float maximunY = targetPoint.Y + LOCATION_THRESHOLD;

      foreach (TUICircle circle in circles)
      {
        //if (this == circle || circle.Grouped)
        if (this == circle)
          continue;

        if (circle.Circle.Center.X < minimunX || circle.Circle.Center.X > maximunX)
          continue;
        if (circle.Circle.Center.Y < minimunY || circle.Circle.Center.Y > maximunY)
          continue;

        return circle;
      }
      return null;
    }

    public static double GetAngle(PointF pointCenter, PointF pointTarget)
    {
      // Negate X and Y values
      double pxRes = pointTarget.X - pointCenter.X;
      double pyRes = pointTarget.Y - pointCenter.Y;
      double angle = 0.0;

      // Calculate the angle
      if (pxRes == 0.0)
      {
        if (pyRes == 0.0)
          angle = 0.0;
        else if (pyRes > 0.0) 
          angle = System.Math.PI / 2.0;
        else
          angle = System.Math.PI * 3.0 / 2.0;
      }
      else if (pyRes == 0.0)
      {
        if (pxRes > 0.0)
          angle = 0.0;
        else
          angle = System.Math.PI;
      }
      else
      {
        if (pxRes < 0.0)
          angle = System.Math.Atan(pyRes / pxRes) + System.Math.PI;
        else if (pyRes < 0.0) 
          angle = System.Math.Atan(pyRes / pxRes) + (2 * System.Math.PI);
        else
          angle = System.Math.Atan(pyRes / pxRes);
      }

      // Convert to degrees
      angle = angle * 180 / System.Math.PI; 
      return angle;
    }

    public override string ToString()
    {
      return String.Format("Center: {0}, IsPivot: {1}, Grouped: {2}", Circle.Center, IsPivot, Grouped);
    }

  }

}
