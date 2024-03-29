using System.Data.SqlClient;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

string? connectionString = builder.Configuration.GetConnectionString("CookingDB");

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Identity/Login";
        options.AccessDeniedPath = "/Identity/Login";
        options.ReturnUrlParameter = "returnUrl";
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<MyDbContext>(dbContextOptionsBuilder =>
{
    dbContextOptionsBuilder.UseSqlServer(connectionString, useSqlOptions => {
        useSqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
    });
});

// IDENTITY
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = true;
})
    .AddEntityFrameworkStores<MyDbContext>();

// RECIPE
builder.Services.AddScoped<IRecipesRepository>(p => {
    return new RecipesRepository(new SqlConnection(connectionString));
});

builder.Services.AddScoped<IRecipeService>(p => {
    return new RecipeService(new SqlConnection(connectionString), new RecipesRepository(new SqlConnection(connectionString)));
});

// BUCKET
builder.Services.AddScoped<IBucketRepository>(p => {
    return new BucketRepository(new SqlConnection(connectionString));
});
builder.Services.AddScoped<IBucketService>(p => {
    return new BucketService(new BucketRepository(new SqlConnection(connectionString)));
});

// COMMENTS
builder.Services.AddScoped<ICommentService>(p => {
    return new CommentService(new CommentRepository(new SqlConnection(connectionString)), new SqlConnection(connectionString));
});
builder.Services.AddScoped<ICommentRepository>(p => {
    return new CommentRepository(new SqlConnection(connectionString));
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