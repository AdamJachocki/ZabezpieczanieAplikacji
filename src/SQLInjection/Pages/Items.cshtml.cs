using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SQLInjection.Data;
using SQLInjection.Models.DbModels;

namespace SQLInjection.Pages
{
    public enum GetMode
    {
        GetAll,
        SearchSafe,
        SearchAdo,
        SearchEf
    }

    [Authorize]
    public class ItemsModel(ApplicationDbContext _dbContext, UserManager<IdentityUser<Guid>> _userManager) : PageModel
    {
        public List<ToDoItem> Items { get; set; } = new List<ToDoItem>();

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }
        public async Task OnGetAsync(GetMode mode)
        {
            var loggedId = Guid.Parse(_userManager.GetUserId(User)!);
            switch (mode)
            {
                case GetMode.GetAll:
                    await GetAllItems(loggedId);
                    break;
                case GetMode.SearchSafe:
                    await SearchItemsSafe(loggedId);
                    break;

                case GetMode.SearchAdo:
                    await SearchWithAdo(loggedId);
                    break;

                case GetMode.SearchEf:
                    await SearchWithEf(loggedId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public async Task GetAllItems(Guid loggedId)
        {
            Items = await _dbContext.ToDoItems
                .Where(x => x.OwnerId == loggedId)
                .ToListAsync();
        }

        public async Task SearchItemsSafe(Guid loggedId)
        {
            var query = _dbContext.ToDoItems
                .Where(x => x.OwnerId == loggedId);

            if (!string.IsNullOrEmpty(SearchString))
                query = query.Where(x => x.Title.Contains(SearchString) || x.Description.Contains(SearchString));

            Items = await query.ToListAsync();
        }

        public async Task SearchWithAdo(Guid loggedId)
        {
            var connection = _dbContext.Database.GetDbConnection();
            await connection.OpenAsync();
            var command = connection.CreateCommand();

            command.CommandText = $"""  
                                        SELECT * 
                                        FROM ToDoItems 
                                        WHERE OwnerId = '{loggedId}' AND 
                                        (Title LIKE '%{SearchString}%' OR Description LIKE '%{SearchString}%')
                                    """;

            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Items.Add(new ToDoItem
                {
                    Id = reader.GetGuid(0),
                    Title = reader.GetString(1),
                    Description = reader.GetString(2),
                    IsCompleted = reader.GetBoolean(3),
                    OwnerId = reader.GetGuid(4)
                });
            }
        }

        public async Task SearchWithEf(Guid loggedId)
        {
            //kod bezpieczny
            //var finalSearch = $"%{SearchString}%";
            //FormattableString sql = 
            //    $"""
            //        SELECT *
            //        FROM ToDoItems 
            //        WHERE OwnerId = {loggedId} AND
            //        (Title LIKE {finalSearch} OR Description LIKE {finalSearch})
            //    """;

            //var query = _dbContext.ToDoItems.FromSqlInterpolated(sql); //FromSqlRaw(sql);

            //kod niebezpieczny
            string sql =
                $"""
                    SELECT *
                    FROM ToDoItems 
                    WHERE OwnerId = '{loggedId}' AND
                    (Title LIKE '%{SearchString}%' OR Description LIKE '%{SearchString}%')
                """;

            var query = _dbContext.ToDoItems.FromSqlRaw(sql);
            Items = await query.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(ToDoItem item)
        {
            var loggedId = Guid.Parse(_userManager.GetUserId(User)!);
            item.OwnerId = loggedId;

            _dbContext.ToDoItems.Add(item);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var item = await _dbContext.ToDoItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            _dbContext.ToDoItems.Remove(item);
            await _dbContext.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
