using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV.Structure;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Displex
{
    class iPhone
    {
        // class members
        private CircleF apple, camera;
        public CircleF Apple
        {
            get { return apple; } 
        }
        public CircleF Camera 
        {
            get { return camera; }
        }
        private PointF iPhoneCenter;
        public PointF DeviceCenter
        {
            get { return iPhoneCenter; }
        }

        private float[] cameraStandCoord;
        private int quadrant;
        private double orientation;
        private double beta;
        public double hypothenuse;
        private double alpha = 0.805;

        //private double zeta;
        private double distFromAppleToCenter = 35;
        public double delta = 10;

        // Constructors
        public iPhone() { }

        public iPhone(CircleF apple, CircleF camera)
        {
            this.updatePosition(apple, camera);
        }

        // Methods
        public void updatePosition(CircleF apple, CircleF camera)
        {
            this.apple = apple;
            this.camera = camera;
            // converting the coordinates of the camera point to standard coordinates on a x,y plane
            // where the apple point is at the center (0,0)
            cameraStandCoord = new float[2] { (camera.Center.X - apple.Center.X), (apple.Center.Y - camera.Center.Y) };
            this.hypothenuse = Math.Sqrt(Math.Pow(cameraStandCoord[0], 2) + Math.Pow(cameraStandCoord[1], 2));
            Console.WriteLine("hypothenuse: " + hypothenuse);
            CalculatePosition();
        }

        public bool IsInList(ObservableCollection<iPhone> list)
        {
            if (list == null) return false;
            foreach (iPhone device in list)
            {
                if (Euclidean(this.Apple.Center, device.Apple.Center) < delta
                    && Euclidean(this.Camera.Center, device.Camera.Center) < delta)
                {
                    return true;
                }
            }
            return false;
        }

        public int Orientation
        {
            get { return Convert.ToInt32(ToDegree(orientation)); }
        }

        private void CalculatePosition()
        {
            double cosTheta = cameraStandCoord[0] / hypothenuse;
            double sinTheta = cameraStandCoord[1] / hypothenuse;
            // the absolute value in radians of the angle between the AC axis and the X plane
            double theta = Math.Abs(Math.Asin(sinTheta));
            Console.WriteLine("theta: " + theta);

            // First quadrant
            if (cosTheta >= 0 && sinTheta >= 0)
            {
                quadrant = 1;
                beta = theta + alpha;
                if (beta > (Math.PI / 2))
                {
                    quadrant = 4;
                    beta = Math.PI - beta;
                }
            }
            // Second quadrant
            if (cosTheta >= 0 && sinTheta <= 0)
            {
                quadrant = 2;
                beta = theta - alpha;
                if (beta < 0)
                {
                    quadrant = 1;
                    beta = -beta;
                }

            }
            // Third quadrant
            if (cosTheta <= 0 && sinTheta <= 0)
            {
                quadrant = 3;
                beta = theta + alpha;
                if (beta > (Math.PI / 2))
                {
                    quadrant = 2;
                    beta = Math.PI - beta;
                }
            }
            // Fourth quadrant
            if (cosTheta <= 0 && sinTheta >= 0)
            {
                quadrant = 4;
                beta = theta - alpha;
                if (beta < 0)
                {
                    quadrant = 3;
                    beta = -beta;
                }
            }

            switch (quadrant)
            {
                case 1:
                    orientation = (Math.PI / 2) - beta;
                    break;
                case 2:
                    orientation = (Math.PI / 2) + beta;
                    break;
                case 3:
                    orientation = ((3 * Math.PI) / 2) - beta;
                    break;
                case 4:
                    orientation = ((3 * Math.PI) / 2) + beta;
                    break;
                default:
                    break;
            }

            double distX = Math.Cos(Math.PI - Math.Acos(cosTheta)) * distFromAppleToCenter;
            double distY = Math.Sin(Math.Asin(sinTheta)) * distFromAppleToCenter;
            iPhoneCenter = new PointF((apple.Center.X + (float)distX),(apple.Center.Y - (float)distY));
            Console.WriteLine("center x: " + iPhoneCenter.X + ", y: " + iPhoneCenter.Y);

        }

        //private double OrientationDouble()
        //{
        //    if (Apple.Center.Equals(Camera.Center))
        //    {
        //        throw new Exception("detection not valid: points are identical");
        //    }
        //    // the axis Apple-Camera is located exactly along a reference line
        //    if (Apple.Center.X == Camera.Center.X)
        //    {
        //        if (Apple.Center.Y > Camera.Center.Y) return (360 - alpha);
        //        if (Apple.Center.Y < Camera.Center.Y) return (180 - alpha);
        //    }
        //    if (Apple.Center.Y == Camera.Center.Y)
        //    {
        //        if (Apple.Center.X < Camera.Center.X) return (90 - alpha);
        //        if (Apple.Center.X > Camera.Center.X) return (270 - alpha);
        //    }
        //    // located somewhere in the half-circle 0 to 180 degrees
        //    if (Apple.Center.X < Camera.Center.X)
        //    {
        //        // located in quadrant 0-90 degrees
        //        if (Apple.Center.Y > Camera.Center.Y)
        //        {
        //            CalculateZeta(new PointF(Apple.Center.X, Camera.Center.Y));
        //            if (zeta < 0)
        //                return zeta + 360;
        //            else
        //                return zeta;
        //        }
        //        // located in quadrant 90-180 degrees
        //        if (Apple.Center.Y < Camera.Center.Y)
        //        {
        //            CalculateZeta(new PointF(Camera.Center.X, Apple.Center.Y));
        //            return zeta + 90;
        //        }
        //    }
        //    // located somewhere in the half-circle 180 to 360 degrees
        //    if (Apple.Center.X > Camera.Center.X)
        //    {
        //        // located in quadrant 180-270 degrees
        //        if (Apple.Center.Y < Camera.Center.Y)
        //        {
        //            CalculateZeta(new PointF(Apple.Center.X, Camera.Center.Y));
        //            return zeta + 180;
        //        }
        //        // located in quadrant 270-360 degrees
        //        if (Apple.Center.Y > Camera.Center.Y)
        //        {
        //            CalculateZeta(new PointF(Camera.Center.X, Apple.Center.Y));
        //            return zeta + 270;
        //        }
        //    }
        //    return 0;
        //}

        //private void CalculateZeta(PointF Cpoint)
        //{
        //    double Adjacent = Euclidean(Apple.Center, Cpoint);
        //    double beta = ToDegree(Math.Acos(Adjacent / hypothenuse));
        //    Console.WriteLine("Beta: " + beta);
        //    zeta = beta - alpha;
        //}

        // Return the distance between 2 points
        private double Euclidean(PointF p1, PointF p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        // Convert a radian angle to a degree angle
        private double ToDegree(double radianAngle)
        {
            return radianAngle * (180.0 / Math.PI);
        }

        //// Convert a degree angle to a radian angle
        //private double ToRadian(double degreeAngle)
        //{
        //    return degreeAngle * (Math.PI / 180.0);
        //}
    }
}
