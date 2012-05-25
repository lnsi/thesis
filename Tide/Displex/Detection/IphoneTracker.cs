using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Displex.Properties;

namespace Displex.Detection
{
     public class IPhoneTracker
     {
         public List<IPhone> FindIphoneDevices(Contour<Point> contours)
         {
            List<IPhone> FoundDevices = new List<IPhone>();

            if (contours == null)
                return null;

            ResetContoursNavigation(ref contours);

            CircleF apple, camera = new CircleF();

            for (; contours != null; contours = contours.HNext)
            {
                //Console.WriteLine("potential APPLE: {0}", contours.Area);
                // look for the Apple logo
                if (contours.Area >= Settings.Default.iPhoneAppleMin && contours.Area <= Settings.Default.iPhoneAppleMax)
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
                        if (contours.Area >= Settings.Default.iPhoneCameraMin && contours.Area <= Settings.Default.iPhoneCameraMax)
                        {
                            camera = new CircleF(
                                new PointF(contours.BoundingRectangle.Left + contours.BoundingRectangle.Width / 2,
                                    contours.BoundingRectangle.Top + contours.BoundingRectangle.Height / 2),
                                    contours.BoundingRectangle.Width / 2);

                            // check distance between apple and camera
                            double dist = Euclidean(apple.Center, camera.Center);
                            if (dist >= Settings.Default.iPhoneEuclideanDistanceMin && dist <= Settings.Default.iPhoneEuclideanDistanceMax)
                            {
                                FoundDevices.Add(new IPhone(apple, camera));
                                //Console.WriteLine("found iphone");
                            }
                        }
                        if (contours.HNext == null) break;
                    }
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

        private PointF ConvertCenter(PointF center)
        {
            return new PointF((center.X * 1024) / 768, (center.Y * 768) / 576);
        }
     }
}
