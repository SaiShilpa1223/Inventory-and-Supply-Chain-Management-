using InventorySupply.DAL;
using InventorySupply.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly InventorySupplyDbContext _db;
    private readonly IConfiguration _config;
    public AuthController(InventorySupplyDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null || !VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt))
            return Unauthorized("Invalid credentials");

        var jwt = GenerateJwt(user);
        var refreshToken = CreateRefreshToken(user);
        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        return Ok(new LoginResponseDto
        {
            Token = jwt,
            Role = user.Role,
            RefreshToken = refreshToken.Token
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        var oldToken = await _db.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(r => r.Token == dto.Token);
        if (oldToken == null || oldToken.IsExpired) return Unauthorized("Invalid refresh token");

        var jwt = GenerateJwt(oldToken.User);
        var newToken = CreateRefreshToken(oldToken.User);

        _db.RefreshTokens.Remove(oldToken);
        _db.RefreshTokens.Add(newToken);
        await _db.SaveChangesAsync();

        return Ok(new LoginResponseDto
        {
            Token = jwt,
            Role = oldToken.User.Role,
            RefreshToken = newToken.Token
        });
    }

    private string GenerateJwt(User user)
    {
        var claims = new[] {
      new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
      new Claim(ClaimTypes.Name, user.Username),
      new Claim(ClaimTypes.Role, user.Role)
    };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], claims,
          expires: DateTime.UtcNow.AddHours(1), signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool VerifyPassword(string password, byte[] hash, byte[] salt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512(salt);
        var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computed.SequenceEqual(hash);
    }

    private RefreshToken CreateRefreshToken(User user) => new()
    {
        Token = Guid.NewGuid().ToString(),
        Expires = DateTime.UtcNow.AddDays(7),
        User = user
    };
}
