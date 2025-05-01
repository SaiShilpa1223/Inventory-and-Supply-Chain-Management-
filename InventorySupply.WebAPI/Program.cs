using System;
using System.Text;
using System.Text.Json;
using InventorySupply.DAL;
using InventorySupply.DAL.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext from DAL project
builder.Services.AddDbContext<InventorySupplyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(opt =>
  {
      opt.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = builder.Configuration["Jwt:Issuer"],
          ValidAudience = builder.Configuration["Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(key)
      };
  });
builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InventorySupplyDbContext>();
    if (!db.Users.Any())
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        db.Users.AddRange(
          new User
          {
              Username = "admin",
              PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Admin@123")),
              PasswordSalt = hmac.Key,
              Role = "Admin"
          },
          new User
          {
              Username = "manager",
              PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Manager@123")),
              PasswordSalt = hmac.Key,
              Role = "Manager"
          },
          new User
          {
              Username = "operator",
              PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Operator@123")),
              PasswordSalt = hmac.Key,
              Role = "Operator"
          });
        db.SaveChanges();
    }
}

// Enable Swagger only in Development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // Maps API controllers

app.Run();