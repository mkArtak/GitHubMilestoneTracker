using MilestoneTracker.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Model
{
    public class InMemoryTeamsManager : ITeamsManager, IUserTeamsManager
    {
        private IDictionary<string, TeamInfo> teams = new Dictionary<string, TeamInfo> {
            { "ASP.NET Core MVC",  new TeamInfo
                {
                    CostLabels = new[] {
                        new CostMarker { Name = "area-mvc", Factor = 1 },
                        new CostMarker { Name = "area-blazor", Factor = 1 }
                    },
                    FixedIssuesIndicatingLabel = "Done",
                    DefaultMilestonesToTrack = "3.0.0-preview8",
                    AreaLabels = new[] { "area-mvc", "area-blazor" },
                    Name = "ASP.NET Core MVC",
                    Organization = "aspnet",
                    Repositories = new[] { "aspnet/AspNetCore", "aspnet/Extensions" },
                    LabelsToExclude = new[] { "Validation", "duplicate", "external" },
                    TeamMembers = new[] {
                        "mkArtakMSFT",
                        "ajaybhargavb",
                        "javiercn",
                        "NTaylorMullen",
                        "pranavkm",
                        "dougbu",
                        "ryanbrandenburg",
                        "rynowak",
                        "SteveSandersonMS" }
                    .Select(name => new TeamMember { Name = name, IncludeInReports = true })
                    .Append(new TeamMember { Name = "mkArtak", IncludeInReports = false })
                    .ToArray()
                }
            },
            {
                "ASP.NET Core MVC Razor",  new TeamInfo
                {
                    CostLabels = new[] {
                        new CostMarker { Name = "area-mvc", Factor = 1 },
                        new CostMarker { Name = "area-blazor", Factor = 1 }
                    },
                    FixedIssuesIndicatingLabel = "Done",
                    DefaultMilestonesToTrack = "3.0.0-preview8",
                    AreaLabels = null,
                    Name = "ASP.NET Core MVC Razor",
                    Organization = "aspnet",
                    Repositories = new[] { "aspnet/AspNetCore-Tooling" },
                    LabelsToExclude = new[] { "Validation", "duplicate", "external" },
                    TeamMembers = new[] {
                        "mkArtakMSFT",
                        "ajaybhargavb",
                        "javiercn",
                        "NTaylorMullen",
                        "pranavkm",
                        "dougbu",
                        "ryanbrandenburg",
                        "rynowak",
                        "SteveSandersonMS" }
                    .Select(name => new TeamMember { Name = name, IncludeInReports = true })
                    .Append(new TeamMember { Name = "mkArtak", IncludeInReports = false })
                    .ToArray()
                }
            },
        };

        public Task AddMemberAsync(string teamName, TeamMember member, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AddTeamAsync(string ownerLogin, TeamInfo info, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TeamInfo> GetTeamInfoAsync(string teamName, CancellationToken cancellationToken)
        {
            if (!this.teams.TryGetValue(teamName, out TeamInfo team))
            {
                throw new ArgumentException($"Team `{teamName}` is not configured");
            }

            return Task.FromResult(team);
        }

        public async Task<TeamInfo> GetTeamInfoAsync(string userName, string teamName, CancellationToken token)
        {
            var team = await GetTeamInfoAsync(teamName, token);
            if (team != null && team.TeamMembers.Any(item => item.Name == userName))
            {
                return team;
            }

            return null;
        }

        public Task<PagedDataResponse<TeamInfo>> GetTeamsAsync(int count, CancellationToken cancellationToken, string continuationToken)
        {
            return Task.FromResult(new PagedDataResponse<TeamInfo>(this.teams.Values, null));
        }

        public Task<IEnumerable<string>> GetUserTeamsAsync(string userName, CancellationToken token)
        {
            var teams = this.teams.Values.Where(t => t.TeamMembers.Any(item => item.Name == userName)).Select(t => t.Name);
            return Task<IEnumerable<string>>.FromResult(teams);
        }

        public Task RemoveTeamMemberAsync(string teamName, string memberName, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
