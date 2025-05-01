namespace InventorySupplyWebApp.Models;

public class TokenResponse
{
    public string Token { get; set; }
    public string Role { get; set; }
    public string RefreshToken { get; set; }
}