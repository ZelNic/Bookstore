using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Minotaur.DataAccess;
using Minotaur.Models;
using Minotaur.Utility;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<MinotaurUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();


builder.Services.AddScoped<UserManager<MinotaurUser>>();
builder.Services.AddScoped<SignInManager<MinotaurUser>>();

builder.Services.AddRazorPages();
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = $"/Areas/Identity/Account/Logout";
    option.LogoutPath = $"/Areas/Identity/Account/Login";
    option.AccessDeniedPath = $"/Areas/Identity/Account/AccessDenied";
});



var app = builder.Build();

if (!app.Environment.IsDevelopment())
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

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.Run();




//builder.Services.AddScoped<UserManager<MinotaurUser>>();
//builder.Services.AddScoped<SignInManager<User>>();
//builder.Services.AddScoped<ILogger<LoginModel>, Logger<LoginModel>>();