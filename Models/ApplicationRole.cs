using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Edge.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}