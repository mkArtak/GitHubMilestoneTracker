using System;

namespace MilestoneTracker.Contracts.DTO
{
    public class WorkDTO
    {
        public DateTimeOffset Date { get; set; }

        public double DaysOfWorkLeft { get; set; }
    }
}
