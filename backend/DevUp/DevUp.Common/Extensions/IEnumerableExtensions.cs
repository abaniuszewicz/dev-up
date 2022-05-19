namespace DevUp.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var element in enumerable)
                action(element);
        }

        public static bool None<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            return !enumerable.Any(predicate);
        }
    }
}
