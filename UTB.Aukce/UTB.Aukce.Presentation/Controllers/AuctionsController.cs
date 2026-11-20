using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UTB.Aukce.Application.Services;
using UTB.Aukce.Application.ViewModels;

namespace UTB.Aukce.Presentation.Controllers
{
    public class AuctionsController : Controller
    {
        private readonly IAuctionService _auctionService;
        private readonly ICategoryService _categoryService;

        public AuctionsController(IAuctionService auctionService, ICategoryService categoryService)
        {
            _auctionService = auctionService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? categoryId, string? search, bool onlyActive = true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _auctionService.GetCatalogAsync(categoryId, search, onlyActive, userId);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _auctionService.GetAuctionDetailAsync(id, userId);
            if (model == null)
                return NotFound();

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new AuctionCreateViewModel
            {
                Categories = await _categoryService.GetAllAsync()
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuctionCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoryService.GetAllAsync();
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _auctionService.CreateAuctionAsync(model, userId);
            TempData["Success"] = "Aukce byla úspěšně vytvořena.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceBid(int auctionId, decimal amount)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var (succeeded, error) = await _auctionService.PlaceBidAsync(auctionId, amount, userId);

            if (succeeded)
                TempData["Success"] = "Váš příhoz byl úspěšně zaznamenán.";
            else
                TempData["Error"] = error;

            return RedirectToAction(nameof(Details), new { id = auctionId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleWatchlist(int auctionId, string? returnUrl)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _auctionService.ToggleWatchlistAsync(auctionId, userId);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Details), new { id = auctionId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int auctionId, int rating, string comment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var (succeeded, error) = await _auctionService.AddReviewAsync(auctionId, rating, comment, userId);

            if (succeeded)
                TempData["Success"] = "Hodnocení bylo přidáno.";
            else
                TempData["Error"] = error;

            return RedirectToAction(nameof(Details), new { id = auctionId });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyActivity()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var model = await _auctionService.GetMyActivityAsync(userId);
            return View(model);
        }
    }
}