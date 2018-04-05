using System.ComponentModel.DataAnnotations;

namespace MT.DataManagement.Teams.AzureSql.Model
{
    public class CostMarker
    {
        [Key]
        public string CostMarkerId { get; set; }

        [Key]
        public string TeamId { get; set; }

        public double Factor { get; set; }
    }
}
