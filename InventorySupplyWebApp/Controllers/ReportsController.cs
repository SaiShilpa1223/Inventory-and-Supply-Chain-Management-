using InventorySupplyWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace InventorySupplyWebApp.Controllers
{
    public class ReportsController : Controller
    {
        private readonly HttpClient _httpClient;

        public ReportsController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IActionResult> Index()
        {
            // Fetch the raw JSON string from the API
            var responseString = await _httpClient.GetStringAsync("http://localhost:5146/api/reports/products-by-warehouse");

            var Warehouse = JsonConvert.DeserializeObject<List<ReportsModel>>(responseString);

            // If the response is null, use an empty list
            if (Warehouse == null)
            {
                Warehouse = new List<ReportsModel>();
            }
            return View(Warehouse);
        }
    }
}
