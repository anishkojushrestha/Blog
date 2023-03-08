using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class ResetPasswordVM
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        [Compare("Password")]
        [Required]
        public string? ConfirmPassword { get; set; }
    }
}
