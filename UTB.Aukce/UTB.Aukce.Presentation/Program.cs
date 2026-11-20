using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UTB.Aukce.Application.Services;
using UTB.Aukce.Domain.Entities;
using UTB.Aukce.Domain.Interfaces;
using UTB.Aukce.Infrastructure.Data;
using UTB.Aukce.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Základní služby pro MVC
builder.Services.AddControllersWithViews();

// 2. Připojení k databázi
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Registrace Identity (Přihlašování a role)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// 4. Registrace repozitářů a našich aplikačních služeb
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IAuctionService, AuctionService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 5. Směrování pro Areas (musí být NAD defaultní routou)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 6. Naplnění výchozích rolí, účtů a kategorií
await DbInitializer.SeedAsync(app.Services);

app.Run();