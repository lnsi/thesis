using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.ObjectModel;

namespace Displex
{
    class iPhoneTracker
    {       
        private SurfaceWindow1 mainWindow;
        private ObservableCollection<iPhone> potentialiPhones;
        private ObservableCollection<iPhone> currentiPhones;
        private ColorPalette pal;
        private bool isConnected;
        private int counter = 0;

        public iPhoneTracker(SurfaceWindow1 window)
        {
            mainWindow = window;
        }
        
        public void ProcessImage(Bitmap bitmap)
        {
            Convert8bppBMPToGrayscale(bitmap);
            PerformDetection(new Image<Gray, byte>(bitmap));     
        }

        private void Convert8bppBMPToGrayscale(Bitmap bmp)
        {
            if (pal == null) // pal is defined at module level as --- ColorPalette pal;
            {
                pal = bmp.Palette;
                for (int i = 0; i < 256; i++)
                {
                    pal.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
                }
            }

            bmp.Palette = pal;
        }

        private void PerformDetection(Image<Gray, Byte> gray)
        {
            gray = gray.ThresholdBinary(new Gray(10), new Gray(400));

            Contour<System.Drawing.Point> contours = gray.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
              Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST);
            //ListCircleAreas(contours);
            DetectDevice(contours);
        }

        private void ListCircleAreas(Contour<Point> contours)
        {
            if (contours == null)
                return;
            ResetContoursNavigation(ref contours);
            int count = 0;
            for (; contours != null; contours = contours.HNext)
            {
                Console.WriteLine("area " + (++count) + ": " + contours.Area.ToString());
                if (contours.HNext == null) break;
            }
            Console.WriteLine();
        }

        private void DetectDevice(Contour<Point> contours)
        {
            if (contours == null)
                return;

            ResetContoursNavigation(ref contours);

            CircleF apple, camera = new CircleF();

            for (; contours != null; contours = contours.HNext)
            {
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
                        // look for the camera lens
                        if (contours.Area >= 20 && contours.Area <= 40)
                        {
                            camera = new CircleF(
                                new PointF(contours.BoundingRectangle.Left + contours.BoundingRectangle.Width / 2,
                                    contours.BoundingRectangle.Top + contours.BoundingRectangle.Height / 2),
                                    contours.BoundingRectangle.Width / 2);

                            // check distance between apple and camera
                            double dist = Euclidean(apple.Center,camera.Center);
                            if (dist >= 35 && dist <= 40)
                            {
                                TrackDevice(new iPhone(apple, camera));
                            }
                        }
                        if (contours.HNext == null) break;
                    }
                }
                if (contours.HNext == null) break;
            }
        }

        private void RegisteriPhone(CircleF apple, CircleF camera)
        {
            iPhone current = new iPhone(apple, camera);
            if (current.IsInList(currentiPhones))
            {
                // iPhoneUpdated()
            }
            else if (current.IsInList(potentialiPhones))
            {
                counter++;
                if (counter >= 5)
                {
                    currentiPhones.Add(current); // iPhoneAdded()
                    potentialiPhones.Clear();
                    counter = 0;
                }
                else
                {
                    potentialiPhones.Add(current);
                }
            }
            else
            {
                potentialiPhones.Clear();
                counter = 0;
            }
        }      

        private void TrackDevice(iPhone device)
        {
            Console.WriteLine("hypothenuse: " + device.Hypothenuse);
            Console.WriteLine("orientation: " + device.Orientation);
            
            // connect only once
            //if (!isConnected)
            //{
            //    Console.WriteLine("attempting connection..");
            //    mainWindow.Connect("10.1.1.198");
            //    isConnected = true;
            //}
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
