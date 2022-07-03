using App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Pages
{
    public class IndexModel : PageModel
    {
        
        private readonly ILogger<IndexModel> _logger;

        private readonly AppDbContext myBlogContext;
        public IndexModel(ILogger<IndexModel> logger, AppDbContext _myContext)
        {
            _logger = logger;
            myBlogContext = _myContext;
        }

        public void OnGet()
        {
            var posts = (from a in myBlogContext.articles
                         orderby a.Created descending
                         select a).ToList();
            ViewData["posts"] = posts;
        }
    }
}


