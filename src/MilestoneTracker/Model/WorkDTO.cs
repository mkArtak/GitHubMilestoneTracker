using System;

namespace MilestoneTracker.Model
{
    public class WorkDTO
    {
        public string Milestone { get; set; }

        public DateTime Date { get; set; }

        public double DaysOfWorkLeft { get; set; }
    }
}
