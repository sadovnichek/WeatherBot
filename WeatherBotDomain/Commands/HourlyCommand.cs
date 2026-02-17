using BotInfrastructure;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using WeatherBotDomain.Reply;

namespace WeatherBotDomain.Commands
{
    public class HourlyCommand : ICommand
    {
        private readonly HttpClient httpClient;
        private readonly string uriAddress;
        private readonly WeatherCore weatherDomain;

        public string Description => "Погода и температура на каждый час сегодня";

        public HourlyCommand(HttpClient client, 
            string uri,
            WeatherCore domain)
        {
            httpClient = client;
            uriAddress = uri;
            weatherDomain = domain;
        }

        public async IAsyncEnumerable<IReply> Execute(string[] args)
        {
            if (!AreArgumentsValid(args))
            {
                yield return new PlainReply("""
                    Неверные аргументы.
                    Нужно указать два числа - начало и конец временного промежутка от 0 до 23 часов
                    """);
                yield break;
            }
            
            var request = GetValues();
            var response = await httpClient.PostAsync(uriAddress, request);
            var content = await response.Content.ReadAsStringAsync();
            var parsedJson = JsonConvert.DeserializeObject<OpenMeteoResponse>(content);

            var startIndex = args.Length > 0 ? int.Parse(args[0]) : 0;
            var endIndex = args.Length > 0 ? int.Parse(args[1]) + 1 : 24;

            var reply = new HourlyForecastReply();
            var daytime = new TimeSegment(TimeOnly.Parse(parsedJson.DailyData.Sunrise[0]),
                TimeOnly.Parse(parsedJson.DailyData.Sunset[0]));
            for(var i = startIndex; i < endIndex; i++)
            {
                var time = new TimeOnly(i, 0);
                var isDay = daytime.IsTimeInSegment(time);
                var emoji = weatherDomain.GetEmoji(parsedJson.WeatherData.WeatherCodes[i], !isDay);
                var temperature = parsedJson.WeatherData.TemperaturePoints[i];
                var data = new HourlyForecastData(time, emoji, temperature);
                reply.AppendData(data);
            }

            yield return reply;
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

        //Dublicate?
        private HttpContent GetValues()
        {
            var values = new Dictionary<string, string>
            {
                  { "latitude", "56.823457" },
                  { "longitude", "60.551424" },
                  { "hourly", "temperature_2m,weather_code" },
                  { "daily", "sunrise,sunset" },
                  { "forecast_days", "1" },
                  { "timezone", "auto" }
            };
            return new FormUrlEncodedContent(values);
        }
    }
}