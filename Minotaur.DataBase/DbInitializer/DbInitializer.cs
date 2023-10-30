using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Minotaur.Models;
using Minotaur.Models.SD;

namespace Minotaur.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<MinotaurUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(UserManager<MinotaurUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }

        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }


            if (!_roleManager.RoleExistsAsync(Roles.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(Roles.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.Role_Order_Picker)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.Role_Worker_Order_Pickup_Point)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.Role_Stockkeeper)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.Role_HR)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.Role_Operator)).GetAwaiter().GetResult();

                _userManager.CreateAsync(new MinotaurUser
                {
                    UserName = "ninileo55555@gmail.com",
                    Email = "ninileo55555@gmail.com",
                    FirstName = "Николай",
                    Surname = "Игоревич",
                    LastName = "Админович",
                    PhoneNumber = "1234567890",

                }, "604c075d-c691-49d6-9d6f-877cfa866e59").GetAwaiter().GetResult();


                MinotaurUser? user = _db.MinotaurUsers.FirstOrDefault(u => u.Email == "ninileo55555@gmail.com");
                _userManager.AddToRoleAsync(user, Roles.Role_Admin).GetAwaiter().GetResult();
            }

            return;
        }
    }
}
