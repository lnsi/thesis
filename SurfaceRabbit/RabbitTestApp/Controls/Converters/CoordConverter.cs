using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using TUIO;

namespace RabbitTestApp.Controls.Converters
{

  class CoordConverter : IMultiValueConverter
  {

    public CoordConverter()
    { }

    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (values == null || values.Length != 2)
        return 0;
      if (values[0] == DependencyProperty.UnsetValue)
        return 0;

      List<TuioPoint> path = (List<TuioPoint>)values[0];
      TuioPoint currentPos = path.Last();

      double posX = currentPos.getScreenX((int)Surface.Instance.ActualWidth);
      double posY = currentPos.getScreenY((int)Surface.Instance.ActualHeight);
      return (String)values[1] == "X" ? posX : posY;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

  }

}

