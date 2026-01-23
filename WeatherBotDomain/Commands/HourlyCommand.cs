using BotInfrastructure;
using Newtonsoft.Json;
using System.Text;

namespace WeatherBotDomain.Commands
{
    public class HourlyCommand : ICommand
    {
        private readonly HttpClient httpClient;
        private readonly string uriAddress;
        private readonly IMessageBus<string> messageBus;
        private readonly WeatherCore weatherDomain;

        public string Description => "Погода и температура на каждый час сегодня";

        public HourlyCommand(HttpClient client, 
            string uri, 
            IMessageBus<string> bus,
            WeatherCore domain)
        {
            httpClient = client;
            uriAddress = uri;
            messageBus = bus;
            weatherDomain = domain;
        }

        public async Task Execute(string[] args)
        {
            var request = GetValues();
            var response = await httpClient.PostAsync(uriAddress, request);
            var content = await response.Content.ReadAsStringAsync();
            var parsedJson = JsonConvert.DeserializeObject<OpenMeteoResponse>(content);
            await messageBus.Put(GetHourlyForecast(parsedJson));
        }

        private string GetHourlyForecast(OpenMeteoResponse response)
        {
            var sb = new StringBuilder();

            for(var i = 0; i < 24; i++)
            {
                sb.Append($"{i.ToString().PadLeft(2, '0')}:00 ");
                sb.Append(weatherDomain.GetEmoji(response.Data.WeatherCodes[i]));
                sb.Append(' ');
                sb.Append($"{response.Data.TemperaturePoints[i]}°C\n");
            }

            return sb.ToString();
        }

        //Dublicate?
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
    }
}
