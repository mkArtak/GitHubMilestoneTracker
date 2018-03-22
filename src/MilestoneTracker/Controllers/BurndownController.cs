using Microsoft.AspNetCore.Mvc;
using MilestoneTracker.Model;
using System;
using System.Threading.Tasks;

namespace MilestoneTracker.Controllers
{
    [Route("api/Burndown")]
    public class BurndownController : Controller
    {
        [HttpGet]
        public IActionResult GetBurndown()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> GetBurndownDataAsync([FromQuery]string teamName/*, [FromBody]string[] milestones*/)
        {
            string milestone1 = "2.1.0-preview2";
            WorkDTO[] burnDownDatas = new[] {
                new WorkDTO { Date = new DateTime(2018, 01, 01), Milestone = milestone1, DaysOfWorkLeft = 10 },
                new WorkDTO { Date = new DateTime(2018, 01, 02), Milestone = milestone1, DaysOfWorkLeft = 9 },
                new WorkDTO { Date = new DateTime(2018, 01, 03), Milestone = milestone1, DaysOfWorkLeft = 10},
                new WorkDTO { Date = new DateTime(2018, 01, 04), Milestone = milestone1, DaysOfWorkLeft = 7 }};
            await Task.CompletedTask;
            return new JsonResult(burnDownDatas);
        }
    }
}