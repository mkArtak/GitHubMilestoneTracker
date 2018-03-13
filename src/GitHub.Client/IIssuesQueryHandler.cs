using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GitHub.Client
{
    public interface IIssuesQueryHandler
    {
        Task<IEnumerable<Issue>> QueryAsync(string assignee, string milestone, CancellationToken cancellationToken);
    }
}
