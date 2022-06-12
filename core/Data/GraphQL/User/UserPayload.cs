using core.Data.Models;
namespace core.Data.GraphQL
{
    public class UserPayload
    {
        public UserPayload(AppUser user)
        {
            User = user;
        }

        public AppUser User { get; }
    }
}
