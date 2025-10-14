using Microsoft.EntityFrameworkCore;
using Municiple_Project_st10259527.Repository;
using Municiple_Project_st10259527.Services;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var dataDir = Path.Combine(builder.Environment.ContentRootPath, "Data");
Directory.CreateDirectory(dataDir);
var dbPath = Path.Combine(dataDir, "municipal.db");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAnnouncementsRepository, AnnouncementsRepository>();
builder.Services.AddScoped<IEventsRepository, EventsRepository>();
builder.Services.AddScoped<IUserSearchHistoryRepository, UserSearchHistoryRepository>();
builder.Services.AddScoped<RecommendationService>();
builder.Services.AddScoped<EventManagementService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



var app = builder.Build();
app.UseDeveloperExceptionPage();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
