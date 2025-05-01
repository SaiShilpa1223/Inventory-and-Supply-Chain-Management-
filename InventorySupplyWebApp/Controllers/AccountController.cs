using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using InventorySupplyWebApp.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace InventorySupplyWebApp.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly HttpClient _httpClient;

    public AccountController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpGet]
    public IActionResult Login()
    {
        //  HttpContext.Session.Clear(); // ✅ Now it's allowed here
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Prepare the login data to send to the API
            var loginData = new
            {
                username = model.Username,
                password = model.Password
            };

            var jsonContent = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send the login data to the API
            var response = await _httpClient.PostAsync("http://localhost:5146/api/auth/", content);

            //if (response.IsSuccessStatusCode)
            //{
            //    var responseData = await response.Content.ReadAsStringAsync();
            //    var tokens = JsonConvert.DeserializeObject<TokenResponse>(responseData);

            //    // Redirect to the Home page
            //    return RedirectToAction("Index", "Home");
            //}
            //else
            //{
            //    // If authentication fails, show an error message
            //    ModelState.AddModelError("", "Invalid login attempt.");
            //}

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }
            var responseData = await response.Content.ReadAsStringAsync();
            var authResult = JsonConvert.DeserializeObject<TokenResponse>(responseData);
            // Build claims for cookie
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, model.Username)
        };
            //foreach (var role in authResult.Roles)
            //    claims.Add(new Claim(ClaimTypes.Role, role));
            claims.Add(new Claim(ClaimTypes.Role, authResult.Role));

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);
            var props = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                props);

            // Optionally store JWT for API calls
            HttpContext.Session.SetString("JWToken", authResult.Token);

            // Always redirect to Home/Index
            return RedirectToAction("Index", "Home");

        }

        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }
}