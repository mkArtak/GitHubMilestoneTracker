using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Contracts
{
    public interface IUserTeamsManager
    {
        Task<IEnumerable<string>> GetUserTeamsAsync(string userName, CancellationToken token);

        Task<TeamInfo> GetTeamInfoAsync(string userName, string teamName, CancellationToken token);
    }
}
