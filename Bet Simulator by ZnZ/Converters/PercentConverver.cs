﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Bet_Simulator_by_ZnZ.Converters
{
    public class PercentConverver : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return double.Parse(
                           value.ToString().Replace("..", ".").Replace(",,", ",").Replace("--", "-").TrimEnd('%'),
                           CultureInfo.InvariantCulture) / 100;
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
