using MilestoneTracker.Contracts.DTO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Contracts
{
    public interface IWorkEstimator
    {
        Task<IEnumerable<WorkItem>> GetAmountOfWorkAsync(IssuesQuery query, CancellationToken cancellationToken);

        Task<BurndownDTO> GetBurndownDataAsync(IssuesQuery query, CancellationToken none);
    }
}
