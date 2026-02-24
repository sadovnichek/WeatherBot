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
    }
}