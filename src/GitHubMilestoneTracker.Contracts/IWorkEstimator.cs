using MilestoneTracker.Contracts.DTO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Contracts
{
    public interface IWorkEstimator
    {
        Task<IEnumerable<WorkItem>> GetAmountOfWorkAsync(TeamInfo team, string milestone, IEnumerable<string> labelsFilter, CancellationToken cancellationToken);

        Task<BurndownDTO> GetBurndownDataAsync(TeamInfo team, string milestone, IEnumerable<string> labelsFilter, CancellationToken none);
    }
}
