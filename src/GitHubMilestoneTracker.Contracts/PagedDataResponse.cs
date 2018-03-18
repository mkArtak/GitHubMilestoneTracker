using AM.Common.Validation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MilestoneTracker.Contracts
{
    public class PagedDataResponse<T>
    {
        private readonly IEnumerable<T> results;
        private readonly string continuationToken;

        public IEnumerable<T> Results { get => this.results; }

        public string ContinuationToken { get => this.continuationToken; }

        public PagedDataResponse(IEnumerable<T> results, string continuationToken)
        {
            this.results = new ReadOnlyCollection<T>(results.Ensure(nameof(results)).IsNotNull().Value.ToList());
            this.continuationToken = continuationToken;
        }
    }
}
