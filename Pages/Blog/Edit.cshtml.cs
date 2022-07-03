using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models;
using Microsoft.AspNetCore.Authorization;

namespace App.Pages_Blog
{
    public class EditModel : PageModel
    {
        private readonly App.Models.AppDbContext _context;
         private readonly IAuthorizationService _authorizationService;

        public EditModel(App.Models.AppDbContext context,IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        [BindProperty]
        public Article Article { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.articles == null)
            {
                return Content("Not Find Post!");
            }

            var article =  await _context.articles.FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return Content("Not Find Post!");
            }
            Article = article;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Article).State = EntityState.Modified;

            try
            {
                // Kiem tra quyen cap nhat
                var canupdate =  await _authorizationService.AuthorizeAsync(this.User, Article, "CanUpdateArticle");
                if  (canupdate.Succeeded)
                {
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return Content("Không được quyền cập nhật");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(Article.Id))
                {
                    return Content("Not Find Post!");
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ArticleExists(int id)
        {
          return (_context.articles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
