namespace WeatherBotDomain.Reply
{
    public class CompoundWeatherReply : WeatherReply
    {
        public string[] WeathersWithWording { get; init; }

        public string[] WeathersWithoutWording { get; init; }

        public override string BuildMessage()
        {
            var delimiter = WeathersWithoutWording.Length > 0 && WeathersWithWording.Length > 0 ? ", а также " : string.Empty;
            var wording = WeathersWithWording.Length > 0 ? " погода" : string.Empty;
            var withWording = string.Join(" и ", WeathersWithWording.Select(w => $"*{w}*"));
            var withoutWording = string.Join(", ", WeathersWithoutWording.Select(w => $"*{w}*"));
            
            return $"{Greeting} {TimePointer} ожидаются {withWording}{wording}{delimiter}{withoutWording} {Emoji}\n" +
                        $"Средняя температура днем: *{MedianTemperature}°C*.\n" +
                        $"Перепады температур в течении суток с *{MinTemperature}°C* до *{MaxTemperature}°C*";
        }
    }
}