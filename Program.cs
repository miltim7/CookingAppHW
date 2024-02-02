using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

string? connectionString = builder.Configuration.GetConnectionString("CookingDB");

ArgumentNullException.ThrowIfNull(connectionString);

builder.Services.AddScoped<ICookingRepository>(p =>
{
    return new CookingRepository(new SqlConnection(connectionString));
});

bool CanLog = builder.Configuration.GetSection("CanLog").Get<bool>();
if (CanLog)
{
    builder.Services.AddTransient<LogMiddleware>();

    builder.Services.AddScoped<ILogService>(provider =>
    {
        return new LogService(new SqlConnection(connectionString));
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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();