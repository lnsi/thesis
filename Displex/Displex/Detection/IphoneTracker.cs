using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Displex.Detection
{
     public class IphoneTracker
     {
         public List<Iphone> FindIphones(Contour<Point> contours)
         {
            List<Iphone> FoundiPhones = new List<Iphone>();

            if (contours == null)
                return null;

            ResetContoursNavigation(ref contours);

            CircleF apple, camera = new CircleF();

            for (; contours != null; contours = contours.HNext)
            {
                //Console.WriteLine("potential APPLE: {0}", contours.Area);
                // look for the Apple logo
                if (contours.Area >= 200 && contours.Area <= 250)
                {
                    apple = new CircleF(
                      new PointF(contours.BoundingRectangle.Left + contours.BoundingRectangle.Width / 2,
                        contours.BoundingRectangle.Top + contours.BoundingRectangle.Height / 2),
                        contours.BoundingRectangle.Width / 2);

                    ResetContoursNavigation(ref contours);

                    for (; contours != null; contours = contours.HNext)
                    {
                        //Console.WriteLine("potential camera: {0}", contours.Area);
                        // look for the camera lens
                        if (contours.Area >= 20 && contours.Area <= 35)
                        {
                            camera = new CircleF(
                                new PointF(contours.BoundingRectangle.Left + contours.BoundingRectangle.Width / 2,
                                    contours.BoundingRectangle.Top + contours.BoundingRectangle.Height / 2),
                                    contours.BoundingRectangle.Width / 2);

                            // check distance between apple and camera
                            double dist = Euclidean(apple.Center, camera.Center);
                            if (dist >= 35 && dist <= 40)
                            {
                                FoundiPhones.Add(new Iphone(apple, camera));
                                Console.WriteLine("found iphone");
                            }
                        }
                        if (contours.HNext == null) break;
                    }
                }
                if (contours.HNext == null) break;
            }
            return FoundiPhones;
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
