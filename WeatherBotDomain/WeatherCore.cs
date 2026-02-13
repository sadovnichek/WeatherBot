using BotInfrastructure;
using WeatherBotDomain.Reply;

namespace WeatherBotDomain
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

    public class WeatherDescriptor
    {
        public string Description { get; init; }

        public string DayEmoji { get; init; }

        public string NightEmoji { get; init; }

        public bool IsWordingNeeded { get; init; }

        public bool IsPrecipitation { get; init; }

        public WeatherDescriptor(string description, 
            string dayEmoji, 
            string nightEmoji, 
            bool isWordingNeeded, 
            bool isPrecipitation) 
        {
            Description = description;
            DayEmoji = dayEmoji;
            NightEmoji = nightEmoji;
            IsWordingNeeded = isWordingNeeded;
            IsPrecipitation = isPrecipitation;
        }
    }

    public class WeatherCore
    {
        private static readonly Dictionary<Weather, WeatherDescriptor> weatherData = new()
        {
            { Weather.Sunny, new WeatherDescriptor("солнечная", "☀️", "🌙", true, false) },
            { Weather.PartlyCloudy, new WeatherDescriptor("переменная облачность", "⛅", "🌙", false, false) },
            { Weather.Cloudy, new WeatherDescriptor("облачная", "☁️", "🌙", true, false) },
            { Weather.Fog, new WeatherDescriptor("туман", "🌫️", "🌫️", false, false) },
            { Weather.Rainy, new WeatherDescriptor("дождь", "🌧️", "🌧️", false, true) },
            { Weather.SlightlyRainy, new WeatherDescriptor("небольшой дождь", "💧", "💧", false, true) },
            { Weather.Snowy, new WeatherDescriptor("снег", "🌨️", "🌨️", false, true) },
            { Weather.Shower, new WeatherDescriptor("ливень", "☔", "☔", false, true) },
            { Weather.Thunderstorm, new WeatherDescriptor("гроза", "⛈️", "⛈", false, true) }
        };

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

        public string GetDescription(int weatherCode)
        {
            return GetDescription(GetWeather(weatherCode));
        }

        public string GetDescription(Weather weather)
        {
            return weatherData[weather].Description;
        }

        public string GetEmoji(int weatherCode, bool isNight = false)
        {
            return GetEmoji(GetWeather(weatherCode), isNight);
        }

        public string GetEmoji(Weather weather, bool isNight = false)
        {
            return isNight
                ? weatherData[weather].NightEmoji
                : weatherData[weather].DayEmoji;
        }

        public bool IsWordingNeeded(int weatherCode)
        {
            return weatherData[GetWeather(weatherCode)].IsWordingNeeded;
        }

        public bool IsPrecipitationExpected(int[] weatherCodes)
        {
            return weatherCodes.Any(IsPrecipitation);
        }

        public bool IsPrecipitation(int weatherCode)
        {
            return IsPrecipitation(GetWeather(weatherCode));
        }

        public bool IsPrecipitation(Weather weather)
        {
            return weatherData[weather].IsPrecipitation;
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

            var emojies = string.Join("", weatherCodesModes.Select(x => GetEmoji(x)).Distinct());
            var withWording = weatherCodesModes.Where(IsWordingNeeded)
                .Select(GetDescription)
                .Distinct()
                .ToArray();
            var withoutWording = weatherCodesModes.Where(x => !IsWordingNeeded(x))
                .Select(GetDescription)
                .ToArray();

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
                return @"Доброй ночи\!";
            if (time.Hour >= 4 && time.Hour < 10)
                return @"Доброе утро\!";
            if (time.Hour >= 10 && time.Hour < 16)
                return @"Добрый день\!";

            return @"Добрый вечер\!";
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