using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Contracts
{
    public interface IWorkEstimator
    {
        Task<double> GetAmountOfWorkAsync(string assignee, string milestone, CancellationToken cancellationToken);
    }
}
