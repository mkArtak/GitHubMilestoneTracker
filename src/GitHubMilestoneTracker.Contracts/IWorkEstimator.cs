using MilestoneTracker.Contracts.DTO;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Contracts
{
    public interface IWorkEstimator
    {
        Task<IEnumerable<WorkItem>> GetWorkItemsAsync(IssuesQuery query, CancellationToken cancellationToken);

        Task<IEnumerable<PR>> GetPullRequestsAsync(DateTimeOffset mergedOnOrAfter, CancellationToken cancellationToken);

        Task<BurndownDTO> GetBurndownDataAsync(IssuesQuery query, CancellationToken none);
    }
}
