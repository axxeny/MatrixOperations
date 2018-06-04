using System.Collections.Generic;
using System.Linq;

namespace AxxenyUtilities.Helpers
{
    public static class EnumerableExtensions
    {
        public static T[] AsOrToArray<T>(this IEnumerable<T> enumerable) => enumerable as T[] ?? enumerable.ToArray();
        public static List<T> AsOrToList<T>(this IEnumerable<T> enumerable) => enumerable as List<T> ?? enumerable.ToList();
        public static IList<T> AsIListOrToList<T>(this IEnumerable<T> enumerable) => enumerable as IList<T> ?? enumerable.ToList();
        public static ICollection<T> AsICollectionOrToList<T>(this IEnumerable<T> enumerable) => enumerable as ICollection<T> ?? enumerable.ToList();
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable) => new HashSet<T>(enumerable);
        public static HashSet<T> AsOrToHashSet<T>(this IEnumerable<T> enumerable) => enumerable as HashSet<T> ?? new HashSet<T>(enumerable);

        public static IEnumerable<T> ConcatOrNotNullOrDefault<T>(this IEnumerable<T> one, IEnumerable<T> another)
        {
            return one == null
                ? another
                : (another == null
                    ? one
                    : one.Concat(another));
        }
        public static List<T> ConcatOrNotNullOrDefault<T>(this List<T> one, IEnumerable<T> another)
        {
            if (one == null) return another.AsOrToList();
            if (another == null) return one;
            one.AddRange(another);
            return one;
        }

        public static IEnumerable<T> ConcatOneOrNotNullOrDefault<T>(this IEnumerable<T> one, T another)
        {
            var anotherArray = new[] { another };
            return one == null
                ? anotherArray
                : (another == null
                    ? one
                    : one.Concat(anotherArray));
        }
    }
}