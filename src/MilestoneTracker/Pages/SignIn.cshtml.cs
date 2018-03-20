using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MilestoneTracker.Extensions;
using System.Threading.Tasks;

namespace MilestoneTracker.Pages
{
    public class SignInModel : PageModel
    {
        public AuthenticationScheme[] Providers
        {
            get; private set;
        }

        public async Task<IActionResult> OnGet()
        {
            this.Providers = await HttpContext.GetExternalProvidersAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromForm]string provider)
        {
            // Note: the "provider" parameter corresponds to the external
            // authentication provider choosen by the user agent.
            if (string.IsNullOrWhiteSpace(provider))
            {
                return new BadRequestResult();
            }

            if (!await HttpContext.IsProviderSupportedAsync(provider))
            {
                return new BadRequestResult();
            }

            // Instruct the middleware corresponding to the requested external identity
            // provider to redirect the user agent to its own authorization endpoint.
            // Note: the authenticationScheme parameter must match the value configured in Startup.cs
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/"
            }, provider);
        }
    }
}