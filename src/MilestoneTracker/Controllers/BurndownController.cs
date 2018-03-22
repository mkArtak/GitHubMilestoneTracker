using Microsoft.AspNetCore.Mvc;
using MilestoneTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilestoneTracker.Controllers
{
    [Route("api/Burndown")]
    public class BurndownController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> GetBurndownDataAsync([FromQuery]string teamName, [FromBody]IEnumerable<string> milestones)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidOperationException(ModelState.Values.First().Errors.First().ErrorMessage);
            }

            string milestone = milestones.FirstOrDefault();
            WorkDTO[] burnDownDatas = new[] {
                new WorkDTO { Date = new DateTime(2018, 01, 01), Milestone = milestone, DaysOfWorkLeft = 10 },
                new WorkDTO { Date = new DateTime(2018, 01, 02), Milestone = milestone, DaysOfWorkLeft = 9 },
                new WorkDTO { Date = new DateTime(2018, 01, 03), Milestone = milestone, DaysOfWorkLeft = 10},
                new WorkDTO { Date = new DateTime(2018, 01, 04), Milestone = milestone, DaysOfWorkLeft = 8 },
                new WorkDTO { Date = new DateTime(2018, 01, 06), Milestone = milestone, DaysOfWorkLeft = 7},
                new WorkDTO { Date = new DateTime(2018, 01, 08), Milestone = milestone, DaysOfWorkLeft = 5 }};
            await Task.CompletedTask;
            return new JsonResult(burnDownDatas);
        }
    }
}