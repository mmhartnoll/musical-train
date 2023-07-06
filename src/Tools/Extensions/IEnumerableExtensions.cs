namespace Tools.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Enumerate<T>(this IEnumerable<T> source)
        {
            foreach (var item in source)
                yield return item;
        }
    }
}