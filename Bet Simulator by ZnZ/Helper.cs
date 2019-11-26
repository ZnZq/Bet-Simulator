using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bet_Simulator_by_ZnZ
{
    public static class Helper
    {
        public static int Range(int value, int min, int max) => value < min ? min : (value > max ? max : value);
        public static double Range(double value, double min, double max) => value < min ? min : (value > max ? max : value);
    }
}
