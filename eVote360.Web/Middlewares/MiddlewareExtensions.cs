namespace eVote360.Web.Middlewares;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        => builder.UseMiddleware<AuthMiddleware>();
}
