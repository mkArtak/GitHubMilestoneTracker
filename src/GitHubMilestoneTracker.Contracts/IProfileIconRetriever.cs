using System.Threading.Tasks;

namespace MilestoneTracker.Contracts
{
    public interface IProfileIconRetriever
    {
        Task<string> GetUserProfileIconUrl(string login);
    }
}
