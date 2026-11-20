using System.ComponentModel.DataAnnotations;

namespace UTB.Aukce.Application.ViewModels
{
    public class UserEditViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadejte křestní jméno.")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadejte příjmení.")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadejte e-mail.")]
        [EmailAddress(ErrorMessage = "Neplatný formát e-mailu.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vyberte roli uživatele.")]
        public string SelectedRole { get; set; } = string.Empty;

        public List<string> AvailableRoles { get; set; } = new();
    }
}