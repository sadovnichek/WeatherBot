using BotInfrastructure;

namespace WeatherBotDomain
{
    public class WeatherCore
    {
        private enum Weather
        {
            Sunny,
            PartlyCloudy,
            Cloudy,
            Rainy,
            Fog,
            SlightlyRainy,
            Snowy,
            Shower,
            Thunderstorm
        }

        private static readonly Dictionary<int, Weather> weatherGrouping = new()
        {
            {0, Weather.Sunny },
            {1, Weather.Sunny },
            {2, Weather.PartlyCloudy },
            {3, Weather.Cloudy },
            {45, Weather.Fog },
            {51, Weather.Rainy },
            {53, Weather.Rainy },
            {55, Weather.Rainy },
            {61, Weather.SlightlyRainy },
            {71, Weather.Snowy },
            {73, Weather.Snowy },
            {77, Weather.Snowy },
            {80, Weather.Shower },
            {81, Weather.Shower },
            {85, Weather.Snowy },
            {95, Weather.Thunderstorm }
        };

        // Could be moved into DB?
        private static readonly Dictionary<Weather, string> weatherDescription = new()
        {
            { Weather.Sunny, "солнечная" },
            { Weather.PartlyCloudy, "переменная облачность" },
            { Weather.Cloudy, "облачная" },
            { Weather.Fog, "туман" },
            { Weather.Rainy, "дождь" },
            { Weather.SlightlyRainy, "небольшой дождь" },
            { Weather.Snowy, "снегопад" },
            { Weather.Shower, "ливень" },
            { Weather.Thunderstorm, "гроза" }
        };

        private static Dictionary<Weather, string> weatherEmojies = new()
        {
            { Weather.Sunny, "☀️" },
            { Weather.PartlyCloudy, "⛅" },
            { Weather.Cloudy, "☁️" },
            { Weather.Fog, "🌫️" },
            { Weather.Rainy, "🌧️" },
            { Weather.SlightlyRainy, "💧" },
            { Weather.Snowy, "🌨️" },
            { Weather.Shower, "☔" },
            { Weather.Thunderstorm, "⛈️" }
        };

        private static readonly Dictionary<Weather, bool> isWordingNeeded = new()
        {
            { Weather.Sunny, true },
            { Weather.PartlyCloudy, false },
            { Weather.Cloudy, true },
            { Weather.Fog, false },
            { Weather.Rainy, false },
            { Weather.SlightlyRainy, false },
            { Weather.Snowy, false },
            { Weather.Shower, false },
            { Weather.Thunderstorm, false }
        };

        private static readonly Dictionary<Weather, bool> isPrecipitation = new()
        {
            { Weather.Sunny, false },
            { Weather.PartlyCloudy, false },
            { Weather.Cloudy, false },
            { Weather.Fog, false },
            { Weather.Rainy, true },
            { Weather.SlightlyRainy, true },
            { Weather.Snowy, true },
            { Weather.Shower, true },
            { Weather.Thunderstorm, true }
        };

        public string GetDescription(int weatherCode)
        {
            if (!weatherDescription.TryGetValue(weatherGrouping[weatherCode], out var description))
                return $"{weatherCode}";

            return description;
        }

        public string GetEmoji(int weatherCode)
        {
            if (!weatherEmojies.TryGetValue(weatherGrouping[weatherCode], out var description))
                return $"{weatherCode}";

            return description;
        }

        public bool IsWordingNeeded(int weatherCode)
        {
            if (!isWordingNeeded.TryGetValue(weatherGrouping[weatherCode], out var answer))
                return false;

            return answer;
        }

        public static bool IsPrecipitationExpected(int[] weatherCodes)
        {
            return weatherCodes.Any(code => isPrecipitation[weatherGrouping[code]]);
        }

        /// <exception cref="ArgumentException"></exception>
        public static Dictionary<int, (int, int)> GetLongestSubsequence(int[] weatherCodes)
        {
            if (weatherCodes.Length == 0)
                throw new ArgumentException("Sequence is empty");

            var dict = new Dictionary<int, (int, int)>();
            int begin = 0, end = weatherCodes.Length - 1;
            for(var i = 1; i < weatherCodes.Length + 1; i++)
            {
                if (i < weatherCodes.Length && weatherCodes[i] == weatherCodes[i - 1])
                    continue;
                end = i - 1;
                if (dict.ContainsKey(weatherCodes[i - 1]))
                {
                    if (end - begin > dict[weatherCodes[i - 1]].Item2 - dict[weatherCodes[i - 1]].Item1)
                        dict[weatherCodes[i - 1]] = (begin, end);
                }
                else
                    dict.Add(weatherCodes[i - 1], (begin, end));
                begin = i;
            }

            return dict;
        }

        public WeatherReply GetReply(string timePointer,
            DateTime timeNow,
            int[] weatherCodes,
            double[] temperatures)
        {
            var greeting = GetGreeting(timeNow);
            var medianTemperatureWithinDay = GetValueRounded(temperatures, xs => xs.Median());
            var minTemperature = GetValueRounded(temperatures, xs => xs.Min());
            var maxTemperature = GetValueRounded(temperatures, xs => xs.Max());
            var weatherCodesModes = weatherCodes.Mode().ToList();

            if (weatherCodesModes.Count == 1)
            {
                var mainWeatherCode = weatherCodesModes.First();
                var weather = GetDescription(mainWeatherCode);
                var emoji = GetEmoji(mainWeatherCode);

                return new SimpleWeatherReply()
                {
                    Greeting = greeting,
                    IsWordingNeeded = IsWordingNeeded(mainWeatherCode),
                    TimePointer = timePointer,
                    Weather = weather,
                    MedianTemperature = medianTemperatureWithinDay,
                    MinTemperature = minTemperature,
                    MaxTemperature = maxTemperature,
                    Emoji = emoji,
                };
            }

            var emojies = string.Join("", weatherCodesModes.Select(GetEmoji).ToHashSet());
            var withWording = string.Join(" и ", weatherCodesModes.Where(IsWordingNeeded).Select(GetDescription).ToHashSet());
            var withoutWording = string.Join(", ", weatherCodesModes.Where(x => !IsWordingNeeded(x)).Select(GetDescription));

            return new CompoundWeatherReply()
            {
                Greeting = greeting,
                TimePointer = timePointer,
                MedianTemperature = medianTemperatureWithinDay,
                MinTemperature = minTemperature,
                MaxTemperature = maxTemperature,
                Emoji = emojies,
                WeathersWithWording = withWording,
                WeathersWithoutWording = withoutWording
            };
        }

        private double GetValueRounded(double[] values, Func<double[], double> func, int digits = 1)
        {
            return Math.Round(func(values), digits);
        }

        private string GetGreeting(DateTime time)
        {
            if (time.Hour >= 22 && time.Hour < 4)
                return "Доброй ночи!";
            if (time.Hour >= 4 && time.Hour < 10)
                return "Доброе утро!";
            if (time.Hour >= 10 && time.Hour < 16)
                return "Добрый день!";

            return "Добрый вечер!";
        }
    }
}