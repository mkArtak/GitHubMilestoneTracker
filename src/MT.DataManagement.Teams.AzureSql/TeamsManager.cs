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

            Member owner = this.context.Members.Where(item => item.MemberId == ownerLogin).SingleOrDefault();
            if (owner == null)
            {
                owner = new Member
                {
                    MemberId = ownerLogin
                };
                await this.context.Members.AddAsync(owner, cancellationToken);
            }

            Team team = new Team
            {
                TeamId = info.Name,
                CostMarkers = info.CostLabels.Select(item => Convert(item)),
                DefaultMilestonesToTrack = info.DefaultMilestonesToTrack,
                Organization = info.Organization,
                Repos = info.Repositories.Select(item => new Repo { RepoId = item }),
                Members = info.TeamMembers?.Select(item => new Member { MemberId = item })
            };
            await this.context.Teams.AddAsync(team, cancellationToken);

            // TODO: Add team members here
        }

        public Task<TeamInfo> GetTeamInfoAsync(string teamName, CancellationToken cancellationToken)
        {
            teamName.Ensure(nameof(teamName)).IsNotNullOrWhitespace();
            cancellationToken.ThrowIfCancellationRequested();

            TeamInfo result = null;

            var team = this.context.Teams.Where(item => item.TeamId == teamName).SingleOrDefault();
            if (team != null)
            {
                result = new TeamInfo
                {
                    CostLabels = team.CostMarkers?.Select(item => Convert(item)).ToArray(),
                    DefaultMilestonesToTrack = team.DefaultMilestonesToTrack,
                    Name = team.TeamId,
                    Organization = team.Organization,
                    Repositories = team.Repos?.Select(item => item.RepoId).ToArray(),
                    TeamMembers = team.Members?.Select(item => item.MemberId).ToArray()
                };
            };

            return Task.FromResult(result);
        }

        public Task<PagedDataResponse<TeamInfo>> GetTeamsAsync(int count, CancellationToken cancellationToken, string continuationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> GetUserTeamsAsync(string userName, CancellationToken token)
        {
            await Task.CompletedTask;
            return this.context.Members.Where(item => item.MemberId == userName).SelectMany(item => item.Teams).Select(item => item.TeamId);
        }

        private static MilestoneTracker.Contracts.CostMarker Convert(Model.CostMarker value) => new MilestoneTracker.Contracts.CostMarker
        {
            Factor = value.Factor,
            Name = value.CostMarkerId
        };

        private Model.CostMarker Convert(MilestoneTracker.Contracts.CostMarker item) => new Model.CostMarker
        {
            Factor = item.Factor,
            CostMarkerId = item.Name
        };
    }
}
