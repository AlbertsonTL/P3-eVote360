using eVote360.Application.Common;
using eVote360.Infrastructure.DependencyInjection;
using eVote360.Infrastructure.Persistence;
using eVote360.Web.Middlewares;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Application layer
builder.Services.AddApplicationServices();

// Infrastructure layer
builder.Services.AddInfrastructureServices(builder.Configuration);

// User session
builder.Services.AddScoped<IUserSession, UserSessionService>();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    await DataSeeder.SeedAsync(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

// Custom auth middleware
app.UseAuthMiddleware();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
