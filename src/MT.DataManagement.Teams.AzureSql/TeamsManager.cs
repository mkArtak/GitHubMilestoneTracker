using AM.Common.Validation;
using MilestoneTracker.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MT.DataManagement.Teams.AzureSql
{
    internal class TeamsManager : ITeamsManager, IUserTeamsManager
    {
        private readonly MilestoneAnalyticsDataContext context;

        public TeamsManager(MilestoneAnalyticsDataContext context)
        {
            context.Ensure().IsNotNull();

            this.context = context;

        }

        public Task AddTeamAsync(TeamInfo info, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<TeamInfo> GetTeamInfoAsync(string teamName, CancellationToken cancellationToken)
        {
            teamName.Ensure(nameof(teamName)).IsNotNullOrWhitespace();
            cancellationToken.ThrowIfCancellationRequested();

            TeamInfo result = null;

            var team = await this.context.Teams.FindAsync(teamName, cancellationToken);
            if (team != null)
            {
                result = new TeamInfo
                {
                    CostLabels = team.CostMarkers.Select(item => Convert(item)).ToArray(),
                    DefaultMilestonesToTrack = team.DefaultMilestonesToTrack,
                    Name = team.Name,
                    Organization = team.Organization,
                    Repositories = team.Repos.Select(item => item.Name).ToArray(),
                    TeamMembers = team.Members.Select(item => item.Login).ToArray()
                };
            };

            return result;
        }

        public Task<PagedDataResponse<TeamInfo>> GetTeamsAsync(int count, CancellationToken cancellationToken, string continuationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> GetUserTeamsAsync(string userName, CancellationToken token)
        {
            await Task.CompletedTask;
            return this.context.Teams.Where(item => item.Owner.Login == userName).Select(item => item.Name);
        }

        private static CostMarker Convert(Model.CostMarker value) => new CostMarker
        {
            Factor = value.Factor,
            Name = value.Name
        };
    }
}
