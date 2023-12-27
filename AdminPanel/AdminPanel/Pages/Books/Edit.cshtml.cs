using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Pages.Books
{
    public class EditModel : PageModel
    {
        private readonly ApplicationContext _context;

        public EditModel(ApplicationContext context)
        {
            _context = context;
            Genres = _context.Genre/*.Include(p=>p.Books)*/.ToList();
           // CurrentGenres = _context.Book.ToList();
        }

        [BindProperty]
        public IFormFile? Picture { get; set; } = default!;
        [BindProperty]
        public Book Book { get; set; } = default!;
        [BindProperty]

        public List<Genre> CurrentGenres { get; set; } = default!;
        public List<Genre> Genres { get; set; } = default!;
        [BindProperty]
        public List<int> AreChecked { get; set; } = default!;

        private int idd;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToPage("/Login");
            }
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }
            idd = (int)id;
            var book = _context.Books.Include(p=>p.Genres).FirstOrDefault(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }
            CurrentGenres = book.Genres.ToList();
            Book = book;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (AreChecked.Count == 0)
            {
                return Page();
            }
            var _book = Book;
            Book = _context.Books.Include(p => p.Genres).FirstOrDefault(m => m.Id == Book.Id);
            Book.CountPages = _book.CountPages;
            Book.Author = _book.Author;
            Book.Description = _book.Description;
            Book.Title = _book.Title;

            if(Picture != null)
            {
                Book.Picture = (UploadCloud.UploadImage(Picture).SecureUri.AbsoluteUri).ToString();
            }
            
            foreach (var genre in Book.Genres.Where(g => !AreChecked.Contains(g.Id)).ToList())
            {
                Console.WriteLine(genre.Id);
                Book.Genres.Remove(genre);
            }
            
            foreach (var genreId in AreChecked)
            {
                var genre = await _context.Genre.FindAsync(genreId);
                if (genre != null)
                {
                    Book.Genres.Add(genre);
                }
            }

            _context.Update(Book);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(Book.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BookExists(int id)
        {
          return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
