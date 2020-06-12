using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhilLibX
{
    /// <summary>
    /// A class with Enumerable Extensions
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, TSource start, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            foreach (TSource element in source)
                if (predicate(element)) return element;

            return start;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultVal, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            TSource result = defaultVal;

            foreach (TSource element in source)
            {
                if (predicate(element))
                {
                    result = element;
                }
            }

            return result;
        }
    }
}
