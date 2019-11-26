using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bet_Simulator_by_ZnZ.Model
{
    public static class Model
    {
        public static double Fixed(double value, int places = 8)
        {
            int left = (int)Math.Truncate(value);
            var right = (value - left) * Math.Pow(10, places);
            return double.Parse(string.Format((value < 0 ? "-" : "") + "{0},{1:" + new string('0', places) + "}", Math.Abs(left), Math.Abs(right)));
        }

        public static double FixedTo8(double value, int digits = 8)
        {
            return Math.Round(value, digits, MidpointRounding.ToEven);
        }

        public static double CalcWinBet(double bet, double multiplier)
        {
            return FixedTo8(bet * FixedTo8(multiplier - 1, 2));
        }
    }
}
