// ReSharper disable once CheckNamespace
namespace Utils
{
    public static class EnumerableExtensions
    {
        public static List<T>? AsList<T>(this IEnumerable<T> source)
        {
            return source is null or List<T> ? (List<T>?)source : source.ToList();
        }

        public static IList<T>? AsIList<T>(this IEnumerable<T> source)
        {
            return source is null or IList<T> ? (IList<T>?)source : source.ToList();
        }

        public static T[]? AsArray<T>(this IEnumerable<T> source)
        {
            return source is null or T[] ? (T[]?)source : source.ToArray();
        }

        public static T[] ToArray<T>(this IEnumerable<T> source, int capacity)
        {
            if(source == null)
                throw new ArgumentNullException(nameof(source));
            if(capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Non-negative number required.");

            var array = new T[capacity];

            var i = 0;
            foreach(var item in source)
            {
                if(i >= capacity)
                    throw new ArgumentOutOfRangeException(nameof(capacity), "Source.Count() cannot be greater than capacity.");

                array[i++] = item;
            }

            if(i < capacity)
                array = array[..i];

            return array;
        }

        public static bool HasValues<T>(this IEnumerable<T> source)
        {
            try
            {
                return source != null && source.Any();
            }
            catch(Exception)
            {
                return false;
            }
        }

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int qty)
        {
            var random = new Random();

            var randomOrder = source.OrderBy(x => random.Next());

            var result = randomOrder.Take(qty);

            return result;
        }
    }
}
