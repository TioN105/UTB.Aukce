using Microsoft.EntityFrameworkCore;
using UTB.Aukce.Domain.Interfaces;
using UTB.Aukce.Infrastructure.Data;
using UTB.Aukce.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Přidání MVC služeb (Controllers with Views)
builder.Services.AddControllersWithViews();

// 2. Registrace DbContextu pro Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Registrace generického repozitáře (Scoped lifetime)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();