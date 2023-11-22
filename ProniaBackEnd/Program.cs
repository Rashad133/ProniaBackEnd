using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.DAL;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt =>opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
var app = builder.Build();


app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute("areas", "{area:exists}/{controller=home}/{action=index}/{id?}");

app.MapControllerRoute("default","{controller=home}/{action=index}/{id?}");

app.Run();
