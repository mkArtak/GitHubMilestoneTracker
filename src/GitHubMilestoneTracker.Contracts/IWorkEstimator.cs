using MilestoneTracker.Contracts.DTO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Contracts
{
    public interface IWorkEstimator
    {
        Task<IEnumerable<WorkItem>> GetWorkItemsAsync(IssuesQuery query, CancellationToken cancellationToken);

        Task<IEnumerable<PR>> GetPullRequestsAsync(IssuesQuery query, CancellationToken cancellationToken);

        Task<BurndownDTO> GetBurndownDataAsync(IssuesQuery query, CancellationToken none);

        Task<TeamInfo> GetTeamUserIcons();
    }
}
