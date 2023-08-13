namespace WebLaunchPad.Api.Extensions;

public static class CollectionExtensions
{
    public static IEnumerable<T> RepeatForever<T>(this ICollection<T> source)
    {
        while (true)
        {
            foreach (var item in source)
            {
                yield return item;
            }
        }
        // ReSharper disable once IteratorNeverReturns
    }
}