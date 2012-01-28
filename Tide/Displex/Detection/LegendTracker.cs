using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Displex.Detection
{
     public class LegendTracker
     {
         public List<Legend> FindLegendDevices(Contour<Point> contours)
         {
             List<Legend> FoundDevices = new List<Legend>();

            if (contours == null)
                return null;

            ResetContoursNavigation(ref contours);

            for (; contours != null; contours = contours.HNext)
            {
                // look for the HTC Legend silver body (white rectangle)
                if (contours.Area >= 5400 && contours.Area <= 5600)
                {
                    //Console.WriteLine("legend area: " + contours.Area);
                    CircleF body = new CircleF(new PointF(contours.BoundingRectangle.Left + contours.BoundingRectangle.Width / 2,
                        contours.BoundingRectangle.Top + contours.BoundingRectangle.Height / 2), contours.BoundingRectangle.Width / 2);
                    FoundDevices.Add(new Legend(body.Center));
                    //Console.WriteLine("found legend");
                }
                if (contours.HNext == null) break;
            }
            return FoundDevices;
        }

        private void ResetContoursNavigation(ref Contour<Point> contours)
        {
            if (contours == null)
                return;

            //go back to the begining
            while (contours.HPrev != null)
                contours = contours.HPrev;
        }

        /// <summary>
        /// Return the distance between 2 points
        /// </summary>
        private double Euclidean(PointF p1, PointF p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
     }
}
