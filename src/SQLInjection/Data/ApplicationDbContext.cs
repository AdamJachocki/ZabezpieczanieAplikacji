using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SQLInjection.Models.DbModels;

namespace SQLInjection.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
    {
        public DbSet<ToDoItem> ToDoItems { get; set; } = null!;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var todoItemBuilder = builder.Entity<ToDoItem>();
            todoItemBuilder.HasKey(t => t.Id);
            todoItemBuilder.Property(t => t.Id)
                .ValueGeneratedOnAdd();

            todoItemBuilder.HasOne(t => t.Owner)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
