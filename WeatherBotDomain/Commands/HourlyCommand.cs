using BotInfrastructure;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

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
            if (!AreArgumentsValid(args))
            {
                await messageBus.Put("Неверные аргументы.\nНужно указать два числа - начало и конец временного промежутка от 0 до 23 часов");
                return;
            }
            
            var request = GetValues();
            var response = await httpClient.PostAsync(uriAddress, request);
            var content = await response.Content.ReadAsStringAsync();
            var parsedJson = JsonConvert.DeserializeObject<OpenMeteoResponse>(content);
            await messageBus.Put(GetHourlyForecast(parsedJson, args));
        }

        private bool AreArgumentsValid(string[] args)
        {
            if (args.Length == 0)
                return true;

            if (args.Length == 2)
            {
                return Regex.IsMatch(args[0], @"\b([0-9]|1[0-9]|2[0-3])\b")
                    && Regex.IsMatch(args[1], @"\b([0-9]|1[0-9]|2[0-3])\b");
            }

            return false;
        }

        private string GetHourlyForecast(OpenMeteoResponse response, string[] args)
        {
            var sb = new StringBuilder();

            var start = args.Length == 0
                ? 0
                : int.Parse(args[0]);
            var end = args.Length == 0
                ? 24
                : int.Parse(args[1]) + 1;

            for(var i = start; i < end; i++)
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