using System.Collections.Generic;
using System.Linq;

namespace MilestoneTracker.Contracts.DTO
{
    public class BurndownDTO
    {
        public IEnumerable<WorkDTO> WorkData { get; set; }

        public int TotalNumberOfIssues { get; set; }

        public int NumberOfIssuesLeft { get; set; }
    }
}
