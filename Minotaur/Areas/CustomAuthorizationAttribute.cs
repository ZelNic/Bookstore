using Minotaur.DataAccess;
using Minotaur.Models.SD;

namespace Minotaur.Areas.CustomAuthorization
{
    public class CustomAuthorizationAttribute : Attribute
    {
        public bool AccessСheck(ApplicationDbContext db, IHttpContextAccessor contextAccessor, string accessLevel)
        {
            if (contextAccessor.HttpContext.Session.GetInt32("UserId") != null)
            {
                int? userId = contextAccessor.HttpContext.Session.GetInt32("UserId");

                if ((db.Employees.Find(userId) != null) && (db.Employees.Find(userId).RoleName == accessLevel))
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
