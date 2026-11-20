using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UTB.Aukce.Application.Services;
using UTB.Aukce.Application.ViewModels;

namespace UTB.Aukce.Presentation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var model = await _userService.GetUserForEditAsync(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var reloaded = await _userService.GetUserForEditAsync(model.Id);
                model.AvailableRoles = reloaded?.AvailableRoles ?? new List<string>();
                return View(model);
            }

            var (succeeded, errors) = await _userService.UpdateUserAsync(model);
            if (succeeded)
                return RedirectToAction(nameof(Index));

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            var reload = await _userService.GetUserForEditAsync(model.Id);
            model.AvailableRoles = reload?.AvailableRoles ?? new List<string>();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var (succeeded, error) = await _userService.DeleteUserAsync(id, currentUserId);

            if (!succeeded)
            {
                TempData["Error"] = error;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}