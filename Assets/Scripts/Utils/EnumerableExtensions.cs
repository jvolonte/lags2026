using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public static class EnumerableExtensions
    {
        static readonly System.Random Random = new();

        public static T PickOne<T>(this IEnumerable<T> source) =>
            source.Shuffle().First();

        public static IEnumerable<T> PickMany<T>(this IEnumerable<T> source, int amount) =>
            source.Shuffle().Take(amount);

        public static IEnumerable<T> PickManyWithReplacement<T>(this IList<T> source, int amount)
        {
            if (source == null || source.Count == 0)
                yield break;

            for (var i = 0; i < amount; i++)
                yield return source[Random.Next(source.Count)];
        }

        public static T PickOneWeighted<T>(this IList<T> source, Func<T, int> weightSelector)
        {
            var totalWeight = source.Sum(weightSelector);
            var roll = Random.Next(totalWeight);
            var cumulative = 0;

            foreach (var item in source)
            {
                cumulative += weightSelector(item);
                if (roll < cumulative)
                    return item;
            }

            return source[0];
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source) =>
            source.OrderBy(x => Random.Next());
    }
}