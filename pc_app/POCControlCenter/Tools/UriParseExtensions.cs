using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace POCControlCenter
{
    /// <summary>
    ///     Uri extensions which help with parsing
    /// </summary>
    public static class UriParseExtensions
    {
        /// <summary>
        ///     Query-string To Dictionary creates a IDictionary
        /// </summary>
        /// <param name="queryString">query string which is processed</param>
        /// <returns>IDictionary string, string</returns>
        public static IDictionary<string, string> QueryStringToDictionary(string queryString)
        {
            var parameters = new SortedDictionary<string, string>();
            foreach (var keyValuePair in QueryStringToKeyValuePairs(queryString))
            {
                if (parameters.ContainsKey(keyValuePair.Key))
                {
                    parameters[keyValuePair.Key] = keyValuePair.Value;
                }
                else
                {
                    parameters.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
            return parameters;
        }

        /// <summary>
        ///     Query-string To KeyValuePairs creates a List with KeyValuePair which have the name-values
        /// </summary>
        /// <param name="queryString">query string which is processed</param>
        /// <returns>List KeyValuePair string, string</returns>
        public static IEnumerable<KeyValuePair<string, string>> QueryStringToKeyValuePairs(string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
            {
                yield break;
            }
            // remove starting ? from query-string if needed
            if (queryString.StartsWith("?"))
            {
                queryString = queryString.Substring(1);
            }
            foreach (var vp in Regex.Split(queryString, "&"))
            {
                if (string.IsNullOrEmpty(vp))
                {
                    continue;
                }
                var singlePair = Regex.Split(vp, "=");
                var name = singlePair[0];
                string value = null;
                if (singlePair.Length == 2)
                {
                    value = Uri.UnescapeDataString(singlePair[1]);
                }
                yield return new KeyValuePair<string, string>(name, value);
            }
        }

        /// <summary>
        ///     QueryToDictionary creates a IDictionary with name-values
        /// </summary>
        /// <param name="uri">Uri of which the query is processed</param>
        /// <returns>IDictionary string, string</returns>
        public static IDictionary<string, string> QueryToDictionary(this Uri uri)
        {
            var parameters = new SortedDictionary<string, string>();
            foreach (var keyValuePair in uri.QueryToKeyValuePairs())
            {
                if (parameters.ContainsKey(keyValuePair.Key))
                {
                    parameters[keyValuePair.Key] = keyValuePair.Value;
                }
                else
                {
                    parameters.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
            return parameters;
        }

        /// <summary>
        ///     QueryToKeyValuePairs creates a List with KeyValuePair which have the name-values
        /// </summary>
        /// <param name="uri">Uri of which the query is processed</param>
        /// <returns>List KeyValuePair string, string</returns>
        public static IEnumerable<KeyValuePair<string, string>> QueryToKeyValuePairs(this Uri uri)
        {
            return QueryStringToKeyValuePairs(uri?.Query);
        }

        /// <summary>
        ///     QueryToLookup creates a ILookup with name-values
        /// </summary>
        /// <param name="uri">Uri of which the query is processed</param>
        /// <returns>ILookup string, string</returns>
        public static ILookup<string, string> QueryToLookup(this Uri uri)
        {
            var parameters = uri.QueryToKeyValuePairs();
            return parameters.ToLookup(k => k.Key, e => e.Value);
        }
    }
}
