namespace WeatherBotDomain.Commands
{
    public abstract class WeatherCommand : ICommand
    {
        private readonly HttpClient httpClient;
        private readonly WeatherCore weatherDomain;
        private readonly string uriAddress;

        public string Description => "описание команды weather";

        public WeatherCommand(HttpClient client, 
            WeatherCore domain,
            string uri)
        {
            httpClient = client;
            weatherDomain = domain;
            uriAddress = uri;
        }

        public async Task<string> Execute(string[] args)
        {
            var request = GetValues();
            var response = await httpClient.PostAsync(uriAddress, request);
            var str = await response.Content.ReadAsStringAsync();
            return ProcessResponse(str).BuildMessage();
        }

        protected abstract WeatherReply ProcessResponse(string jsonResponse);

        public WeatherReply GetMessage(string timePointer, 
            DateTime timeNow, 
            int[] weatherCodes,
            double[] temperatures)
        {
            var greeting = GetGreeting(timeNow);
            var medianTemperatureWithinDay = GetValueRounded(temperatures, xs => xs.Median());
            var minTemperature = GetValueRounded(temperatures, xs => xs.Min());
            var maxTemperature = GetValueRounded(temperatures, xs => xs.Max());
            var weatherCodesModes = weatherCodes.Mode().ToList();

            if(weatherCodesModes.Count == 1)
            {
                var mainWeatherCode = weatherCodesModes.First();
                var weather = weatherDomain.GetDescription(mainWeatherCode);
                var emoji = weatherDomain.GetEmoji(mainWeatherCode);

                var wording = weatherDomain.IsWordingNeeded(mainWeatherCode) ? "погода" : string.Empty;

                return new SimpleWeatherReply()
                {
                    Greeting = greeting,
                    TimePointer = timePointer,
                    Weather = weather,
                    MedianTemperature = medianTemperatureWithinDay,
                    MinTemperature = minTemperature,
                    MaxTemperature = maxTemperature,
                    Emoji = emoji,
                    Wording = wording
                };
            }

            var emojies = string.Join("", weatherCodesModes.Select(weatherDomain.GetEmoji).ToHashSet());
            var withWording = string.Join(" и ", weatherCodesModes.Where(weatherDomain.IsWordingNeeded).Select(weatherDomain.GetDescription).ToHashSet()) + " погода";
            var withoutWording = string.Join(", ", weatherCodesModes.Where(x => !weatherDomain.IsWordingNeeded(x)).Select(weatherDomain.GetDescription));

            return new CompoundWeatherReply()
            {
                Greeting = greeting,
                TimePointer = timePointer,
                MedianTemperature = medianTemperatureWithinDay,
                MinTemperature = minTemperature,
                MaxTemperature = maxTemperature,
                Emoji = emojies,
                Weather = $"{withWording}, {withoutWording}"
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

        private HttpContent GetValues()
        {
            var values = new Dictionary<string, string>
            {
                  { "latitude", "56.823457" },
                  { "longitude", "60.551424" },
                  { "hourly", "temperature_2m,weather_code" },
                  { "forecast_days", "2" },
                  { "timezone", "auto" }
            };
            return new FormUrlEncodedContent(values);
        }
    }
}