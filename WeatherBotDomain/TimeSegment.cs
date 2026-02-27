namespace WeatherBotDomain
{
    public record TimeSegment(TimeOnly Start, TimeOnly End)
    {
        public string GetStringRepresentation()
        {
            if (Start.Hour == 0 && Start.Minute == 0)
                return $"до {End}";

            if (End.Hour == 0 && End.Minute == 0)
                return $"с {Start}";

            return $"с {Start} до {End}";
        }

        public bool IsTimeInSegment(TimeOnly time)
        {
            return time.IsBetween(Start, End);
        }
    }
}