using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace AdminPanel.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly AdminCredentials _adminCredentials = new AdminCredentials { Login = "admin", Password = "password" };

        [BindProperty]
        public string Login { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (Login == _adminCredentials.Login && Password == _adminCredentials.Password)
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToPage("/Index");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return Page();
        }
    }
}
