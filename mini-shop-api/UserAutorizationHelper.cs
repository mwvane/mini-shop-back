using Microsoft.EntityFrameworkCore;
using mini_shop_api.Models;

namespace mini_shop_api
{
    public static class UserAutorizationHelper
    {
        public static bool CanCreateItem(User user)
        {
            if(user == null) return false;
            if(user.Role == "user")
            {
                return false;
            }
            return true;
        }
        public static bool CanUpdateItem(User user, Product item)
        {
            if (user == null) return false;
            if (user.Role == "user")
            {
                return false;
            }
            if(user.Role == "seller" && item.CreatedBy != user.Id) 
            { 
                return false;
            }
            return true;
        }
        public static bool CanDeleteItem(User user, Product item)
        {
            return CanUpdateItem(user, item);
        }

    }
}
