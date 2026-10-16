using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OpenRedirect_Fake.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {

        }

        public IActionResult OnPost(string username, string password)
        {
            //save user credentials and redirect to the account page

            return Redirect("https://localhost:7297/Account");
        }
    }
}
