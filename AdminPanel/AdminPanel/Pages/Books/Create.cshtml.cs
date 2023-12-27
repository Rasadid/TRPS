using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminPanel.Pages.Genres;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;

namespace AdminPanel.Pages.Books
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationContext _context;

        public CreateModel(ApplicationContext context)
        {
            _context = context;
            Genres = _context.Genre.ToList();

        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToPage("/Login");
            }
            return Page();
        }

        [BindProperty]
        public Book Book { get; set; } = default!;
        public List<Genre> Genres { get; set; } = default!;
        [BindProperty]
        public List<int> AreChecked { get; set; }
        [BindProperty]
        public IFormFile Image { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Books == null || Book == null || AreChecked.Count == 0)
            {
                return Page();
            }

            int _newId = 0;

            if (_context.Books.ToList().Count != 0)
            {
                foreach (var book in _context.Books.ToList())
                {
                    if(book.Id > _newId)
                    {
                        _newId = book.Id;
                    }
                }

                Book.Id = _newId + 1;
            }
            else
            {
                Book.Id = 1;
            }

            if (Genres != null)
            {
                List<Genre> _genres = new List<Genre>();
                for(int i = 0; i < Genres.Count; i++)
                {
                    for (int j = 0; j < AreChecked.Count; j++) 
                    {
                        if (AreChecked[j] == Genres[i].Id)
                        {
                            _genres.Add(Genres[i]);
                        }
                    }
                }
                Book.Genres = _genres;
            }

            Book.Picture = (UploadCloud.UploadImage(Image).SecureUri.AbsoluteUri).ToString();

            _context.Books.Add(Book);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }

}
