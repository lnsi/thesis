using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV.Structure;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Surface.Presentation.Controls;

namespace Displex.Detection
{
    public class Iphone : IDevice
    {

        #region IDevice Members

        private PointF deviceCenter;
        public System.Drawing.Point Center()
        {
            return System.Drawing.Point.Round(deviceCenter);
        }

        private double orientation;
        public int Orientation()
        {
            return Convert.ToInt32(ToDegree(orientation));
        }

        private int width = 97;
        public int Width()
        {
            return width;
        }

        private int height = 193;
        public int Height()
        {
            return height;
        }

        private string ip = "10.1.1.198";
        public string Ip()
        {
            return ip;
        }

        public bool IsSameDevice(IDevice device)
        {
            if (device == null) return false;

            if (Euclidean(Center(),device.Center()) < deltaCenter 
                && Math.Abs(Orientation() - device.Orientation()) < deltaOrientation)
                return true;
            
            return false;
        }

        private int framesMissingNr;
        public bool CanBeRemoved()
        {
            if (++framesMissingNr >= 5)
                return true;
            else return false;
        }

        public void UpdatePosition()
        {       
            CalculatePosition();
            Console.WriteLine("Before adjust");
            Console.WriteLine("center: " + Center().X + "," + Center().Y);
            Console.WriteLine("orientation: " + Orientation());
            AdjustPosition();
            //Console.WriteLine("After adjust");
            //Console.WriteLine("center: " + Center().X + "," + Center().Y);
            //Console.WriteLine("orientation: " + Orientation());
            Console.WriteLine("**************************");
            framesMissingNr = 0;
        }

        public DeviceControl Control { get; set; }

        #endregion

        // class members
        public CircleF Apple { get; private set; }
        public CircleF Camera  { get; private set; }

        private const double alpha = 0.805;
        private const double distFromAppleToCenter = 15;
        public const double deltaCenter = 30;
        public const double deltaOrientation = 30;

        private float[] cameraStandCoord;
        private int quadrant;
        private double beta;
        public double hypothenuse;

        private CircularList<PointF> centersAvg;
        private CircularList<double> orientationsAvg;
        private const int centersNr = 5, orientationsNr = 5;

        // Constructor
        public Iphone(CircleF apple, CircleF camera)
        {
            Control = new DeviceControl(this);
            SetSkin();

            framesMissingNr = 0;
            Apple = apple;
            Camera = camera;
            CalculatePosition();

            centersAvg = new CircularList<PointF>(centersNr);
            orientationsAvg = new CircularList<double>(orientationsNr);
        }

        // Methods
        private void CalculatePosition()
        {
            // converting the coordinates of the camera point to standard coordinates on a x,y plane
            // where the apple center is at coord (0,0)
            cameraStandCoord = new float[2] { (Camera.Center.X - Apple.Center.X), (Apple.Center.Y - Camera.Center.Y) };
            this.hypothenuse = Math.Sqrt(Math.Pow(cameraStandCoord[0], 2) + Math.Pow(cameraStandCoord[1], 2));
            //Console.WriteLine("hypothenuse: " + hypothenuse);

            double cosTheta = cameraStandCoord[0] / hypothenuse;
            double sinTheta = cameraStandCoord[1] / hypothenuse;
            // the absolute value in radians of the angle between the AC axis and the X plane
            double theta = Math.Abs(Math.Asin(sinTheta));
            //Console.WriteLine("theta: " + theta);

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

            double distX=0,distY=0;

            switch (quadrant)
            {
                case 1:
                    orientation = (Math.PI / 2) - beta;
                    distX = Math.Cos(Math.PI - beta) * distFromAppleToCenter;
                    distY = Math.Sin(-beta) * distFromAppleToCenter;
                    break;
                case 2:
                    orientation = (Math.PI / 2) + beta;
                    distX = Math.Cos(Math.PI - beta) * distFromAppleToCenter;
                    distY = Math.Sin(beta) * distFromAppleToCenter;
                    break;
                case 3:
                    orientation = ((3 * Math.PI) / 2) - beta;
                    distX = Math.Cos(beta) * distFromAppleToCenter;
                    distY = Math.Sin(beta) * distFromAppleToCenter;
                    break;
                case 4:
                    orientation = ((3 * Math.PI) / 2) + beta;
                    distX = Math.Cos(beta) * distFromAppleToCenter;
                    distY = Math.Sin(-beta) * distFromAppleToCenter;
                    break;
                default:
                    break;
            }

            deviceCenter = new PointF((Apple.Center.X + (float)distX), (Apple.Center.Y - (float)distY));
            //Console.WriteLine("apple center x: " + Apple.Center.X + ", y: " + Apple.Center.Y);
            //Console.WriteLine("devicecenter x: " + iphoneCenter.X + ", y: " + iphoneCenter.Y);
        }

        public void AdjustPosition()
        {
            // register last reading and advances to next item in list
            centersAvg.Value = deviceCenter;
            centersAvg.Next();
            // calculate average
            float newX = 0, newY = 0;
            int centersCount = centersAvg.Count;
            for (int i = 0; i < centersCount; i++)
            {
                newX += centersAvg[i].X;
                newY += centersAvg[i].Y;
            }
            deviceCenter = new PointF(newX / centersCount, newY / centersCount);

            // register last reading and advances to next item in list
            orientationsAvg.Value = orientation;
            orientationsAvg.Next();
            // calculate average
            double newO = 0;
            int orientationsCount = orientationsAvg.Count;
            for (int i = 0; i < orientationsCount; i++)
            {
                newO += orientationsAvg[i];
            }
            orientation = newO / orientationsCount;
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

        private void SetSkin()
        {
            string packUri = "pack://application:,,,/Resources/iphoneBody.png";
            Control.MainBg.ImageSource = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;

            // MAIN GRID DEFINITION
            RowDefinition r = new RowDefinition();
            r.Height = new GridLength(0.15, GridUnitType.Star);
            RowDefinition r1 = new RowDefinition();
            r1.Height = new GridLength(0.65, GridUnitType.Star);
            RowDefinition r2 = new RowDefinition();
            r2.Height = new GridLength(0.2, GridUnitType.Star);
            Control.MainGrid.RowDefinitions.Add(r);
            Control.MainGrid.RowDefinitions.Add(r1);
            Control.MainGrid.RowDefinitions.Add(r2);

            ColumnDefinition c = new ColumnDefinition();
            c.Width = new GridLength(0.07, GridUnitType.Star);
            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(0.86, GridUnitType.Star);
            ColumnDefinition c2 = new ColumnDefinition();
            c2.Width = new GridLength(0.07, GridUnitType.Star);
            Control.MainGrid.ColumnDefinitions.Add(c);
            Control.MainGrid.ColumnDefinitions.Add(c1);
            Control.MainGrid.ColumnDefinitions.Add(c2);

            Control.rdfWPF.SetValue(Grid.ColumnProperty, 1);
            Control.rdfWPF.SetValue(Grid.RowProperty, 1);

            Control.Footer.SetValue(Grid.ColumnProperty, 1);
            Control.Footer.SetValue(Grid.RowProperty, 2);

            // FOOTER GRID DEFINITION
            RowDefinition rf = new RowDefinition();
            rf.Height = new GridLength(0.2, GridUnitType.Star);
            RowDefinition rf1 = new RowDefinition();
            rf1.Height = new GridLength(0.5, GridUnitType.Star);
            RowDefinition rf2 = new RowDefinition();
            rf2.Height = new GridLength(0.3, GridUnitType.Star);
            Control.Footer.RowDefinitions.Add(rf);
            Control.Footer.RowDefinitions.Add(rf1);
            Control.Footer.RowDefinitions.Add(rf2);

            ColumnDefinition cf = new ColumnDefinition();
            cf.Width = new GridLength(0.38, GridUnitType.Star);
            ColumnDefinition cf1 = new ColumnDefinition();
            cf1.Width = new GridLength(0.24, GridUnitType.Star);
            ColumnDefinition cf2 = new ColumnDefinition();
            cf2.Width = new GridLength(0.38, GridUnitType.Star);
            Control.Footer.ColumnDefinitions.Add(cf);
            Control.Footer.ColumnDefinitions.Add(cf1);
            Control.Footer.ColumnDefinitions.Add(cf2);

            // HOME BUTTON
            SurfaceButton homeButton = new SurfaceButton();
            homeButton.Click += new RoutedEventHandler(Control.homeButton_Click);
            homeButton.Background = System.Windows.Media.Brushes.Transparent;
            homeButton.BorderBrush = System.Windows.Media.Brushes.Transparent;
            homeButton.VerticalAlignment = VerticalAlignment.Stretch;
            homeButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            homeButton.SetResourceReference(FrameworkElement.StyleProperty, "SurfaceButtonStyleInv");
            Control.Footer.Children.Add(homeButton);
            homeButton.SetValue(Grid.ColumnProperty, 1);
            homeButton.SetValue(Grid.RowProperty, 1);
            
            // EXIT BUTTON
            SurfaceButton closeButton = new SurfaceButton();
            closeButton.Click += new RoutedEventHandler(Control.closeButton_Click);
            closeButton.Background = System.Windows.Media.Brushes.Red;
            closeButton.BorderBrush = System.Windows.Media.Brushes.Transparent;
            closeButton.VerticalAlignment = VerticalAlignment.Stretch;
            closeButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            closeButton.SetResourceReference(FrameworkElement.StyleProperty, "SurfaceButtonStyleInv");
            Control.MainGrid.Children.Add(closeButton);
            closeButton.SetValue(Grid.ColumnProperty, 2);
            closeButton.SetValue(Grid.RowProperty, 2);
        }
    }
}