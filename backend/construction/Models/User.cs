using System.ComponentModel.DataAnnotations;
namespace ConstructionProjectManagement.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; } 
    }
} 