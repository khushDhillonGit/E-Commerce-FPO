using System.ComponentModel.DataAnnotations;

namespace ECommerce.ViewModels
{
    public class ResetPasswordViewModel
    {
        public Guid UserId { get; set; }
        [Required]
        [Compare(nameof(ConfirmPassword))]
        public string NewPassword { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
}
