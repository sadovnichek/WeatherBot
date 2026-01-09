using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain
{
    public class CompoundWeatherReply : WeatherReply
    {
        public override string BuildMessage()
        {
            return $"{Greeting} {TimePointer} ожидается {Weather} {Emoji}\n" +
                        $"Средняя температура днем: {MedianTemperature}.\n" +
                        $"Перепады температур в течении суток с {MinTemperature} до {MaxTemperature}";
        }
    }
}
