using BotInfrastructure;

namespace WeatherBotDomain.Commands
{
    public abstract class WeatherCommand : ICommand
    {
        private readonly HttpClient httpClient;
        private readonly string uriAddress;
        private readonly IMessageBus<string> bus;

        protected readonly WeatherCore weatherDomain;

        public abstract string Description { get; }

        public WeatherCommand(HttpClient client, 
            WeatherCore domain,
            IMessageBus<string> bus,
            string uri)
        {
            httpClient = client;
            weatherDomain = domain;
            uriAddress = uri;
        }

        public async Task Execute(string[] args)
        {
            var request = GetValues();
            var response = await httpClient.PostAsync(uriAddress, request);
            var str = await response.Content.ReadAsStringAsync();
            await bus.Put(ProcessResponse(str).BuildMessage());
            await bus.Put("Hello!");
        }

        protected abstract WeatherReply ProcessResponse(string jsonResponse);

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