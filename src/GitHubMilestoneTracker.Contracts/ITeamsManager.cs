using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Contracts
{
    public interface ITeamsManager
    {
        /// <summary>
        /// Retrieves the details of the specified team.
        /// </summary>
        /// <param name="teamName">The team name</param>
        /// <returns>The team info</returns>
        Task<TeamInfo> GetTeamInfoAsync(string teamName, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the list of all the teams.
        /// </summary>
        /// <param name="pageSize">The maximum number of records to retrieve</param>
        /// <param name="continuationToken">An optional continuation token to retrieve next set of the results from the previous query.</param>
        /// <param name="cancellationToken">A cancellation token, which will allow the caller to request cancellation.</param>
        /// <returns>A tuple </returns>
        Task<PagedDataResponse<TeamInfo>> GetTeamsAsync(int count, CancellationToken cancellationToken, string continuationToken);

        /// <summary>
        /// Registeres data of a new team.
        /// </summary>
        /// <param name="info">The team data.</param>
        /// <param name="continuationToken">An optional continuation token to retrieve next set of the results from the previous query.</param>
        /// <returns>A Task instance representing the requested asynchronous operation.</returns>
        Task AddTeamAsync(TeamInfo info, CancellationToken cancellationToken);
    }
}
