namespace WeatherBotDomain
{
    public record TimeSegment(TimeOnly Start, TimeOnly End)
    {
        public string GetStringRepresentation()
        {
            return $"{Start} - {End}";
        }

        public bool IsTimeInSegment(TimeOnly time)
        {
            return time >= Start && time <= End;
        }
    }
}