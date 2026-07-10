using BotInfrastructure;
using WeatherBotDomain.Replies;

namespace WeatherBotDomain
{
    public class WeatherCore
    {
        private static readonly Dictionary<WeatherType, WeatherDescriptor> weatherData = new()
        {
            { WeatherType.Sunny, new WeatherDescriptor("солнечная", "☀️", "🌙", true, false) },
            { WeatherType.PartlyCloudy, new WeatherDescriptor("переменная облачность", "⛅", "🌙", false, false) },
            { WeatherType.Cloudy, new WeatherDescriptor("облачная", "☁️", "🌙", true, false) },
            { WeatherType.Fog, new WeatherDescriptor("туман", "🌫️", "🌙", false, false) },
            { WeatherType.RimeFog, new WeatherDescriptor("туман с изморозью", "🌫️", "🌙", false, false) },
            { WeatherType.Rainy, new WeatherDescriptor("дождь", "🌧️", "🌙", false, true) },
            { WeatherType.SlightlyRainy, new WeatherDescriptor("небольшой дождь", "💧", "🌙", false, true) },
            { WeatherType.FreezingDrizzle, new WeatherDescriptor("ледяной дождь", "❄️", "🌙", false, true) },
            { WeatherType.Snowy, new WeatherDescriptor("снег", "🌨️", "🌙", false, true) },
            { WeatherType.Shower, new WeatherDescriptor("ливень", "☔", "🌙", false, true) },
            { WeatherType.Thunderstorm, new WeatherDescriptor("гроза", "🌩️", "🌙", false, true) },
            { WeatherType.ThunderstormWithHail, new WeatherDescriptor("гроза с градом", "⛈️", "🌙", false, true) }
        };

        // TODO: what if there will be another weather code?
        private static readonly Dictionary<int, WeatherType> weatherGrouping = new()
        {
            {0, WeatherType.Sunny },
            {1, WeatherType.Sunny },
            {2, WeatherType.PartlyCloudy },
            {3, WeatherType.Cloudy },
            {45, WeatherType.Fog },
            {48, WeatherType.RimeFog },
            {51, WeatherType.SlightlyRainy },
            {53, WeatherType.SlightlyRainy },
            {55, WeatherType.SlightlyRainy },
            {56, WeatherType.FreezingDrizzle },
            {57, WeatherType.FreezingDrizzle },
            {61, WeatherType.SlightlyRainy },
            {63, WeatherType.Rainy },
            {65, WeatherType.Shower },
            {66, WeatherType.FreezingDrizzle },
            {67, WeatherType.FreezingDrizzle },
            {71, WeatherType.Snowy },
            {73, WeatherType.Snowy },
            {75, WeatherType.Snowy },
            {77, WeatherType.Snowy },
            {80, WeatherType.Shower },
            {81, WeatherType.Shower },
            {82, WeatherType.Shower },
            {85, WeatherType.Snowy },
            {86, WeatherType.Snowy },
            {95, WeatherType.Thunderstorm },
            {96, WeatherType.ThunderstormWithHail },
            {99, WeatherType.ThunderstormWithHail },
        };

        public string GetDescription(int weatherCode)
        {
            return GetDescription(GetWeather(weatherCode));
        }

        public string GetDescription(WeatherType weather)
        {
            return weatherData[weather].Description;
        }

        public string GetEmoji(int weatherCode, bool isNight = false)
        {
            return GetEmoji(GetWeather(weatherCode), isNight);
        }

        public string GetEmoji(WeatherType weather, bool isNight = false)
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

        public bool IsPrecipitation(WeatherType weather)
        {
            return weatherData[weather].IsPrecipitation;
        }

        /// <exception cref="ArgumentException"></exception>
        public WeatherType GetWeather(int weatherCode)
        {
            if (!weatherGrouping.TryGetValue(weatherCode, out var weather))
                throw new ArgumentException($"Unknown weather code: {weatherCode}");

            return weather;
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
            var weatherCodesModes = weatherCodes.Mode(1).ToList();

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
                    Date = timeNow
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
                WeathersWithoutWording = withoutWording,
                Date = timeNow
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

        public IEnumerable<WeatherSegment> GetWeatherSegments(int[] weatherCodes)
        {
            return ClassifyItemsByIndex(weatherCodes.Select(GetWeather).ToArray())
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
                        })
                    .ToArray()));
        }

        public PrecipitationReply GetPrecipitationForecast(int[] weatherCodes)
        {
            var weatherSegments = GetWeatherSegments(weatherCodes);

            var reply = new PrecipitationReply(weatherSegments);

            return reply;
        }
    }
}