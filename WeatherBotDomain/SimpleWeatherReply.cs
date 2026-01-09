using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain
{
    public class SimpleWeatherReply : WeatherReply
    {
        public override string BuildMessage()
        {
            return $"{Greeting} {TimePointer} ожидается, в основном, {Weather} {Wording} {Emoji}\n" +
                        $"Средняя температура днем: {MedianTemperature}.\n" +
                        $"Перепады температур в течении суток с {MinTemperature} до {MaxTemperature}";
        }
    }
}
