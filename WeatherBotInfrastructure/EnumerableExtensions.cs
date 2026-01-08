using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotDomain
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
        public static IEnumerable<T> Mode<T>(this IEnumerable<T> source)
            where T : notnull
        {
            ArgumentNullException.ThrowIfNull(source);

            if (!source.Any())
                throw new InvalidOperationException("Sequence is empty");

            var frequency = new Dictionary<T, int>();

            foreach(var element in source)
            {
                frequency[element] = frequency.GetValueOrDefault(element) + 1;
            }

            var max = frequency.Max(kv => kv.Value);

            return frequency.Where(kv => kv.Value == max).Select(kv => kv.Key);
        }
    }
}