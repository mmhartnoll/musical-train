namespace Tools.Extensions
{
    public static class IAsyncEnumerableExtensions
    {
        public async static Task<IList<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
        {
            List<T> list = new();

            await foreach (var item in source.ConfigureAwait(false))
                list.Add(item);

            return list;
        }
    }
}
