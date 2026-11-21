using System.Diagnostics.Contracts;
using System.Numerics;

namespace BotInfrastructure
{
    public static class EnumerableExtensions
    {
        [Pure]
        public static T Median<T>(this IEnumerable<T> source)
            where T : IComparable<T>, INumber<T>
        {
            ArgumentNullException.ThrowIfNull(source);

            if (!source.Any())
                throw new InvalidOperationException("Sequence is empty");

            var materialized = source.OrderBy(x => x).ToList();

            if(materialized.Count % 2 == 0)
                return T.CreateChecked(0.5) * (materialized[materialized.Count / 2 - 1] + materialized[materialized.Count / 2]);

            return materialized[materialized.Count / 2];
        }

        [Pure]
        public static IEnumerable<T> Mode<T>(this IEnumerable<T> source, uint discrepancy = 0)
            where T : notnull
        {
            ArgumentNullException.ThrowIfNull(source);

            if (!source.Any())
                throw new InvalidOperationException("Unable to calculate mode. Sequence is empty");

            var frequency = new Dictionary<T, int>();

            var max = 0;
            foreach(var element in source)
            {
                frequency[element] = frequency.GetValueOrDefault(element) + 1;
                if (frequency[element] > max)
                    max = frequency[element];
            }

            return frequency.Where(kv => Math.Abs(kv.Value - max) <= discrepancy).Select(kv => kv.Key);
        }

        /// <exception cref="ArgumentException"></exception>
        public static List<KeyValuePair<T, (int, int)>> ClassifyItemsByIndex<T>(this T[] items)
            where T : notnull
        {
            if (items.Length == 0)
                throw new ArgumentException("Sequence is empty");

            var result = new List<KeyValuePair<T, (int, int)>>();
            int begin = 0, end = items.Length - 1;
            for (var i = 1; i < items.Length + 1; i++)
            {
                if (i < items.Length && items[i].Equals(items[i - 1]))
                    continue;
                end = i - 1;
                result.Add(new(items[i - 1], (begin, end)));
                begin = i;
            }

            return result;
        }

        public static long ToUnixEpoch(this DateTime dateTime)
        {
            var dateTimeOffset = new DateTimeOffset(dateTime);
            return dateTimeOffset.ToUnixTimeSeconds();
        }
    }
}