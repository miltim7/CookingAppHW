using System.Data.SqlClient;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Identity/Login";
        options.ReturnUrlParameter = "returnUrl";
    });

string? connectionString = builder.Configuration.GetConnectionString("CookingDB");

ArgumentNullException.ThrowIfNull(connectionString);

builder.Services.AddScoped<IRecipesRepository>(p =>
{
    return new RecipesRepository(new SqlConnection(connectionString));
});

builder.Services.AddScoped<IUserRepository>(p =>
{
    return new UserRepository(new SqlConnection(connectionString));
});

builder.Services.AddScoped<IRecipeService>(p => {
    return new RecipeService(new SqlConnection(connectionString), new RecipesRepository(new SqlConnection(connectionString)));
});

builder.Services.AddScoped<IUserService>(p =>
{
    return new UserService(new SqlConnection(connectionString), new UserRepository(new SqlConnection(connectionString)));
});

bool CanLog = builder.Configuration.GetSection("CanLog").Get<bool>();

if (CanLog)
{
    builder.Services.AddTransient<LogMiddleware>();

    builder.Services.AddScoped<ILogRepository>(provider =>
    {
        return new LogRepository(new SqlConnection(connectionString));
    });
}

var app = builder.Build();

if (CanLog)
    app.UseMiddleware<LogMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Main}/{id?}");

app.Run();