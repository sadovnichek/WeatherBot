using BotInfrastructure;
using WeatherBotDomain.Entities;
using WeatherBotDomain.Replies;

namespace WeatherBotDomain.Services
{
    public interface IWeatherService
    {
        string GetDescription(int weatherCode);

        string GetDescription(WeatherType weather);

        string GetEmoji(int weatherCode, bool isNight = false);

        string GetEmoji(WeatherType weather, bool isNight = false);

        bool IsWordingNeeded(int weatherCode);

        bool IsPrecipitationExpected(int[] weatherCodes);

        bool IsPrecipitation(int weatherCode);

        bool IsPrecipitation(WeatherType weather);

        WeatherType GetWeather(int weatherCode);

        WeatherReply GetReply(string timePointer,
            DateTime timeNow,
            bool isGreetingNeeded,
            int[] weatherCodes,
            double[] temperatures);
    }

    public class WeatherService : IWeatherService
    {
        private readonly Dictionary<WeatherType, WeatherDescriptor> weatherData = new()
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

        private readonly Dictionary<int, WeatherType> weatherGrouping = new()
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

        public WeatherReply GetReply(string timePointer,
            DateTime timeNow,
            bool isGreetingNeeded,
            int[] weatherCodes,
            double[] temperatures)
        {
            var greeting = GetGreeting(timeNow);
            var medianTemperatureWithinDay = temperatures.Median();
            var minTemperature = temperatures.Min();
            var maxTemperature = temperatures.Max();
            var weatherCodesModes = weatherCodes.Mode(1).ToList();

            if (weatherCodesModes.Count == 1)
            {
                var mainWeatherCode = weatherCodesModes.First();
                var weather = GetDescription(mainWeatherCode);
                var emoji = GetEmoji(mainWeatherCode);

                return new SimpleWeatherReply()
                {
                    Greeting = greeting,
                    IsGreetingNeeded = isGreetingNeeded,
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
                IsGreetingNeeded = isGreetingNeeded,
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