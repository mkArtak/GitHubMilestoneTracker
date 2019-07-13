using MilestoneTracker.Contracts;
using MilestoneTracker.Contracts.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Model
{
    public class InMemoryTeamsManager : ITeamsManager, IUserTeamsManager
    {
        private TeamInfo _team = new TeamInfo
        {
            DefaultMilestonesToTrack = "3.0.0-preview8",
            Name = "ASP.NET Core MVC",
            Organization = "aspnet",
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
            .ToArray(),
            Repos = new[] {
                new Repository{
                    Name = "aspnet/AspNetCore",
                    CostLabels=new[] {
                        new CostMarker { Name = "area-mvc", Factor = 1 },
                        new CostMarker { Name = "area-blazor", Factor = 1 }
                    },
                    FixedIssueLabel = "Done",
                    PRRules = new Rules{ LabelsToInclude = new[]{ "area-mvc", "area-blazor" } },
                    RepoRules = new Rules {
                        LabelsToInclude = new[]{ "area-mvc", "area-blazor" },
                        LabelsToExclude =new[]{"external", "duplicate", "Validation" }
                    }
                }
            }
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
            if (teamName != _team.Name)
            {
                throw new ArgumentException($"Team `{teamName}` is not configured");
            }

            return Task.FromResult(_team);
        }

        public Task<TeamInfo> GetTeamInfoAsync(string userName, string teamName, CancellationToken token)
        {
            if (_team.Name == teamName && _team.TeamMembers.Any(item => item.Name == userName))
            {
                return Task.FromResult(_team);
            }

            return Task.FromResult<TeamInfo>(null);
        }

        public Task<PagedDataResponse<TeamInfo>> GetTeamsAsync(int count, CancellationToken cancellationToken, string continuationToken)
        {
            return Task.FromResult(new PagedDataResponse<TeamInfo>(new[] { _team }, null));
        }

        public Task<IEnumerable<string>> GetUserTeamsAsync(string userName, CancellationToken token)
        {
            if (_team.TeamMembers.Any(item => item.Name == userName))
            {
                return Task.FromResult((IEnumerable<string>)new[] { _team.Name });
            }

            return Task.FromResult<IEnumerable<string>>(null);
        }

        public Task RemoveTeamMemberAsync(string teamName, string memberName, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
