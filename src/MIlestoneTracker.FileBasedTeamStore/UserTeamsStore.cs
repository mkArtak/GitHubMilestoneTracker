using AM.Common.Validation;
using Microsoft.Extensions.Options;
using MilestoneTracker.Contracts;
using MilestoneTracker.DataManagement.Teams.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.DataManagement.Teams
{
    internal class UserTeamsStore : IUserTeamsManager
    {
        private readonly UserTeamsOptions mappingsOptions;

        public IDictionary<string, IEnumerable<string>> Mappings { get => this.mappingsOptions.Mappings; }

        public UserTeamsStore(IOptions<UserTeamsOptions> options)
        {
            options.Ensure(o => o.Value).IsNotNull();

            this.mappingsOptions = options.Value;

            /// TODO: remove - a temp around
            if (this.mappingsOptions.Mappings == null)
            {
                this.mappingsOptions = new UserTeamsOptions { Mappings = new Dictionary<string, IEnumerable<string>>() };
                this.mappingsOptions.Mappings.Add("mkArtakMSFT", new[] { "ASP.NET MVC" });
            }
        }

        public Task<IEnumerable<string>> GetUserTeamsAsync(string userName, CancellationToken token)
        {
            IEnumerable<string> teamNames;
            if (!this.Mappings.TryGetValue(userName, out teamNames))
            {
                teamNames = null;
            }

            return Task.FromResult<IEnumerable<string>>(teamNames);
        }

        public Task<TeamInfo> GetTeamInfoAsync(string userName, string teamName, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }
    }
}
