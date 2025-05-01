using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MindfulEase.Data;
using MindfulEase.Models;
using MindfulEase.Services.MindfulEase.Services;
using MindfulEase.Services;
using Hangfire;
using Microsoft.AspNetCore.StaticFiles;

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
builder.Services.AddScoped<RecommendationService>(); // Service-ul depinde de DbContext
builder.Services.AddScoped<ClusteringService>(); // Service-ul depinde de DbContext
builder.Services.AddHttpClient<SentimentAnalysisService>(); // Acest serviciu foloseste HTTP
builder.Services.AddScoped<RewardService>();
builder.Services.AddScoped<WeeklyChallengeService>();
builder.Services.AddScoped<WeeklyReportService>();
builder.Services.AddScoped<ChallengeNotificationsService>();
builder.Services.AddScoped<BadgeService>();
var appDataPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
// Înregistrează serviciul de predicție
builder.Services.AddSingleton(new BrainDamagePredictionService(appDataPath));

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
//app.UseStaticFiles();

var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".glb"] = "model/gltf-binary"; // sau "model/gltf+json" dacă e .gltf

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider,
    ServeUnknownFileTypes = true // opțional, dacă vrei fallback pentru alte extensii
});


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.MapRazorPages();

app.UseHangfireDashboard("/hangfire");

RecurringJob.AddOrUpdate<BadgeService>(
    "generate_user_badges",
     service => service.CheckAndAwardBadgesAsync(),
     Cron.Minutely); //Seteaza jobul sa ruleze la fiecare minut

RecurringJob.AddOrUpdate<WeeklyChallengeService>(
    "verify_weekly_challenges",
    service => service.CheckWeeklyChallengesAsync(),   
    Cron.Minutely);  // Seteaza jobul sa ruleze la fiecare minut

RecurringJob.AddOrUpdate<ChallengeNotificationsService>(
    "generate_notifications",
    service => service.CheckActiveChallengesAndSendNotifications(),
    Cron.Minutely);  // Seteaza jobul sa ruleze la fiecare minut

RecurringJob.AddOrUpdate<WeeklyReportService>(
    "generate_weekly_report",
    service => service.GenerateWeeklyReports(),
    Cron.Weekly(DayOfWeek.Sunday, 23, 59));

app.Run();
