using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace CodeInjection.Pages
{
    public class IndexModel : PageModel
    {
        public string Message { get; set; } = null!;
    
        public void OnPostUnsafe(string url)
        {
            var psi = new ProcessStartInfo("cmd.exe", $"/c ping {url}")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var process = Process.Start(psi);
            string output = process.StandardOutput.ReadToEnd();
            output = output.Replace(Environment.NewLine, "<br />");
            Message = output;
        }

        public void OnPostSafe(string url)
        {
            if (url.Contains('&'))
            {
                Message = "Invalid URL";
                return;
            }

            var psi = new ProcessStartInfo("ping", url)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var process = Process.Start(psi);
            string output = process.StandardOutput.ReadToEnd();
            output = output.Replace(Environment.NewLine, "<br />");
            Message = output;
        }
    }
}
