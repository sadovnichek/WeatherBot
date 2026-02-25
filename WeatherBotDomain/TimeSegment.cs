namespace WeatherBotDomain
{
    public record TimeSegment(TimeOnly Start, TimeOnly End)
    {
        public string GetStringRepresentation()
        {
            return $"с {Start} до {End}";
        }

        public bool IsTimeInSegment(TimeOnly time)
        {
            return time >= Start && time <= End;
        }
    }
}