using AM.Common.Validation;
using MilestoneTracker.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MIlestoneTracker.FileBasedTeamStore
{
    public class TeamsManager : ITeamsManager
    {
        private readonly string fileLocation;
        private readonly Task initTask;
        private IDictionary<string, TeamInfo> teams;

        public TeamsManager(string fileLocation)
        {
            this.fileLocation = fileLocation.Ensure(nameof(fileLocation)).IsNotNullOrWhitespace().Value;

            this.initTask = Task.Run(() =>
            {
                this.teams = this.ParseFile().ToDictionary(i => i.Name);
            });
        }

        public async Task AddTeamAsync(TeamInfo info, CancellationToken cancellationToken)
        {
            await this.initTask;

            this.teams.Add(info.Name, info);

            await AddTeamToStoreAsync(info);
        }

        public Task<PagedDataResponse<TeamInfo>> GetAllTeamsAsync(int count, CancellationToken cancellationToken, string continuationToken)
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

        private IEnumerable<TeamInfo> ParseFile()
        {
            using (StreamReader reader = new StreamReader(this.fileLocation))
            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<IEnumerable<TeamInfo>>(jsonReader);
            }
        }

        private Task AddTeamToStoreAsync(TeamInfo info)
        {
            return Task.Run(() =>
            {
                string path = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
                using (StreamWriter writer = new StreamWriter(path, true))
                using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
                {
                    JsonSerializer jsonSerializer = new JsonSerializer();
                    jsonSerializer.Serialize(jsonWriter, info);
                }
            });
        }
    }
}
