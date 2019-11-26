using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Bet_Simulator_by_ZnZ.Converters
{
    public class ChanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return 9500 / double.Parse(value.ToString()) / 10000;
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return 95 / double.Parse(value.ToString().Replace("..", ".").Replace(",,", ",").TrimEnd('%'),
                           CultureInfo.InvariantCulture);
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
