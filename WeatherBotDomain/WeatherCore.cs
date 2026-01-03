using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain
{
    public class WeatherCore
    {
        private static readonly Dictionary<int, string> weatherCodes = new()
        {
            {0, "солнечно" },
            {1, "преимущественно солнечно" },
            {2, "переменная облачность" },
            {3, "облачно" },
            {45, "туман" },
            {51, "дождь" },
            {53, "дождь" },
            {55, "дождь" },
            {61, "небольшой дождь" },
            {71, "снегопад" },
            {73, "снегопад" },
            {77, "снегопад" },
            {80, "ливень" },
            {81, "ливень" },
            {85, "снегопад" },
            {95, "гроза" }
        };

        public string GetDescription(int weatherCode)
        {
            if (!weatherCodes.TryGetValue(weatherCode, out var description))
                return $"Code {weatherCode}";

            return description;
        }
    }
}
