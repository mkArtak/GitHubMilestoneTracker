using System.Diagnostics;

namespace MilestoneTracker.Contracts
{
    [DebuggerDisplay("Id = {Id} Owner = {Owner} Cost = {Cost}")]
    public class WorkItem
    {
        public string Owner { get; set; }

        public double Cost { get; set; }

        public int Id { get; set; }
    }
}
