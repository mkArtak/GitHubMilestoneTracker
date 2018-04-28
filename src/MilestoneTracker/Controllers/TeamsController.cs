﻿using AM.Common.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilestoneTracker.Contracts;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MilestoneTracker.Controllers
{
    [Route("api/Teams")]
    [Authorize]
    public class TeamsController : Controller
    {
        private readonly IUserTeamsManager userTeamsManager;
        private readonly ITeamsManager teamsManager;
        private TeamInfo currentTeam;

        public TeamsController(
            ITeamsManager teamsManager,
            IUserTeamsManager userTeamsManager)
        {
            this.userTeamsManager = userTeamsManager.Ensure(nameof(userTeamsManager)).IsNotNull().Value;
            this.teamsManager = teamsManager.Ensure(nameof(teamsManager)).IsNotNull().Value;
        }

        [HttpGet("{teamName}")]
        public async Task<TeamInfo> GetTeamInfoAsync([FromQuery]string teamName)
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
            if (this.currentTeam == null)
            {
                this.currentTeam = await this.teamsManager.GetTeamInfoAsync(teamName, token);
            }

            return this.currentTeam;
        }
    }
}