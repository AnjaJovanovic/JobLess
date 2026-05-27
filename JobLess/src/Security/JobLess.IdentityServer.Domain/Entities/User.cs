using JobLess.IdentityServer.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace JobLess.IdentityServer.Domain.Entities;

public class User : IdentityUser<Guid>
{
    //id, email and password are already defined 
    public Roles UserRole {get; set;}
}