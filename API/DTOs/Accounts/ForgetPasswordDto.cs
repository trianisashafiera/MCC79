using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Accounts;
    public class ForgetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

