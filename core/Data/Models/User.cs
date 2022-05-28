using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core.Data.Models;
public class User : IdentityUser
{
    [Required]
    public override string UserName { get; set; }
    [Required]
    public override string Email { get; set; }
    [Required]
    public string Role { get; set; }
}

