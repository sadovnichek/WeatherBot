using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain
{
    public static class Format
    {
        public static string Temperature(double temperature)
        {
            return $"{Math.Round(temperature, 2)}°C";
        }
    }
}