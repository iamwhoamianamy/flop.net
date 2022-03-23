using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace flop.net.View
{
   public class MultiplyConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return AsDouble(value) * AsDouble(parameter);
      }
      double AsDouble(object value)
      {
         var valueText = value as string;
         if (valueText != null)
            return double.Parse(valueText);
         else
            return (double)value;
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         throw new NotSupportedException();
      }
   }
}
