namespace WeatherBotDomain
{
    public class WeatherCore
    {
        // Could be moved into DB?
        private static readonly Dictionary<int, string> weatherCodes = new()
        {
            {0, "солнечная" },
            {1, "солнечная" },
            {2, "переменная облачность" },
            {3, "облачная" },
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

        private static Dictionary<int, string> weatherEmojies = new()
        {
            {0, "☀️" },
            {1, "🌤️" },
            {2, "⛅" },
            {3, "☁️" },
            {45, "🌫️" },
            {51, "🌧️" },
            {53, "🌧️" },
            {55, "🌧️" },
            {61, "💧" },
            {71, "🌨️" },
            {73, "🌨️" },
            {77, "🌨️" },
            {80, "☔" },
            {81, "☔" },
            {85, "🌨️" },
            {95, "⛈️" }
        };

        private static readonly Dictionary<int, bool> isWordingNeeded = new()
        {
            {0, true },
            {1, true },
            {2, false },
            {3, true },
            {45, false },
            {51, false },
            {53, false },
            {55, false },
            {61, false },
            {71, false },
            {73, false },
            {77, false },
            {80, false },
            {81, false },
            {85, false },
            {95, false }
        };

        public string GetDescription(int weatherCode)
        {
            if (!weatherCodes.TryGetValue(weatherCode, out var description))
                return $"{weatherCode}";

            return description;
        }

        public string GetEmoji(int weatherCode)
        {
            if (!weatherEmojies.TryGetValue(weatherCode, out var description))
                return $"{weatherCode}";

            return description;
        }

        public bool IsWordingNeeded(int weatherCode)
        {
            if (!isWordingNeeded.TryGetValue(weatherCode, out var answer))
                return false;

            return answer;
        }
    }
}