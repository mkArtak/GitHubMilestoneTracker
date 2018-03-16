namespace MilestoneTracker.Contracts
{
    public class TeamInfo
    {
        public string Organization { get; set; }

        public string[] TeamMembers { get; set; }

        public CostMarker[] CostLabels { get; set; }

        public string DefaultMilestonesToTrack { get; set; }
    }
}
