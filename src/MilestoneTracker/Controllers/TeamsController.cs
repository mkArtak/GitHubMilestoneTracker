using AM.Common.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilestoneTracker.Contracts;
using MilestoneTracker.Model;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Controllers
{
    [Route("api/Teams")]
    [Authorize]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamsManager teamsManager;

        public TeamsController(ITeamsManager teamsManager)
        {
            this.teamsManager = teamsManager.Ensure(nameof(teamsManager)).IsNotNull().Value;
        }

        [HttpGet("{teamName}")]
        public async Task<TeamInfo> GetTeamInfoAsync([FromQuery(Name = QueryStringParameters.TeamName)]string teamName)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidOperationException(ModelState.Values.First().Errors.First().ErrorMessage);
            }

            TeamInfo team = await this.GetCurrentTeamAsync(teamName, CancellationToken.None);

            return team;
        }

        private async Task<TeamInfo> GetCurrentTeamAsync(string teamName, CancellationToken token)
        {
            TeamInfo result = await this.teamsManager.GetTeamInfoAsync(teamName, token);
            return result;
        }
    }
}