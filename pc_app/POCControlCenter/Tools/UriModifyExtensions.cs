using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter
{
    /// <summary>
    ///     Uri extensions which modify an Uri (return a new one)
    /// </summary>
    public static class UriModifyExtensions
    {
        /// <summary>
        ///     Append path segment(s) to the specified Uri
        /// </summary>
        /// <param name="uri">Uri to extend</param>
        /// <param name="segments">array of objects which will be added after converting them to strings</param>
        /// <returns>new Uri with segments added to the path</returns>
        public static Uri AppendSegments(this Uri uri, params object[] segments)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }
            var uriBuilder = new UriBuilder(uri);

            if (segments != null)
            {
                var stringBuilder = new StringBuilder();
                // Only add the path if it contains more that just a /
                if (!"/".Equals(uriBuilder.Path))
                {
                    stringBuilder.Append(uriBuilder.Path);
                }
                foreach (var segment in segments)
                {
                    // Do nothing with null segments
                    if (segment == null)
                    {
                        continue;
                    }

                    // Add a / if the current path doesn't end with it and the segment doesn't have one
                    var hasPathTrailingSlash = stringBuilder.ToString().EndsWith("/");
                    var hasSegmentTrailingSlash = segment.ToString().StartsWith("/");
                    if (hasPathTrailingSlash && hasSegmentTrailingSlash)
                    {
                        // Remove trailing slash
                        stringBuilder.Length -= 1;
                    }
                    else if (!hasPathTrailingSlash && !hasSegmentTrailingSlash)
                    {
                        stringBuilder.Append("/");
                    }

                    // Add the segment
                    stringBuilder.Append(segment);
                }
                uriBuilder.Path = stringBuilder.ToString();
            }
            return uriBuilder.Uri;
        }

        /// <summary>
        ///     Adds query string value to an existing url, both absolute and relative URI's are supported.
        /// </summary>
        /// <code>
        ///     // returns "www.domain.com/test?param1=val1&amp;param2=val2&amp;param3=val3"
        ///     new Uri("www.domain.com/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
        /// 
        ///     // returns "/test?param1=val1&amp;param2=val2&amp;param3=val3"
        ///     new Uri("/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
        /// </code>
        /// <param name="uri">Uri to extend</param>
        /// <param name="name">string name of value</param>
        /// <param name="value">value</param>
        /// <returns>Uri with extended query</returns>
        public static Uri ExtendQuery<T>(this Uri uri, string name, T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            var keyValuePairs = uri.QueryToKeyValuePairs().Concat(new[] { new KeyValuePair<string, string>(name, value.ToString()) });

            var uriBuilder = new UriBuilder(uri)
            {
                Query = keyValuePairs.ToQueryString()
            };
            return uriBuilder.Uri;
        }

        /// <summary>
        ///     Adds query string value to an existing url, both absolute and relative URI's are supported.
        /// </summary>
        /// <code>
        ///     // returns "www.domain.com/test?param1=val1&amp;param2=val2&amp;param3=val3"
        ///     new Uri("www.domain.com/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
        /// 
        ///     // returns "/test?param1=val1&amp;param2=val2&amp;param3=val3"
        ///     new Uri("/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
        /// </code>
        /// <param name="uri">Uri to extend</param>
        /// <param name="values">IDictionary with values</param>
        /// <returns>Uri with extended query</returns>
        public static Uri ExtendQuery<T>(this Uri uri, IDictionary<string, T> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            var keyValuePairs = uri.QueryToKeyValuePairs().Concat(values.Select(nameValue => new KeyValuePair<string, string>(nameValue.Key, nameValue.Value?.ToString())));

            var uriBuilder = new UriBuilder(uri)
            {
                Query = keyValuePairs.ToQueryString()
            };
            return uriBuilder.Uri;
        }

        /// <summary>
        ///     Adds query string value to an existing url, both absolute and relative URI's are supported.
        /// </summary>
        /// <code>
        ///     // returns "www.domain.com/test?param1=val1&amp;param2=val2&amp;param3=val3"
        ///     new Uri("www.domain.com/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
        /// 
        ///     // returns "/test?param1=val1&amp;param2=val2&amp;param3=val3"
        ///     new Uri("/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
        /// </code>
        /// <param name="uri">Uri to extend the query for</param>
        /// <param name="values">ILookup with values</param>
        /// <returns>Uri with extended query</returns>
        public static Uri ExtendQuery<T>(this Uri uri, ILookup<string, T> values)
        {
            var keyValuePairs = uri.QueryToKeyValuePairs().Concat(from kvp in values from value in kvp select new KeyValuePair<string, string>(kvp.Key, value?.ToString()));

            var uriBuilder = new UriBuilder(uri)
            {
                Query = keyValuePairs.ToQueryString()
            };
            return uriBuilder.Uri;
        }

        /// <summary>
        ///     Sets the userinfo of the Uri
        /// </summary>
        /// <param name="uri">Uri to extend</param>
        /// <param name="username">username of value</param>
        /// <param name="password">password for the user</param>
        /// <returns>Uri with extended query</returns>
        public static Uri SetCredentials(this Uri uri, string username, string password)
        {
            var uriBuilder = new UriBuilder(uri)
            {
                UserName = username,
                Password = password
            };
            return uriBuilder.Uri;
        }
    }
}
