using Microsoft.AspNetCore.Identity;

namespace WebAPIDay2.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string ApplicationName { get; set; }
    }
}
