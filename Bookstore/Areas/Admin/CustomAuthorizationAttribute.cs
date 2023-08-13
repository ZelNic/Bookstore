using Bookstore.DataAccess;
using Bookstore.Models.SD;

namespace Bookstore.Areas.Admin
{
    public class CustomAuthorizationAttribute : Attribute
    {
        public bool CheckUserIsAdmin(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
        {
            if (contextAccessor.HttpContext.Session.GetInt32("Username") != null)
            {
                int? userId = contextAccessor.HttpContext.Session.GetInt32("Username");

                if ((db.Employees.Find(userId) != null) && (db.Employees.Find(userId).RoleName == SD.RoleAdmin))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
