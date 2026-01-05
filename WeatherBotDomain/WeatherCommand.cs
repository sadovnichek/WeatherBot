using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain
{
    public class OpenMeteoResponse
    {
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
        private const string uri = "https://api.open-meteo.com/v1/forecast";

        public string Description => "описание команды weather";

        public WeatherCommand(HttpClient client, 
            WeatherCore domain)
        {
            httpClient = client;
            weatherDomain = domain;
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
            var response = await httpClient.PostAsync(uri, request);
            var str = await response.Content.ReadAsStringAsync();
            return ProcessResponce(str);
        }

        private string ProcessResponce(string response)
        {
            try
            {
                var parsedJson = JsonConvert.DeserializeObject<OpenMeteoResponse>(response);

                var temperatures = parsedJson.Data.TemperaturePoints;
                var weatherCodes = parsedJson.Data.WeatherCodes;

                var minTemperature = temperatures.Min();
                var medianTemperature = temperatures.Median();
                var maxTemperature = temperatures.Max();
                var weatherMode = weatherCodes.Mode().Select(x => weatherDomain.GetDescription(x));

                return $"Сегодня будет {string.Join(",", weatherMode)}.\n" +
                    $"Средняя температура: {medianTemperature}.\n" +
                    $"Перепады температур: {minTemperature} - {maxTemperature}";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "An error occured :(";
            }
        }
    }
}