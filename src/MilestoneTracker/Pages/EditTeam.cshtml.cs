using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MilestoneTracker.Contracts;

namespace MilestoneTracker.Pages
{
    public class EditTeamModel : PageModel
    {
        public TeamInfo Team { get; private set; }

        public void OnGet()
        {

        }
    }
}