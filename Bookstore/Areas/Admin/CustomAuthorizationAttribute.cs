using Bookstore.DataAccess;

namespace Bookstore.Areas.Admin
{
    public class CustomAuthorizationAttribute : Attribute
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;

        public CustomAuthorizationAttribute(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
        {
            _db = db;
            _contextAccessor = contextAccessor;
        }


        private bool CheckIfUserIsAdmin()
        {
            if (_contextAccessor.HttpContext.Session.GetInt32("Username") != null)
            {
                int? userId = _contextAccessor.HttpContext.Session.GetInt32("Username");
                if ((userId != null) && (_db.Roles.Find(userId) != null))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}
