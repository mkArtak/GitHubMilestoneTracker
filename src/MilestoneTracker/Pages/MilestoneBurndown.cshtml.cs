using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MilestoneTracker.Model;

namespace MilestoneTracker.Pages
{
    public class MilestoneBurndownModel : PageModel
    {
        [FromQuery(Name = QueryStringParameters.Milestone)]
        public string Milestone { get; set; }

        [FromQuery(Name = QueryStringParameters.TeamName)]
        public string TeamName { get; set; }

        [FromQuery(Name = QueryStringParameters.Label)]
        public string Label { get; set; }

        [FromQuery(Name = QueryStringParameters.IncludeInvestigations)]
        public bool IncludeInvestigations { get; set; }

        public BurndownChartModel BurndownModel { get; set; }

        public void OnGet()
        {
            this.BurndownModel = new BurndownChartModel
            {
                Milestone = this.Milestone,
                TeamName = this.TeamName,
                LabelsFilter = this.Label,
                IncludeInvestigations = this.IncludeInvestigations
            };
        }
    }
}