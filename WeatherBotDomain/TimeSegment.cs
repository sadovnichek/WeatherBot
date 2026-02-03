using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain
{
    public record TimeSegment(TimeOnly Start, TimeOnly End)
    {
        public string GetStringRepresentation()
        {
            return $"{Start} - {End}";
        }
    }
}
