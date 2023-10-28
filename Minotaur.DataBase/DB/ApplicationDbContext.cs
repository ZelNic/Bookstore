using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.SD;
using Microsoft.EntityFrameworkCore;
using Minotaur.Utility;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Minotaur.Models.OrganizationalDocumentation.HR;
using Minotaur.Models.Models.ModelReview;

namespace Minotaur.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<MinotaurUser>
    {
        DbContextOptionsBuilder optionsBuilder;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        }

        public DbSet<MinotaurUser> MinotaurUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ShoppingBasket> ShoppingBaskets { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<Order> Orders { get; set; }

        public DbSet<Review> ProductReviews { get; set; }
        //public DbSet
        // TODO: добавить новые таблицы для рейтингов заказов, доставки, сотрудников

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<RecordStock> StockMagazine { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<OrganizationalOrder> OrganizationalOrders { get; set; }
        public DbSet<RequestTelegram> TelegramRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    Name = "И нас пожирает пламя",
                    Author = "Жауме Кабре",
                    ISBN = "978-5-389-22890-0",
                    Category = 1,
                    Description = "Человек просыпается неизвестно где - возможно, в больничной палате, но это неточно - и не помнит о себе вообще ничего. " +
                    "\"Зовите меня Измаил\", - предлагает он врачам, которых, за неимением других версий, нарекает Юрием Живаго и мадам Бовари.",
                    Price = 417
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

