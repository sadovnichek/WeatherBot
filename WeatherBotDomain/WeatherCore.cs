using BotInfrastructure;
using System.Text;

namespace WeatherBotDomain
{
    public class WeatherCore
    {
        public enum Weather
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
            return weatherDescription[GetWeather(weatherCode)];
        }

        public string GetDescription(Weather weather)
        {
            return weatherDescription[weather];
        }

        public string GetEmoji(int weatherCode)
        {
            return weatherEmojies[GetWeather(weatherCode)];
        }

        public string GetEmoji(Weather weather)
        {
            return weatherEmojies[weather];
        }

        public bool IsWordingNeeded(int weatherCode)
        {
            return isWordingNeeded[GetWeather(weatherCode)];
        }

        public bool IsPrecipitationExpected(int[] weatherCodes)
        {
            return weatherCodes.Any(code => isPrecipitation[GetWeather(code)]);
        }

        public bool IsPrecipitation(int weatherCode)
        {
            return isPrecipitation[GetWeather(weatherCode)];
        }

        public bool IsPrecipitation(Weather weather)
        {
            return isPrecipitation[weather];
        }

        /// <exception cref="ArgumentException"></exception>
        public Weather GetWeather(int weatherCode)
        {
            if (weatherGrouping.TryGetValue(weatherCode, out var weather))
                return weather;

            throw new ArgumentException($"Unknown weather code: {weatherCode}");
        }

        /// <exception cref="ArgumentException"></exception>
        public static List<KeyValuePair<T, (int, int)>> ClassifyItemsByIndex<T>(T[] items)
            where T : notnull
        {
            if (items.Length == 0)
                throw new ArgumentException("Sequence is empty");

            var result = new List<KeyValuePair<T, (int, int)>>();
            int begin = 0, end = items.Length - 1;
            for(var i = 1; i < items.Length + 1; i++)
            {
                if (i < items.Length && items[i].Equals(items[i - 1]))
                    continue;
                end = i - 1;
                result.Add(new (items[i - 1], (begin, end)));
                begin = i;
            }

            return result;
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

        public IReply GetPrecipitationForecast(int[] weatherCodes)
        {
            var weatherSegments = ClassifyItemsByIndex(weatherCodes.Select(GetWeather).ToArray())
                    .Where(kv => IsPrecipitation(kv.Key))
                    .GroupBy(kv => kv.Key)
                    .Select(group => new WeatherSegment(
                        GetDescription(group.Key),
                        GetEmoji(group.Key),
                        group.Select(p =>
                        {
                            var start = new TimeOnly(p.Value.Item1, 0);
                            var end = new TimeOnly(p.Value.Item2, 0).AddHours(1);
                            return new TimeSegment(start, end);
                        }).ToArray()
                        ));

            var reply = new PrecipitationReply(weatherSegments);

            return reply;
        }
    }
}