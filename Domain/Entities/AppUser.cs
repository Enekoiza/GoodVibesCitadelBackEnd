namespace Domain.Entities;

using Microsoft.AspNetCore.Identity;

public class AppUser : IdentityUser
{
    public bool IsPasswordTemporary { get; set; }
}