using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace SurfaceRabbit.Controls.Converters
{
  class RabbitBorderConverter : IValueConverter
  {

    private SolidColorBrush bBlue = new SolidColorBrush(Colors.Blue);
    private SolidColorBrush bRed = new SolidColorBrush(Colors.Red);

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == DependencyProperty.UnsetValue)
        return bBlue;

      bool isBound = (bool)value;
      if (isBound)
        return bRed;
      return bBlue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
