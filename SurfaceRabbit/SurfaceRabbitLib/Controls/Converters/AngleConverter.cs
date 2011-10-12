using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace SurfaceRabbit.Controls.Converters
{

  public class AngleConverter : IMultiValueConverter
  {

    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if(values.Length != 2)
        return (double)0;
      if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
        return (double)0;

      float actualAngle = float.Parse(values[0].ToString());
      float angleMultiplier = float.Parse(values[1].ToString());

      return (double)(actualAngle * angleMultiplier);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

  }

}
