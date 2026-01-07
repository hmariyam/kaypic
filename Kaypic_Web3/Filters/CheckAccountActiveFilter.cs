using Kaypic_Web3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public class CheckAccountActiveFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var httpContext = context.HttpContext;
        var userId = httpContext.Session.GetInt32("UtilisateurID");

        if (userId != null)
        {
            var db = httpContext.RequestServices.GetService<MainDbContext>();
            var user = db.Utilisateurs.AsNoTracking().FirstOrDefault(u => u.CustomId == userId);

            if (user?.IsDeleted == true)
            {
                httpContext.Session.Clear();

                if (context.Controller is Controller controller)
                {
                    controller.TempData["AlertMessage"] = "Compte supprimé. Vous ne pouvez plus vous connecter.";
                }

                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
    public void OnActionExecuted(ActionExecutedContext context) { }
}