using System.Collections.Generic;

namespace MT.DataManagement.Teams.AzureSql.Model
{
    public class Team
    {
        public string Name { get; set; }

        public Member Owner { get; set; }

        public string DefaultMilestonesToTrack { get; set; }

        public string Organization { get; set; }

        public IEnumerable<Member> Members { get; set; }

        public IEnumerable<Repo> Repos { get; set; }

        public IEnumerable<CostMarker> CostMarkers { get; set; }

    }
}
