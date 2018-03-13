using System.Threading;
using System.Threading.Tasks;

namespace GitHub.Client
{
    public interface IWorkEstimator
    {
        Task<double> GetAmountOfWorkAsync(string assignee, string milestone, CancellationToken cancellationToken);
    }
}
