using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherBotDomain.OpenMeteo;

namespace WeatherBotDomain
{
    public class WeatherApiClient : IWeatherApiClient
    {
        private readonly HttpClient _client;

        public WeatherApiClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<string> TrySendRequestAsync(WeatherRequest request)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress);
            httpRequest.Content = request.GetValues();
            var response = await _client.SendAsync(httpRequest);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
