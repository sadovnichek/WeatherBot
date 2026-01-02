using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain
{
    public class WeatherDomain
    {

    }

    internal class WeatherCommand : ICommand
    {
        private readonly HttpClient httpClient;
        private readonly WeatherDomain weatherDomain;
        private const string uri = "https://api.open-meteo.com/v1/forecast";

        public WeatherCommand(HttpClient client, 
            WeatherDomain domain)
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
            return await response.Content.ReadAsStringAsync();
        }
    }
}