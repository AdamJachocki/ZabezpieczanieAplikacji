using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SQLInjection.Data;
using SQLInjection.Models.DbModels;

namespace SQLInjection.Pages
{
    public class EditItemModel(ApplicationDbContext _dbContext) : PageModel
    {
        [BindProperty]
        public ToDoItem Item { get; set; } = new ToDoItem();
        public async Task OnGet(Guid id)
        {
            Item = await _dbContext.ToDoItems.FirstAsync(x => x.Id == id);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            _dbContext.Attach(Item).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return RedirectToPage("/Items");
        }
    }
}
