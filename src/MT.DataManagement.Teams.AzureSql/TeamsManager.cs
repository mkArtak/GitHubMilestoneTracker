using AM.Common.Validation;
using Microsoft.EntityFrameworkCore;
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
            throw new NotImplementedException();
            //ownerLogin.Ensure(nameof(ownerLogin)).IsNotNullOrWhitespace();
            //info.Ensure(nameof(info)).IsNotNull();
            //cancellationToken.ThrowIfCancellationRequested();

            //Member owner = this.context.Members.Where(item => item.MemberId == ownerLogin).SingleOrDefault();
            //if (owner == null)
            //{
            //    owner = new Member
            //    {
            //        MemberId = ownerLogin
            //    };
            //    await this.context.Members.AddAsync(owner, cancellationToken);
            //}

            //Team team = new Team
            //{
            //    TeamId = info.Name,
            //    CostMarkers = info.CostLabels.Select(item => Convert(item)).ToList(),
            //    DefaultMilestonesToTrack = info.DefaultMilestonesToTrack,
            //    Organization = info.Organization,
            //};
            //if (info.Repositories != null)
            //{
            //    foreach (var r in info.Repositories)
            //    {
            //        team.Repos.Add(new Repo { RepoId = r });
            //    }
            //}

            //if (info.TeamMembers != null)
            //{
            //    foreach (var member in info.TeamMembers)
            //    {
            //        team.Members.Add(new Member { MemberId = member.Name });
            //    }
            //    // TODO: Rework this part to allow storing the "IncludeInReporting" field for members.
            //}

            //await this.context.Teams.AddAsync(team, cancellationToken);
        }

        public async Task<TeamInfo> GetTeamInfoAsync(string teamName, CancellationToken cancellationToken)
        {
            teamName.Ensure(nameof(teamName)).IsNotNullOrWhitespace();
            cancellationToken.ThrowIfCancellationRequested();

            TeamInfo result = null;

            var team = this.context.Teams.Include(t => t.CostMarkers).Where(item => item.TeamId == teamName);
            if (team != null)
            {
                result = await ToTeamInfoAsync(team);
                PopulateRelationProperties(teamName, result);
            }

            return result;
        }

        public Task<PagedDataResponse<TeamInfo>> GetTeamsAsync(int count, CancellationToken cancellationToken, string continuationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> GetUserTeamsAsync(string userName, CancellationToken token)
        {
            var f = this.context.TeamMembers.First();
            Console.WriteLine(f);
            return await Task.Run(() => this.context.TeamMembers.Where(t => t.MemberId == userName).Select(t => t.TeamId));
        }

        public async Task<TeamInfo> GetTeamInfoAsync(string userName, string teamName, CancellationToken token)
        {
            TeamInfo result = null;

            if (await this.context.TeamMembers
                 .AnyAsync(t => t.TeamId == teamName && t.MemberId == userName))
            {
                var r = this.context.Teams
                    .Include("TeamMembers")
                    .Include("TeamRepos")
                    .Include(t => t.CostMarkers)
                    .Where(t => t.TeamId == teamName);

                result = await ToTeamInfoAsync(r);
                PopulateRelationProperties(teamName, result);
            }

            return result;
        }

        public async Task AddMemberAsync(string teamName, MilestoneTracker.Contracts.TeamMember member, CancellationToken cancellationToken)
        {
            teamName.Ensure(nameof(teamName)).IsNotNullOrWhitespace();
            member.Ensure(nameof(member)).IsNotNull();
            cancellationToken.ThrowIfCancellationRequested();

            if (!await this.context.Members.AnyAsync(m => m.MemberId == member.Name))
            {
                this.context.Members.Add(new Member { MemberId = member.Name });
            }

            this.context.TeamMembers.Add(new Model.TeamMember
            {
                IncludeInReports = member.IncludeInReports,
                MemberId = member.Name,
                TeamId = teamName
            });

            await this.context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveTeamMemberAsync(string teamName, string memberName, CancellationToken token)
        {
            teamName.Ensure(nameof(teamName)).IsNotNullOrWhitespace();
            memberName.Ensure(nameof(memberName)).IsNotNullOrWhitespace();

            token.ThrowIfCancellationRequested();

            Model.TeamMember member = await this.context.TeamMembers.SingleOrDefaultAsync(m => m.TeamId == teamName && m.MemberId == memberName);
            this.context.TeamMembers.Remove(member);
            await this.context.SaveChangesAsync(token);
        }

        private void PopulateRelationProperties(string teamName, TeamInfo result)
        {
            throw new NotImplementedException();
            //result.TeamMembers = this.context.TeamMembers.Where(tm => tm.TeamId == teamName).Select(tm => new MilestoneTracker.Contracts.TeamMember { Name = tm.MemberId, IncludeInReports = tm.IncludeInReports }).ToArray();
            //result.Repos = this.context.TeamRepos.Where(tr => tr.TeamId == teamName).Select(tr => tr.RepoId).ToArray();
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

        private static async Task<TeamInfo> ToTeamInfoAsync(IQueryable<Team> teamQuery)
        {
            //Team team = await teamQuery.SingleAsync();
            //var result = new TeamInfo
            //{
            //    CostLabels = team.CostMarkers?.Select(item => Convert(item))?.ToArray(),
            //    DefaultMilestonesToTrack = team.DefaultMilestonesToTrack,
            //    Name = team.TeamId,
            //    Organization = team.Organization,
            //};

            //return result;
            throw new NotImplementedException();
        }
    }
}
