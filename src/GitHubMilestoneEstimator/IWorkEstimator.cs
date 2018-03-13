using System.Threading;
using System.Threading.Tasks;

namespace GitHub.Client
{
    public interface IWorkEstimator
    {
        Task<int> GetAmountOfWorkAsync(string assignee, string milestone, CancellationToken cancellationToken);
    }
}
