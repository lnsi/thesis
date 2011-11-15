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
        private CircleF apple, camera;
        public CircleF Apple
        {
            get { return apple; }
            set { apple = value; } 
        }
        public CircleF Camera 
        {
            get { return camera; }
            set { camera = value; }
        }

        private double Alpha = 46.1;
        public double Hypothenuse;
        public double delta = 10;

        public iPhone(CircleF apple, CircleF camera)
        {
            this.Apple = apple;
            this.Camera = camera;
            this.Hypothenuse = Euclidean(Apple.Center, Camera.Center);
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
            get { return Convert.ToInt32(this.OrientationDouble()); }
        }

        private double OrientationDouble()
        {
            if (Apple.Center.Equals(Camera.Center))
            {
                throw new Exception("detection not valid: points are identical");
            }
            // the axis Apple-Camera is located exactly along a reference line
            if (Apple.Center.X == Camera.Center.X)
            {
                if (Apple.Center.Y > Camera.Center.Y) return (360 - Alpha);
                if (Apple.Center.Y < Camera.Center.Y) return (180 - Alpha);
            }
            if (Apple.Center.Y == Camera.Center.Y)
            {
                if (Apple.Center.X < Camera.Center.X) return (90 - Alpha);
                if (Apple.Center.X > Camera.Center.X) return (270 - Alpha);
            }
            // located somewhere in the half-circle 0 to 180 degrees
            if (Apple.Center.X < Camera.Center.X)
            {
                // located in quadrant 0-90 degrees
                if (Apple.Center.Y > Camera.Center.Y)
                {
                    double AngleToRefZero = CalculateAngleToReference(new PointF(Apple.Center.X, Camera.Center.Y));
                    if (AngleToRefZero < 0)
                        return AngleToRefZero + 360;
                    else
                        return AngleToRefZero;
                }
                // located in quadrant 90-180 degrees
                if (Apple.Center.Y < Camera.Center.Y)
                {
                    return CalculateAngleToReference(new PointF(Camera.Center.X, Apple.Center.Y)) + 90;
                }
            }
            // located somewhere in the half-circle 180 to 360 degrees
            if (Apple.Center.X > Camera.Center.X)
            {
                // located in quadrant 180-270 degrees
                if (Apple.Center.Y < Camera.Center.Y)
                {
                    return CalculateAngleToReference(new PointF(Apple.Center.X, Camera.Center.Y)) + 180;
                }
                // located in quadrant 270-360 degrees
                if (Apple.Center.Y > Camera.Center.Y)
                {
                    return CalculateAngleToReference(new PointF(Camera.Center.X, Apple.Center.Y)) + 270;
                }
            }
            return 0;
        }

        private double CalculateAngleToReference(PointF Cpoint)
        {
            double Adjacent = Euclidean(Apple.Center, Cpoint);
            double Beta = ToDegree(Math.Acos(Adjacent / Hypothenuse));
            Console.WriteLine("Beta: " + Beta);
            return Beta - Alpha;
        }

        /// <summary>
        /// Return the distance between 2 points
        /// </summary>
        private double Euclidean(PointF p1, PointF p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        private double ToDegree(double radianAngle)
        {
            return radianAngle * (180.0 / Math.PI);
        }
    }
}
