using BrokenAccessControl.Data;
using BrokenAccessControl.Models.DbModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BrokenAccessControl.Pages
{ 

    [Authorize]
    public class ItemsModel(ApplicationDbContext _dbContext, UserManager<IdentityUser<Guid>> _userManager) : PageModel
    {
        public List<ToDoItem> Items { get; set; } = new List<ToDoItem>();

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }
        public async Task OnGetAsync()
        {
            var loggedId = Guid.Parse(_userManager.GetUserId(User)!);
            Items = await _dbContext.ToDoItems
                .Where(x => x.OwnerId == loggedId)
                .ToListAsync();
        }


        public async Task<IActionResult> OnPostAsync(ToDoItem item)
        {
            var loggedId = Guid.Parse(_userManager.GetUserId(User)!);
            item.OwnerId = loggedId;

            _dbContext.ToDoItems.Add(item);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage();
        }


        //kod niebezpieczny
        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            await _dbContext.ToDoItems
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync();

            return RedirectToPage();
        }

        //kod bezpieczny
        //public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        //{
        //    var loggedId = Guid.Parse(_userManager.GetUserId(User)!);
        //    var loggedIsAdmin = User.IsInRole("Admin");

        //    if (!loggedIsAdmin)
        //    {
        //        var item = await _dbContext.ToDoItems
        //            .Where(x => x.Id == id)
        //            .FirstOrDefaultAsync();

        //        if (item == null || item.OwnerId != loggedId)
        //            return Forbid();
        //    }

        //    await _dbContext.ToDoItems
        //        .Where(x => x.Id == id)
        //        .ExecuteDeleteAsync();

        //    return RedirectToPage();
        //}
    }
}
