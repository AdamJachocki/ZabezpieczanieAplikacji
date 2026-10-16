using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace OpenRedirect_Original.Pages
{
    public class IndexModel : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public string RedirectUrl { get; set; } = string.Empty;


        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost(string username, string password)
        {
            //assume we have valid username and password
            //so we log in

            var identity = new ClaimsIdentity(
                [
                    new Claim(ClaimsIdentity.DefaultNameClaimType, "User")
                ], CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);
            return RedirectAfterLogin();
        }


        private IActionResult RedirectAfterLogin()
        {
            if (string.IsNullOrEmpty(RedirectUrl))
                return RedirectToPage("/Account");
            else
                return Redirect(RedirectUrl); //kod niebezpieczny
        }













        //private IActionResult RedirectAfterLogin()
        //{
        //    if (string.IsNullOrEmpty(RedirectUrl))
        //        return RedirectToPage("/Account");
        //    else
        //        return RedirectToPage(RedirectUrl); 
        //}
    }
}
