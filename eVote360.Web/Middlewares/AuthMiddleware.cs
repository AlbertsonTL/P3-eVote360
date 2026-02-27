namespace eVote360.Web.Middlewares;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";
        var userRole = context.Session.GetString("UserRole");
        var userId = context.Session.GetInt32("UserId");
        bool isAuthenticated = userId.HasValue && !string.IsNullOrEmpty(userRole);

        // Allow static files, public pages
        bool isPublicPath = path.StartsWith("/css") || path.StartsWith("/js") ||
                            path.StartsWith("/images") || path.StartsWith("/lib") ||
                            path.Contains(".") || // static files
                            path == "/" || path == "/home/index" || path == "/home/privacy" ||
                            path.StartsWith("/login") || path.StartsWith("/cuenta/iniciarsesion");

        if (isPublicPath)
        {
            await _next(context);
            return;
        }

        // Admin area protection
        if (path.StartsWith("/admin"))
        {
            if (!isAuthenticated)
            {
                context.Response.Redirect("/Login/Index");
                return;
            }
            if (userRole != "Administrador")
            {
                context.Response.Redirect("/Cuenta/Denegado");
                return;
            }
        }

        // Dirigente area protection
        if (path.StartsWith("/dirigente"))
        {
            if (!isAuthenticated)
            {
                context.Response.Redirect("/Login/Index");
                return;
            }
            if (userRole != "Dirigente")
            {
                context.Response.Redirect("/Cuenta/Denegado");
                return;
            }
        }

        // Elector (voting) path: redirect authenticated admin/dirigente to their dashboard
        if (path.StartsWith("/elector") || path == "/home/index" || path == "/")
        {
            if (isAuthenticated && userRole == "Administrador")
            {
                context.Response.Redirect("/Admin/Home/Index");
                return;
            }
            if (isAuthenticated && userRole == "Dirigente")
            {
                context.Response.Redirect("/Dirigente/Home/Index");
                return;
            }
        }

        await _next(context);
    }
}
