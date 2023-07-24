using Bookstore.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Title = "И нас пожирает пламя",
                    Author = "Жауме Кабре",
                    ISBN = "978-5-389-22890-0",
                    Category = 1,
                    Description = "Человек просыпается неизвестно где - возможно, в больничной палате, но это неточно - и не помнит о себе вообще ничего. " +
                    "\"Зовите меня Измаил\", - предлагает он врачам, которых, за неимением других версий, нарекает Юрием Живаго и мадам Бовари.",
                    Price = "417"
                }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Художественная литература",
                }
            );

        }

    }
}

