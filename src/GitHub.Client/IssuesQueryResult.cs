using Newtonsoft.Json;
using System.Collections.Generic;

namespace GitHub.Client
{
    public partial class IssuesQueryResult
    {
        [JsonProperty("total_count")]
        public int TotalCount { get; set; }


        [JsonProperty("incomplete_results")]
        public bool IncompleteResults { get; set; }

        public IEnumerable<Issue> Items { get; set; }
    }
}
