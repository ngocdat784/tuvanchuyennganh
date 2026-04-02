using System.Collections.Generic; 
using System.ComponentModel.DataAnnotations; 
namespace CareerOrientationAPI.Models {
    public class Admin
    {
    [Key]
    public int AdminId { get; set; }

    [Required, MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string PasswordHash { get; set; } = string.Empty;

    // phân quyền admin
    [MaxLength(50)]
    public string Role { get; set; } = "Admin";
 
}



    }
