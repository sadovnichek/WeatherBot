using BotInfrastructure;
using Newtonsoft.Json;
using System.Threading.Channels;

namespace WeatherBotDomain.Commands
{
    public abstract class WeatherCommand : ICommand
    {
        private readonly HttpClient httpClient;
        private readonly string uriAddress;

        protected readonly WeatherCore weatherDomain;

        public abstract string Description { get; }

        public WeatherCommand(HttpClient client, 
            WeatherCore domain,
            string uri)
        {
            httpClient = client;
            weatherDomain = domain;
            uriAddress = uri;
        }

        public async IAsyncEnumerable<IReply> Execute(string[] args)
        {
            var request = GetValues();
            var response = await httpClient.PostAsync(uriAddress, request);
            var content = await response.Content.ReadAsStringAsync();
            var parsedJson = JsonConvert.DeserializeObject<OpenMeteoResponse>(content);

            yield return ProcessResponse(parsedJson);
            yield return GetPrecipitationForecast(parsedJson);
        }

        protected abstract IReply ProcessResponse(OpenMeteoResponse response);

        protected abstract IReply GetPrecipitationForecast(OpenMeteoResponse response);

        //Dublicate
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