using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Interfaces;
using ProniaBackEnd.Models;
using ProniaBackEnd.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSession(options=>
options.IdleTimeout=TimeSpan.FromSeconds(50)
);
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(opt =>opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;

    options.User.RequireUniqueEmail = true;

    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(3);
}
).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<LayoutService>();
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.UseStaticFiles();


app.MapControllerRoute("areas", "{area:exists}/{controller=home}/{action=index}/{id?}");

app.MapControllerRoute("default","{controller=home}/{action=index}/{id?}");
    
app.Run();
