using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationContext _context;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public IndexModel(ApplicationContext context)
        {
            _context = context;
        }

        public List<Book> Book { get;set; } = default!;

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToPage("/Login");
            }
            if (SearchString != null)
            {
                var books = _context.Books.Include(u => u.Genres).Where(b => b.Title.Contains(SearchString) || b.Author.Contains(SearchString)).ToList();
                Book = books;
            }
            else
            {
                Book = _context.Books.Include(u => u.Genres).ToList();
            }
            return Page();
        }
    }
}
