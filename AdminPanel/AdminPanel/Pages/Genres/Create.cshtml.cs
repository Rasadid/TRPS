using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminPanel.Pages.Genres
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationContext _context;

        public CreateModel(ApplicationContext context)
        {
            _context = context;
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
        public Genre Genre { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Genre == null || Genre == null)
            {
                return Page();
            }
            int _newId = 0;
            if (_context.Genre.ToList().Count != 0)
            {
                foreach (var genre in _context.Genre.ToList())
                {
                    if (genre.Id > _newId)
                    {
                        _newId = genre.Id;
                    }
                }

                Genre.Id = _newId + 1;
            }
            else
            {
                Genre.Id = 1;
            }
            _context.Genre.Add(Genre);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
