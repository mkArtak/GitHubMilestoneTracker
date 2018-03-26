using AM.Common.Validation;
using MilestoneTracker.Contracts;
using MT.DataManagement.Teams.AzureSql.Model;
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

        public async Task AddTeamAsync(string ownerLogin, TeamInfo info, CancellationToken cancellationToken)
        {
            ownerLogin.Ensure(nameof(ownerLogin)).IsNotNullOrWhitespace();
            info.Ensure(nameof(info)).IsNotNull();
            cancellationToken.ThrowIfCancellationRequested();

            Member owner = this.context.Members.Where(item => item.Login == ownerLogin).SingleOrDefault();
            if (owner == null)
            {
                owner = new Member
                {
                    Login = ownerLogin
                };
                await this.context.Members.AddAsync(owner, cancellationToken);
            }

            Team team = new Team
            {
                Name = info.Name,
                Owner = owner,
                CostMarkers = info.CostLabels.Select(item => Convert(item)),
                DefaultMilestonesToTrack = info.DefaultMilestonesToTrack,
                Organization = info.Organization,
                Repos = info.Repositories.Select(item => new Repo { Name = item })
            };
            await this.context.Teams.AddAsync(team, cancellationToken);

            // TODO: Add team members here
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

        private static MilestoneTracker.Contracts.CostMarker Convert(Model.CostMarker value) => new MilestoneTracker.Contracts.CostMarker
        {
            Factor = value.Factor,
            Name = value.Name
        };

        private Model.CostMarker Convert(MilestoneTracker.Contracts.CostMarker item) => new Model.CostMarker
        {
            Factor = item.Factor,
            Name = item.Name
        };
    }
}
