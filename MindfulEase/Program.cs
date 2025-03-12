using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MindfulEase.Data;
using MindfulEase.Models;
using MindfulEase.Services.MindfulEase.Services;
using MindfulEase.Services;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();


// PASUL 2 - useri si roluri

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();  // Register HttpClient for compiler
builder.Services.AddScoped<BadgeService>();
builder.Services.AddScoped<RecommendationService>(); // Service-ul depinde de DbContext
builder.Services.AddScoped<ClusteringService>(); // Service-ul depinde de DbContext
builder.Services.AddHttpClient<SentimentAnalysisService>(); // Acest serviciu foloseste HTTP
builder.Services.AddScoped<RewardService>();
builder.Services.AddScoped<WeeklyChallengeService>();
builder.Services.AddScoped<ChallengeNotificationsService>();

var app = builder.Build();

// PASUL 5 - useri si roluri
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.MapRazorPages();

app.UseHangfireDashboard("/hangfire");
RecurringJob.AddOrUpdate<BadgeService>(
     service => service.CheckAndAwardBadgesAsync(),
     Cron.Minutely); //Seteaza jobul sa ruleze la fiecare minut

RecurringJob.AddOrUpdate<WeeklyChallengeService>(
    service => service.CheckWeeklyChallengesAsync(),   
    Cron.Minutely);  // Seteaz? jobul s? ruleze la fiecare minut

RecurringJob.AddOrUpdate<ChallengeNotificationsService>(
    service => service.CheckActiveChallengesAndSendNotifications(),
    Cron.Minutely);  // Seteaz? jobul s? ruleze la fiecare minut

app.Run();
