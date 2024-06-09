


using Microsoft.AspNetCore.Identity;

namespace BackendMiniTask.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
