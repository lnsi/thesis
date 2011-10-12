using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using TUIO;

namespace RabbitTestApp.Controls.Converters
{

  class MarginConverter : IMultiValueConverter
  {

    public MarginConverter()
    { }

    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (values == null || values.Length != 3)
        return new Thickness(0, 0, 0, 0);
      if (values[0] == DependencyProperty.UnsetValue)
        return new Thickness(0, 0, 0, 0);

      List<TuioPoint> path = (List<TuioPoint>)values[0];
      TuioPoint currentPos = path.Last();

      return new Thickness(currentPos.getScreenX((int)Surface.Instance.ActualWidth), currentPos.getScreenY((int)Surface.Instance.ActualHeight), 0, 0);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

  }

}

