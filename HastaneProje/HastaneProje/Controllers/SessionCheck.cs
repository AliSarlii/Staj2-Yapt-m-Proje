using System.Web;
using System.Web.Mvc;

public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var session = filterContext.HttpContext.Session["e-posta"];

        // Eğer session boşsa, kullanıcıyı giriş sayfasına yönlendir
        if (session == null)
        {
            filterContext.Result = new RedirectToRouteResult(
                new System.Web.Routing.RouteValueDictionary
                {
                    { "controller", "Home" },
                    { "action", "Giris" }
                });
        }
        base.OnActionExecuting(filterContext);
    }
}