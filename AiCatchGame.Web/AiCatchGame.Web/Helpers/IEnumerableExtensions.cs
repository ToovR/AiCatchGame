namespace AiCatchGame.Web.Helpers
{
    public static class IEnumerableExtensions
    {
        public static int? MaxOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            if (source.Count() == 0)
            {
                return null;
            }

            return source.Max(selector);
        }
    }
}