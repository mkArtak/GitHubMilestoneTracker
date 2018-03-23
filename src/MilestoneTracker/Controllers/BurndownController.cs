using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilestoneTracker.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MilestoneTracker.Controllers
{
    [Route("api/Burndown")]
    [Authorize]
    public class BurndownController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetBurndownDataAsync([FromQuery]string teamName, [FromQuery]string milestone)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidOperationException(ModelState.Values.First().Errors.First().ErrorMessage);
            }

            WorkDTO[] burnDownDatas = new[] {
                new WorkDTO { Date = new DateTime(2018, 01, 01), DaysOfWorkLeft = 10 },
                new WorkDTO { Date = new DateTime(2018, 01, 02), DaysOfWorkLeft = 9 },
                new WorkDTO { Date = new DateTime(2018, 01, 03), DaysOfWorkLeft = 10},
                new WorkDTO { Date = new DateTime(2018, 01, 04), DaysOfWorkLeft = 8 },
                new WorkDTO { Date = new DateTime(2018, 01, 06), DaysOfWorkLeft = 7},
                new WorkDTO { Date = new DateTime(2018, 01, 08), DaysOfWorkLeft = 5 }};
            await Task.CompletedTask;
            return new JsonResult(burnDownDatas);
        }
    }
}