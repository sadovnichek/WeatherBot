namespace WeatherBotDomain.Replies
{
    public class SimpleWeatherReply : WeatherReply
    {
        public string Weather { get; init; }

        public bool IsWordingNeeded { get; init; }

        public override string BuildMessage()
        {
            var wording = IsWordingNeeded ? " погода" : string.Empty;
            return $"{Greeting} {TimePointer} ожидается, в основном, *{Weather}{wording}* {Emoji}\n" +
                        $"Средняя температура днем: *{Format.Temperature(MedianTemperature)}*.\n" +
                        $"Перепады температур в течении суток с *{Format.Temperature(MinTemperature)}* до *{Format.Temperature(MaxTemperature)}*";
        }
    }
}