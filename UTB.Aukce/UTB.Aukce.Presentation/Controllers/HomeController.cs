using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UTB.Aukce.Application.Services;
using UTB.Aukce.Presentation.Models;

namespace UTB.Aukce.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuctionService _auctionService;

        public HomeController(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var catalog = await _auctionService.GetCatalogAsync(null, null, onlyActive: true, userId);
            return View(catalog);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}