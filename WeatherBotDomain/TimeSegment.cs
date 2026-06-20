namespace WeatherBotDomain
{
    public record TimeSegment(TimeOnly Start, TimeOnly End)
    {
        public string GetStringRepresentation()
        {
            if (Start.Hour == 0 && End.Hour == 0)
                return $"весь день";

            if (Start.Hour == 0)
                return $"до {End}";

            if (End.Hour == 0)
                return $"с {Start}";

            return $"с {Start} до {End}";
        }

        public bool IsTimeInSegment(TimeOnly time)
        {
            return time.IsBetween(Start, End);
        }

        public static IEnumerable<TimeSegment> Join(TimeSegment[] segments)
        {
            if (segments.Length == 0)
                yield break;

            var sorted = segments.OrderBy(s => s.Start).ToArray();
            var currentStart = sorted[0].Start;
            var currentEnd = sorted[0].End;

            for (var i = 1; i < sorted.Length; i++)
            {
                if (sorted[i].Start > currentEnd)
                {
                    yield return new TimeSegment(currentStart, currentEnd);
                    currentStart = sorted[i].Start;
                    currentEnd = sorted[i].End;
                }
                if (sorted[i].End > currentEnd)
                    currentEnd = sorted[i].End;
            }

            yield return new TimeSegment(currentStart, currentEnd);
        }
    }
}