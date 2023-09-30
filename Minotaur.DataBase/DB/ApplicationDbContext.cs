using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.SD;
using Microsoft.EntityFrameworkCore;
using Minotaur.Utility;

namespace Minotaur.DataAccess
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<ShoppingBasket> ShoppingBasket { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<OrderPickupPoint> OrderPickupPoint { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<RecordStock> StockJournal { get; set; }

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
                    HouseNumber = "30",
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
                    IsCourierDelivery = false,
                    OrderStatus = SD.StatusRefunded_6,
                    CurrentPosition = "Moskow",
                    TravelHistory = "SPB,Moskow"
                }
            );

            modelBuilder.Entity<ShoppingBasket>().HasData(
                new ShoppingBasket
                {
                    BasketId = 1,
                    UserId = 1,
                    ProductIdAndCount = "1:1"
                }
            );

            modelBuilder.Entity<Employees>().HasData(
                new Employees
                {
                    EmployeeId = 1,
                    UserId = 1,
                    RoleName = SD.RoleAdmin,
                }
            );

            modelBuilder.Entity<WishList>().HasData(
                new WishList
                {
                    WishListId = 1,
                    UserId = 1,
                    ProductId = "1",
                }
            );

            modelBuilder.Entity<OrderPickupPoint>().HasData(
                new OrderPickupPoint
                {
                    PointId = 1,
                    City = Cities.Moskow,
                    Street = Streets.Soviet,
                    BuildingNumber = "1",
                    WorkingHours = "08:00 - 20:00",
                    CountOfOrders = 0,
                }
            );

            modelBuilder.Entity<Notification>().HasData(
                new Notification
                {
                    Id = 1,
                    SenderId = 1,
                    RecipientId = 1,
                    Text = "seed",
                    IsHidden = false,
                    SendingTime = new DateTime(),
                }
            );

            modelBuilder.Entity<Stock>().HasData(
                new Stock
                {
                    Id = 26,
                    City = Cities.Moskow,
                    Street = Streets.Central,
                    ResponsiblePersonId = 12
                }
            );
            modelBuilder.Entity<RecordStock>().HasData(
                new RecordStock
                {
                    Id = 1,
                    StockId = 26,
                    ResponsiblePersonId = 12,
                    Time = MoscowTime.GetTime(),
                    ProductId = 1,
                    Count = 1,
                    ShelfNumber = 1,
                    Operation = OperationStock.ReceiptOfGoods,
                    IsOrder = true,
                }
            ); ;
        }
    }
}

