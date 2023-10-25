using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Minotaur.DataAccess;
using Minotaur.DataAccess.DbInitializer;
using Minotaur.DataAccess.Repository;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Scheduler;
using Minotaur.TelegramController;
using Minotaur.Utility;
using Quartz.Impl;
using Quartz;
using Telegram.Bot;
using Telegram.Bot.Polling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<MinotaurUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<UserManager<MinotaurUser>>();
builder.Services.AddScoped<SignInManager<MinotaurUser>>();
builder.Services.AddScoped<IEmailSender, EmailSender>();



builder.Services.AddRazorPages();

builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = $"/Areas/Identity/Account/Logout";
    option.LogoutPath = $"/Areas/Identity/Account/Login";
    option.AccessDeniedPath = $"/Areas/Identity/Account/AccessDenied";
});


builder.Services.AddScoped<ITelegramBotClient>(provider =>
{
    var botToken = "6504892449:AAEDmHDwgkFG_Wg6Gywn-5ivRHcePsySn-4";
    return new TelegramBotClient(botToken);
});
builder.Services.AddTransient<TelegramController>();


builder.Services.AddSingleton<IScheduler>(provider =>
{
    var schedulerFactory = new StdSchedulerFactory();
    return schedulerFactory.GetScheduler().GetAwaiter().GetResult();
});

builder.Services.AddSingleton<QuartzHostedService>();
builder.Services.AddSingleton(provider =>
{
    var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
    return new ConfirmationCodeHandler(unitOfWork);
});


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

SeedDatabase();


app.UseRouting();



app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

LifeTelegramBot(app.Services.GetService<IServiceProvider>());

app.Run();


void LifeTelegramBot(IServiceProvider serviceProvider)
{
    var scope = serviceProvider.CreateScope();
    var telegramBot = scope.ServiceProvider.GetRequiredService<TelegramController>();
    telegramBot.StartReceiving<IUpdateHandler>(null, CancellationToken.None);   
}
void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}