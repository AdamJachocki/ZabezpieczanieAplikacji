using Microsoft.AspNetCore.Identity;

namespace SQLInjection.Models.DbModels
{
    public class ToDoItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsCompleted { get; set; }
        public Guid OwnerId { get; set; }
        public IdentityUser<Guid>? Owner { get; set; }
    }
}
