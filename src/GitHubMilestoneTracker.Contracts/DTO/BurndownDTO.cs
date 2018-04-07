using System.Collections.Generic;

namespace MilestoneTracker.Contracts.DTO
{
    public class BurndownDTO
    {
        public IEnumerable<WorkDTO> WorkData { get; set; }
    }
}
