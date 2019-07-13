using System.Collections.Generic;

namespace GitHubMilestoneEstimator.Extensions
{
    public static class DictionaryExtensions
    {
        public static IEnumerable<V> FlattenValues<K, V>(this IDictionary<K, IEnumerable<V>> dictionary)
        {
            IList<V> result = new List<V>();

            foreach (var list in dictionary.Values)
            {
                foreach (var listItem in list)
                {
                    result.Add(listItem);
                }
            }

            return result;
        }
    }
}
