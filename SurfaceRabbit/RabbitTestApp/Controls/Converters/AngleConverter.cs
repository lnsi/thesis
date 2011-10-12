using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using RabbitTestApp.Properties;

namespace RabbitTestApp.Controls.Converters
{

  class AngleConverter : IValueConverter
  {

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == DependencyProperty.UnsetValue)
        return 0;

      float actualAngle = (float)value;
      return actualAngle * Settings.Default.AngleMultiplier;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

  }

}
