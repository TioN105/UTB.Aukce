using System.ComponentModel.DataAnnotations;

namespace UTB.Aukce.Application.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Zadejte křestní jméno.")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadejte příjmení.")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadejte e-mail.")]
        [EmailAddress(ErrorMessage = "Neplatný formát e-mailu.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadejte heslo.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Heslo musí mít alespoň 6 znaků a obsahovat číslici.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potvrďte heslo.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Zadaná hesla se neshodují.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}