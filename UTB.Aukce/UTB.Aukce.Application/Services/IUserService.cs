using UTB.Aukce.Application.ViewModels;

namespace UTB.Aukce.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserViewModel>> GetAllUsersAsync();
        Task<UserEditViewModel?> GetUserForEditAsync(string id);
        Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateUserAsync(UserEditViewModel model);
        Task<(bool Succeeded, string Error)> DeleteUserAsync(string id, string currentUserId);
    }
}