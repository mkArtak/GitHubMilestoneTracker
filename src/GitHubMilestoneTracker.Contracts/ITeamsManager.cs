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
        /// <param name="ownerLogin">The login of the owner member</param>
        /// <param name="info">The team data.</param>
        /// <param name="cancellationToken">A cancellation token, which will allow the caller to request cancellation.</param>
        /// <returns>A Task instance representing the requested asynchronous operation.</returns>
        Task AddTeamAsync(string ownerLogin, TeamInfo info, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new member to the team.
        /// </summary>
        /// <param name="teamName">The name of the team to add member to.</param>
        /// <param name="member">The identity of the member.</param>
        /// <param name="cancellationToken">A cancellation token, which will allow the caller to request cancellation.</param>
        /// <returns>A Task instance representing the requested asynchronous operation.</returns>
        Task AddMemberAsync(string teamName, TeamMember member, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the specified <see cref="memberName"/> from the <see cref="teamName"/> team.
        /// </summary>
        /// <param name="teamName">The name of the team to remove the specified member from.</param>
        /// <param name="memberName">The name of the member to remove from the specified team.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A Task instance representing the requested asynchronous operation.</returns>
        Task RemoveTeamMemberAsync(string teamName, string memberName, CancellationToken token);
    }
}
