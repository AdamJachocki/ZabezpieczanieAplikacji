using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;

namespace PathTraversal.Pages
{
    public class IndexModel(IWebHostEnvironment _webHost) : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string FileName { get; set; } = string.Empty;
        public List<string?> ExistingFiles = new();

        public void OnGet()
        {
            string mainPath = Path.Combine(_webHost.WebRootPath, "data");
            if (Directory.Exists(mainPath))
            {
                ExistingFiles = Directory.GetFiles(mainPath).Select(Path.GetFileName).ToList();
            }
        }

        public IActionResult OnGetFile()
        {
            if (string.IsNullOrWhiteSpace(FileName))
                return Page();

            string mainPath = Path.Combine(_webHost.ContentRootPath, "Data");
            string path = Path.Combine(mainPath, FileName); //vulnerability

            if (!System.IO.File.Exists(path))
                return NotFound();

            return Content(System.IO.File.ReadAllText(path));
        }

        public IActionResult OnGetShowFileInsecure(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Page();

            string mainPath = Path.Combine(_webHost.WebRootPath, "data");
            string path = Path.Combine(mainPath, name); //vulnerability

            if (!System.IO.File.Exists(path))
                return NotFound();

            var contentTypeProvider = new FileExtensionContentTypeProvider();
            var contentType = contentTypeProvider.Mappings[Path.GetExtension(path)];
            return File(System.IO.File.ReadAllBytes(path), contentType);
        }

        public IActionResult OnGetShowFileSecure(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Page();

            name = Path.GetFileName(name); //vulnerability fix
            string mainPath = Path.Combine(_webHost.WebRootPath, "data");
            string path = Path.Combine(mainPath, name);

            if (!System.IO.File.Exists(path))
                return NotFound();

            var contentTypeProvider = new FileExtensionContentTypeProvider();
            var contentType = contentTypeProvider.Mappings[Path.GetExtension(path)];
            return File(System.IO.File.ReadAllBytes(path), contentType);
        }


        public IActionResult OnGetSecure()
        {
            if (string.IsNullOrWhiteSpace(FileName))
                return Page();
            

            FileName = Path.GetFileName(FileName); //vulnerability fix
            string mainPath = Path.Combine(_webHost.ContentRootPath, "Data");
            string path = Path.Combine(mainPath, FileName);

            if (!System.IO.File.Exists(path))
                return NotFound();

            return Content(System.IO.File.ReadAllText(path));
        }
    }
}
