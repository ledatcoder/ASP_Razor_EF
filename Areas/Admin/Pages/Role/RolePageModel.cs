using ASP_Razor_EF.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Admin.Role
{
    public class RolePageModel : PageModel
    {
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly MyBlogContext _context;
        [TempData]
        public string StatusMessage {set;get;}
        public RolePageModel(RoleManager<IdentityRole> roleManager,MyBlogContext myBlogContext)
        {
            _roleManager = roleManager;
            _context = myBlogContext;
        }
    }
}