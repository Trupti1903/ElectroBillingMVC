using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

public class BaseController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Allow pages like Login, Start
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata
            .Any(em => em is AllowAnonymousAttribute);

        if (allowAnonymous)
        {
            return;
        }

        // Check session
        var user = context.HttpContext.Session.GetString("AdminEmail");

        if (string.IsNullOrEmpty(user))
        {
            context.Result = new RedirectToActionResult("Login", "Billing", null);
        }
    }
}