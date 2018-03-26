using System.Collections.Generic;

namespace MT.DataManagement.Teams.AzureSql.Model
{
    public class Member
    {
        public string Login { get; set; }

        public IEnumerable<Team> OwnedTeams { get; set; }
    }
}
