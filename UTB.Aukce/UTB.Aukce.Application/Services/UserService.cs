using Microsoft.AspNetCore.Identity;
using UTB.Aukce.Application.ViewModels;
using UTB.Aukce.Domain.Entities;

namespace UTB.Aukce.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<UserViewModel>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? string.Empty,
                    Role = roles.FirstOrDefault() ?? "Bez role",
                    RegisteredAt = user.RegisteredAt
                });
            }

            return result;
        }

        public async Task<UserEditViewModel?> GetUserForEditAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name!).ToList();

            return new UserEditViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                SelectedRole = userRoles.FirstOrDefault() ?? "Customer",
                AvailableRoles = allRoles
            };
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateUserAsync(UserEditViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return (false, new[] { "Uživatel nebyl nalezen." });

            // Upravují se pouze základní údaje a role – PasswordHash zůstává nedotčený
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return (false, updateResult.Errors.Select(e => e.Description));

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(model.SelectedRole))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.SelectedRole);
            }

            return (true, Enumerable.Empty<string>());
        }

        public async Task<(bool Succeeded, string Error)> DeleteUserAsync(string id, string currentUserId)
        {
            if (id == currentUserId)
                return (false, "Nemůžete smazat svůj vlastní přihlášený účet.");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return (false, "Uživatel nebyl nalezen.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return (false, "Uživatele nelze smazat (pravděpodobně má již vytvořené aukce nebo příhozy).");

            return (true, string.Empty);
        }
    }
}