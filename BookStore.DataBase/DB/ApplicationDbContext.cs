using Bookstore.Models;
using Bookstore.Models.Models;
using Bookstore.Models.SD;
using Bookstore.Models.ViewModel;
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
        public DbSet<User> User { get; set; }
        public DbSet<ShoppingBasket> ShoppingBasket { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<Order> Order { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    BookId = 1,
                    Title = "И нас пожирает пламя",
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

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    FirstName = "Nick",
                    LastName = "Zel",
                    DateofBirth = "2018-10-25",
                    Email = "ninileo55555@gmail.com",
                    Password = "admin",
                    Region = "KMAO",
                    City = "Surgut",
                    Street = "Lenina",
                    HouseNumber = 30,
                    PhoneNumber = "89226578108"
                }
            );

            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    OrderId = 1,
                    UserId = 1,
                    ProductData = "test",
                    PurchaseDate = new DateTime(),
                    PurchaseAmount = 1,
                    isCourierDelivery = false,
                    OrderStatus = SD.StatusRefunded,
                    CurrentPosition = "Moskow",
                    TravelHistory = "SPB,Moskow"                    
                }
            );

            modelBuilder.Entity<ShoppingBasket>().HasData(
                new ShoppingBasket
                {
                    BasketId = 1,
                    UserId = 1,
                    ProductId = 1,
                    CountProduct = 1
                }
            );

            modelBuilder.Entity<Roles>().HasData(
                new Roles
                {
                    RoleId = 1,
                    UserId = 1,
                    RoleName = SD.Role_Admin,
                }
            );

            modelBuilder.Entity<WishList>().HasData(
                new WishList
                {
                    WishListId = 1,
                    UserId = 1,
                    ProductId = 1,
                    CountProduct = 1
                }
            );


        }

    }
}

