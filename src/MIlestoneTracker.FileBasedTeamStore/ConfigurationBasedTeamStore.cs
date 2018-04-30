using AM.Common.Validation;
using Microsoft.Extensions.Options;
using MilestoneTracker.Contracts;
using MilestoneTracker.DataManagement.Teams.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.DataManagement.Teams
{
    internal class ConfigurationBasedTeamStore : ITeamsManager
    {
        private readonly TeamsConfigurationOption teamConfiguration;
        private IDictionary<string, TeamInfo> teams;

        public ConfigurationBasedTeamStore(IOptions<TeamsConfigurationOption> options)
        {
            options.Ensure(o => o.Value).IsNotNull();
            this.teamConfiguration = options.Value;

            this.teams = this.teamConfiguration.Teams.ToDictionary(i => i.Name);
        }

        public Task AddTeamAsync(string ownerLogin, TeamInfo info, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<PagedDataResponse<TeamInfo>> GetTeamsAsync(int count, CancellationToken cancellationToken, string continuationToken)
        {
            return Task.FromResult<PagedDataResponse<TeamInfo>>(
                new PagedDataResponse<TeamInfo>(
                    this.teams.Values
                        .Skip(continuationToken == null ? 0 : Int32.Parse(continuationToken))
                        .Take(count),
                    continuationToken));
        }

        public Task<TeamInfo> GetTeamInfoAsync(string teamName, CancellationToken cancellationToken)
        {
            teamName.Ensure(nameof(teamName)).IsNotNullOrWhitespace();
            cancellationToken.ThrowIfCancellationRequested();

            if (this.teams.TryGetValue(teamName, out TeamInfo teamInfo))
            {
                return Task.FromResult<TeamInfo>(teamInfo);
            }

            return Task.FromResult<TeamInfo>(null);
        }

        public Task AddMemberAsync(string teamName, TeamMember member, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
