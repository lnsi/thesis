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
    public class Legend : IDevice
    {

        #region IDevice Members

        private PointF deviceCenter;
        public System.Drawing.Point Center()
        {
            return System.Drawing.Point.Round(deviceCenter);
        }

        private double orientation = 0;
        public int Orientation()
        {
            return Convert.ToInt32(orientation);
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

        private string ip = "10.1.1.130";
        public string Ip()
        {
            return ip;
        }

        public bool IsSameDevice(IDevice device)
        {
            if (device == null) return false;

            if (Euclidean(Center(),device.Center()) < deltaCenter) 
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
            AdjustPosition();
            framesMissingNr = 0;
        }

        public DeviceControl Control { get; set; }

        #endregion

        // class members
        public const double deltaCenter = 30;

        private CircularList<PointF> centersAvg;
        private const int centersNr = 5;

        // Constructor
        public Legend(PointF center)
        {
            Control = new DeviceControl(this);
            SetSkin();

            framesMissingNr = 0;
            deviceCenter = center;

            centersAvg = new CircularList<PointF>(centersNr);
        }

        // Methods
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
        }

        // Return the distance between 2 points
        private double Euclidean(PointF p1, PointF p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        // call method that simulates a mouse right click
        public void backButton_Click(object sender, RoutedEventArgs e)
        {
            Control.rdfWPF.OnRightClick();
        }

        // call method that simulates a press on PageUp key
        public void menuButton_Click(object sender, RoutedEventArgs e)
        {
            Control.rdfWPF.PageUpKeyPress();
        }

        // call method that simulates a press on Home key
        public void homeButton_Click(object sender, RoutedEventArgs e)
        {
            Control.rdfWPF.HomeKeyPress();
        }

        private void SetSkin()
        {
            string packUri = "pack://application:,,,/Resources/androidBody.png";
            Control.MainBg.ImageSource = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;

            // MAIN GRID DEFINITION
            RowDefinition r = new RowDefinition();
            r.Height = new GridLength(0.1, GridUnitType.Star);
            RowDefinition r1 = new RowDefinition();
            r1.Height = new GridLength(0.7, GridUnitType.Star);
            RowDefinition r2 = new RowDefinition();
            r2.Height = new GridLength(0.2, GridUnitType.Star);
            Control.MainGrid.RowDefinitions.Add(r);
            Control.MainGrid.RowDefinitions.Add(r1);
            Control.MainGrid.RowDefinitions.Add(r2);

            ColumnDefinition c = new ColumnDefinition();
            c.Width = new GridLength(0.09, GridUnitType.Star);
            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(0.82, GridUnitType.Star);
            ColumnDefinition c2 = new ColumnDefinition();
            c2.Width = new GridLength(0.09, GridUnitType.Star);
            Control.MainGrid.ColumnDefinitions.Add(c);
            Control.MainGrid.ColumnDefinitions.Add(c1);
            Control.MainGrid.ColumnDefinitions.Add(c2);

            Control.rdfWPF.SetValue(Grid.ColumnProperty, 1);
            Control.rdfWPF.SetValue(Grid.RowProperty, 1);

            Control.Footer.SetValue(Grid.ColumnProperty, 1);
            Control.Footer.SetValue(Grid.RowProperty, 2);

            // FOOTER GRID DEFINITION
            RowDefinition rf = new RowDefinition();
            rf.Height = new GridLength(0.3, GridUnitType.Star);
            RowDefinition rf1 = new RowDefinition();
            rf1.Height = new GridLength(0.7, GridUnitType.Star);
            Control.Footer.RowDefinitions.Add(rf);
            Control.Footer.RowDefinitions.Add(rf1);

            ColumnDefinition cf = new ColumnDefinition();
            cf.Width = new GridLength(0.25, GridUnitType.Star);
            ColumnDefinition cf1 = new ColumnDefinition();
            cf1.Width = new GridLength(0.25, GridUnitType.Star);
            ColumnDefinition cf2 = new ColumnDefinition();
            cf2.Width = new GridLength(0.25, GridUnitType.Star);
            ColumnDefinition cf3 = new ColumnDefinition();
            cf3.Width = new GridLength(0.25, GridUnitType.Star);
            Control.Footer.ColumnDefinitions.Add(cf);
            Control.Footer.ColumnDefinitions.Add(cf1);
            Control.Footer.ColumnDefinitions.Add(cf2);
            Control.Footer.ColumnDefinitions.Add(cf3);

            // BACK BUTTON
            SurfaceButton backButton = new SurfaceButton();
            backButton.Click += new RoutedEventHandler(backButton_Click);
            backButton.Background = System.Windows.Media.Brushes.Transparent;
            backButton.VerticalAlignment = VerticalAlignment.Stretch;
            backButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            backButton.SetResourceReference(FrameworkElement.StyleProperty, "SurfaceButtonStyleInv");
            Control.Footer.Children.Add(backButton);
            backButton.SetValue(Grid.ColumnProperty, 0);
            backButton.SetValue(Grid.RowProperty, 0);

            // MENU BUTTON
            SurfaceButton menuButton = new SurfaceButton();
            menuButton.Click += new RoutedEventHandler(menuButton_Click);
            menuButton.Background = System.Windows.Media.Brushes.Transparent;
            menuButton.VerticalAlignment = VerticalAlignment.Stretch;
            menuButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            menuButton.SetResourceReference(FrameworkElement.StyleProperty, "SurfaceButtonStyleInv");
            Control.Footer.Children.Add(menuButton);
            menuButton.SetValue(Grid.ColumnProperty, 1);
            menuButton.SetValue(Grid.RowProperty, 0);

            // HOME BUTTON
            SurfaceButton homeButton = new SurfaceButton();
            homeButton.Click += new RoutedEventHandler(homeButton_Click);
            homeButton.Background = System.Windows.Media.Brushes.Transparent;
            homeButton.VerticalAlignment = VerticalAlignment.Stretch;
            homeButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            homeButton.SetResourceReference(FrameworkElement.StyleProperty, "SurfaceButtonStyleInv");
            Control.Footer.Children.Add(homeButton);
            homeButton.SetValue(Grid.ColumnProperty, 2);
            homeButton.SetValue(Grid.RowProperty, 0);

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