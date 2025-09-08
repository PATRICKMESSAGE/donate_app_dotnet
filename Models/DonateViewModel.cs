using System.ComponentModel.DataAnnotations;

namespace GiveApp.Models
{
    public class DonateViewModel
    {
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Please enter a valid amount.")]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }
}