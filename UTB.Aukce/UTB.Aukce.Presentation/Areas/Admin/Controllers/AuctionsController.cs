using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UTB.Aukce.Application.Services;
using UTB.Aukce.Application.ViewModels;

namespace UTB.Aukce.Presentation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
    public class AuctionsController : Controller
    {
        private readonly IAuctionService _auctionService;
        private readonly ICategoryService _categoryService;

        public AuctionsController(IAuctionService auctionService, ICategoryService categoryService)
        {
            _auctionService = auctionService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _auctionService.GetAdminModerationDataAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _auctionService.GetAuctionForAdminEditAsync(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminAuctionEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoryService.GetAllAsync();
                return View(model);
            }

            await _auctionService.UpdateAuctionAsync(model);
            TempData["Success"] = "Aukce byla úspěšně upravena.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _auctionService.DeleteAuctionAsync(id);
            TempData["Success"] = "Aukce včetně navázaných příhozů a recenzí byla smazána.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBid(int id)
        {
            await _auctionService.DeleteBidAsync(id);
            TempData["Success"] = "Příhoz byl odstraněn.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int id)
        {
            await _auctionService.DeleteReviewAsync(id);
            TempData["Success"] = "Recenze byla odstraněna.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWatchlistItem(int id)
        {
            await _auctionService.DeleteWatchlistItemAsync(id);
            TempData["Success"] = "Sledovaná položka byla odstraněna.";
            return RedirectToAction(nameof(Index));
        }
    }
}