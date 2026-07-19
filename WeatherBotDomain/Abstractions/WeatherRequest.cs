using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain.Abstractions
{
    public abstract class WeatherRequest
    {
        public double Latitude { get; init; }

        public double Longitude { get; init; }

        public abstract HttpContent GetValues();
    }
}
