namespace MilestoneTracker.Model
{
    public class BurndownChartModel
    {
        public string Milestone { get; set; }

        public string TeamName { get; set; }

        public string LabelsFilter { get; set; }

        public bool IncludeInvestigations { get; set; }
    }
}
