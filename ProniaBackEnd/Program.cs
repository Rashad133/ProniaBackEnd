using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.DAL;
using ProniaBackEnd.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt =>opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<LayoutService>();
var app = builder.Build();


app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute("areas", "{area:exists}/{controller=home}/{action=index}/{id?}");

app.MapControllerRoute("default","{controller=home}/{action=index}/{id?}");
    
app.Run();
