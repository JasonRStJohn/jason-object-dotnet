using Microsoft.AspNetCore.Identity;

namespace MeDotNet.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Post> Posts { get; set; } = [];
}
