using IdentityServer.Enums;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Entities;

public class User : IdentityUser<Guid>
{
    //id, email and password are already defined 
    public Roles UserRole {get; private set;}
}