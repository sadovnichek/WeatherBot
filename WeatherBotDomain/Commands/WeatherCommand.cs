using BotInfrastructure;
using Newtonsoft.Json;

namespace WeatherBotDomain.Commands
{
    public abstract class WeatherCommand : ICommand
    {
        private readonly HttpClient httpClient;
        private readonly string uriAddress;
        private readonly IMessageBus<string> messageBus;

        protected readonly WeatherCore weatherDomain;

        public abstract string Description { get; }

        public WeatherCommand(HttpClient client, 
            WeatherCore domain,
            IMessageBus<string> bus,
            string uri)
        {
            httpClient = client;
            weatherDomain = domain;
            messageBus = bus;
            uriAddress = uri;
        }

        public async Task Execute(string[] args)
        {
            var request = GetValues();
            var response = await httpClient.PostAsync(uriAddress, request);
            var str = await response.Content.ReadAsStringAsync();
            var parsedJson = JsonConvert.DeserializeObject<OpenMeteoResponse>(str);
            await messageBus.Put(ProcessResponse(parsedJson).BuildMessage());
            await messageBus.Put(GetPrecipitationForecast(parsedJson));
        }

        protected abstract WeatherReply ProcessResponse(OpenMeteoResponse response);

        protected abstract string GetPrecipitationForecast(OpenMeteoResponse response);

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