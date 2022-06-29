using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ASP_Razor_EF.models;
using Microsoft.AspNetCore.Authorization;

namespace ASP_Razor_EF.Pages_Blog
{
    [Authorize] // buộc user phải đăng nhập mới dược chuyễn qua các trang khác
    public class IndexModel : PageModel
    {
        
        private readonly ASP_Razor_EF.models.MyBlogContext _context;

        public IndexModel(ASP_Razor_EF.models.MyBlogContext context)
        {
            _context = context;
        }

        public IList<Article> Article { get;set; } = default!;

        public const int ITEMS_PER_PAGE = 5;
        [BindProperty(SupportsGet = true,Name = "p")]
        public int currentPage {set; get;}

        public int countPages {set; get;}

        public MyBlogContext Context => Context1;

        public MyBlogContext Context1 => _context;

        public async Task OnGetAsync(string SearchString)
        {
            if (Context.articles != null)
            {
                //Article = await _context.articles.ToListAsync();

                int totalArticle = await Context.articles.CountAsync();
                countPages = (int)Math.Ceiling((double)totalArticle / ITEMS_PER_PAGE);
                if(currentPage < 1)
                {
                    currentPage =1;
                }
                 if(currentPage > countPages)
                {
                    currentPage =countPages;
                }
                
                var qr = (from a in Context.articles
                         orderby a.Created descending
                         select a)
                         .Skip((currentPage-1)*ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE);
                if( !string.IsNullOrEmpty(SearchString))
                {
                    Article = qr.Where(a => a.Title.Contains(SearchString)).ToList();
                }
                else
                {
                    Article = await qr.ToListAsync();
                }
                
            }
        }
    }
}
