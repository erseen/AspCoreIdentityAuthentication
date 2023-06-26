using Microsoft.AspNetCore.Identity;

namespace AspCoreIdentityAuthentication.Identity
{
    public class User:IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
