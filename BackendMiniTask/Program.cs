using BackendMiniTask.DAL;
using BackendMiniTask.Models;
using BackendMiniTask.Services;
using BackendMiniTask.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<AppDbContext>(opt =>
opt.UseSqlServer(builder.Configuration.GetConnectionString("default"))
);


builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 8;
    opt.Password.RequireDigit = true;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireUppercase = true;

    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(10);
    opt.Lockout.MaxFailedAccessAttempts = 30;
    opt.Lockout.AllowedForNewUsers = true;

    opt.User.AllowedUserNameCharacters = default;
    opt.User.RequireUniqueEmail = true;

}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(con => { con.LoginPath = $"/Account/Login/{con.ReturnUrlParameter}"; });





builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "default",
      pattern: "{controller=Home}/{action=Index}/{id?}"
    );
});

app.Run();
