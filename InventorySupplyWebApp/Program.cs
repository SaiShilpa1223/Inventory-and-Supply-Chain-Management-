using InventorySupply.DAL;
using InventorySupplyWebApp.Interface;
using InventorySupplyWebApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// in Program.cs (WebApp project)
builder.Services.AddScoped<ITransferService, TransferService>();

// ✅ Register DbContext here
builder.Services.AddDbContext<InventorySupplyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Add MVC or Razor support (only call this once)
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpClient();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie(options =>
        {
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
            });
builder.Services.AddAuthorization(options =>
{
    // This policy acts like [Authorize] everywhere with no attribute
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();  // Serve static files

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
// Authorization middleware
app.UseAuthorization();
app.UseSession();
// Define endpoints
app.MapControllers(); // Map API controllers
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"); // De
// Run the application
app.Run();