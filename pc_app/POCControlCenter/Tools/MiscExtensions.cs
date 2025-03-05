using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter
{
    /// <summary>
    ///     Misc extensions
    /// </summary>
    public static class MiscExtensions
    {
        /// <summary>
        ///     As the BCL doesn't include a ForEach on the IEnumerable, this extension was added
        /// </summary>
        /// <typeparam name="T">Type of the IEnumerable</typeparam>
        /// <param name="source">The IEnumerable</param>
        /// <param name="action">Action to call for each item</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        ///     Create a query string from a list of keyValuePairs
        /// </summary>
        /// <typeparam name="T">type for the value, sometimes it's easier to let this method call ToString on your type.</typeparam>
        /// <param name="keyValuePairs">list of keyValuePair with string,T</param>
        /// <returns>name1=value1&amp;name2=value2 etc...</returns>
        public static string ToQueryString<T>(this IEnumerable<KeyValuePair<string, T>> keyValuePairs)
        {
            if (keyValuePairs == null)
            {
                throw new ArgumentNullException(nameof(keyValuePairs));
            }
            var queryBuilder = new StringBuilder();

            foreach (var keyValuePair in keyValuePairs)
            {
                queryBuilder.Append($"{keyValuePair.Key}");
                if (keyValuePair.Value != null)
                {
                    var encodedValue = Uri.EscapeDataString(keyValuePair.Value?.ToString());
                    queryBuilder.Append($"={encodedValue}");
                }
                queryBuilder.Append('&');
            }
            if (queryBuilder.Length > 0)
            {
                queryBuilder.Length -= 1;
            }
            return queryBuilder.ToString();
        }
    }
}
