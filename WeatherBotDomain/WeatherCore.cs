namespace WeatherBotDomain
{
    public class WeatherCore
    {
        // Could be moved into DB?
        private static readonly Dictionary<int, string> weatherCodes = new()
        {
            {0, "солнечная" },
            {1, "солнечная" },
            {2, "переменная облачность" },
            {3, "облачная" },
            {45, "туман" },
            {51, "дождь" },
            {53, "дождь" },
            {55, "дождь" },
            {61, "небольшой дождь" },
            {71, "снегопад" },
            {73, "снегопад" },
            {77, "снегопад" },
            {80, "ливень" },
            {81, "ливень" },
            {85, "снегопад" },
            {95, "гроза" }
        };

        private static Dictionary<int, string> weatherEmojies = new()
        {
            {0, "☀️" },
            {1, "🌤️" },
            {2, "⛅" },
            {3, "☁️" },
            {45, "🌫️" },
            {51, "🌧️" },
            {53, "🌧️" },
            {55, "🌧️" },
            {61, "💧" },
            {71, "🌨️" },
            {73, "🌨️" },
            {77, "🌨️" },
            {80, "☔" },
            {81, "☔" },
            {85, "🌨️" },
            {95, "⛈️" }
        };

        private static readonly Dictionary<int, bool> isWordingNeeded = new()
        {
            {0, true },
            {1, true },
            {2, false },
            {3, true },
            {45, false },
            {51, false },
            {53, false },
            {55, false },
            {61, false },
            {71, false },
            {73, false },
            {77, false },
            {80, false },
            {81, false },
            {85, false },
            {95, false }
        };

        private static readonly Dictionary<int, bool> isPrecipitation = new()
        {
            {0, false },
            {1, false },
            {2, false },
            {3, false },
            {45, false },
            {51, true },
            {53, true },
            {55, true },
            {61, true },
            {71, true },
            {73, true },
            {77, true },
            {80, true },
            {81, true },
            {85, true },
            {95, true }
        };

        public string GetDescription(int weatherCode)
        {
            if (!weatherCodes.TryGetValue(weatherCode, out var description))
                return $"{weatherCode}";

            return description;
        }

        public string GetEmoji(int weatherCode)
        {
            if (!weatherEmojies.TryGetValue(weatherCode, out var description))
                return $"{weatherCode}";

            return description;
        }

        public bool IsWordingNeeded(int weatherCode)
        {
            if (!isWordingNeeded.TryGetValue(weatherCode, out var answer))
                return false;

            return answer;
        }

        public static bool IsPrecipitationExpected(int[] weatherCodes)
        {
            return weatherCodes.Any(code => isPrecipitation[code]);
        }

        /// <exception cref="ArgumentException"></exception>
        public static Dictionary<int, (int, int)> GetLongestSubsequence(int[] weatherCodes)
        {
            if (weatherCodes.Length == 0)
                throw new ArgumentException("Sequence is empty");

            var dict = new Dictionary<int, (int, int)>();
            int begin = 0, end = weatherCodes.Length - 1;
            for(var i = 1; i < weatherCodes.Length + 1; i++)
            {
                if (i < weatherCodes.Length && weatherCodes[i] == weatherCodes[i - 1])
                    continue;
                end = i - 1;
                if (dict.ContainsKey(weatherCodes[i - 1]))
                {
                    if (end - begin > dict[weatherCodes[i - 1]].Item2 - dict[weatherCodes[i - 1]].Item1)
                        dict[weatherCodes[i - 1]] = (begin, end);
                }
                else
                    dict.Add(weatherCodes[i - 1], (begin, end));
                begin = i;
            }

            return dict;
        }

        public WeatherReply GetReply(string timePointer,
            DateTime timeNow,
            int[] weatherCodes,
            double[] temperatures)
        {
            var greeting = GetGreeting(timeNow);
            var medianTemperatureWithinDay = GetValueRounded(temperatures, xs => xs.Median());
            var minTemperature = GetValueRounded(temperatures, xs => xs.Min());
            var maxTemperature = GetValueRounded(temperatures, xs => xs.Max());
            var weatherCodesModes = weatherCodes.Mode().ToList();

            if (weatherCodesModes.Count == 1)
            {
                var mainWeatherCode = weatherCodesModes.First();
                var weather = GetDescription(mainWeatherCode);
                var emoji = GetEmoji(mainWeatherCode);

                return new SimpleWeatherReply()
                {
                    Greeting = greeting,
                    TimePointer = timePointer,
                    Weather = weather,
                    MedianTemperature = medianTemperatureWithinDay,
                    MinTemperature = minTemperature,
                    MaxTemperature = maxTemperature,
                    Emoji = emoji,
                };
            }

            var emojies = string.Join("", weatherCodesModes.Select(GetEmoji).ToHashSet());
            var withWording = string.Join(" и ", weatherCodesModes.Where(IsWordingNeeded).Select(GetDescription).ToHashSet());
            var withoutWording = string.Join(", ", weatherCodesModes.Where(x => !IsWordingNeeded(x)).Select(GetDescription));

            return new CompoundWeatherReply()
            {
                Greeting = greeting,
                TimePointer = timePointer,
                MedianTemperature = medianTemperatureWithinDay,
                MinTemperature = minTemperature,
                MaxTemperature = maxTemperature,
                Emoji = emojies,
                WeathersWithWording = withWording,
                WeathersWithoutWording = withoutWording
            };
        }

        private double GetValueRounded(double[] values, Func<double[], double> func, int digits = 1)
        {
            return Math.Round(func(values), digits);
        }

        private string GetGreeting(DateTime time)
        {
            if (time.Hour >= 22 && time.Hour < 4)
                return "Доброй ночи!";
            if (time.Hour >= 4 && time.Hour < 10)
                return "Доброе утро!";
            if (time.Hour >= 10 && time.Hour < 16)
                return "Добрый день!";

            return "Добрый вечер!";
        }
    }
}