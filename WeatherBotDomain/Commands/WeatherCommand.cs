using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain.Commands
{
    public class OpenMeteoResponse
    {
        [JsonProperty("utc_offset_seconds")]
        public int UtcOffsetSeconds { get; set; }

        [JsonProperty("hourly")]
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonProperty("temperature_2m")]
        public double[] TemperaturePoints { get; set; }

        [JsonProperty("weather_code")]
        public int[] WeatherCodes { get; set; }
    }

    public class WeatherCommand : ICommand
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
            var response = await SendRequestAsync();
            return response;
        }

        private HttpContent GetValues()
        {
            var values = new Dictionary<string, string>
              {
                  { "latitude", "56.823457" },
                  { "longitude", "60.551424" },
                  { "hourly", "temperature_2m,weather_code" },
                  { "forecast_days", "1" },
                  { "timezone", "auto" }
              };
            return new FormUrlEncodedContent(values);
        }

        private async Task<string> SendRequestAsync()
        {
            var request = GetValues();
            var response = await httpClient.PostAsync(uriAddress, request);
            var str = await response.Content.ReadAsStringAsync();
            return ProcessResponce(str);
        }

        private string ProcessResponce(string response)
        {
            try
            {
                var parsedJson = JsonConvert.DeserializeObject<OpenMeteoResponse>(response);

                var utcOffset = parsedJson.UtcOffsetSeconds;
                var timeNow = DateTime.UtcNow.AddSeconds(utcOffset);

                var temperatures = parsedJson.Data.TemperaturePoints;
                var weatherCodes = parsedJson.Data.WeatherCodes;

                var minTemperature = Math.Round(temperatures.Min(), 1);
                var medianTemperatureWithinDay = Math.Round(temperatures.Skip(7).Take(12).Median(), 1);
                var maxTemperature = Math.Round(temperatures.Max(), 1);
                var weatherMode = weatherCodes.Mode().Select(x => weatherDomain.GetDescription(x));

                var greeting = GetGreeting(timeNow);

                return $"{greeting} Сегодня ожидается {string.Join(" и ", weatherMode)}.\n" +
                    $"Средняя температура днем: {medianTemperatureWithinDay}.\n" +
                    $"Перепады температур в течении суток с {minTemperature} до {maxTemperature}";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "An error occured :(";
            }
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