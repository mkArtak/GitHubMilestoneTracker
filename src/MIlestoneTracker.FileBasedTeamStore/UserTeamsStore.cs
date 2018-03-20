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
        }

        public Task<IEnumerable<string>> GetUserTeamsAsync(string userName, CancellationToken token)
        {
            return Task.FromResult<IEnumerable<string>>(this.Mappings[userName]);
        }
    }
}
