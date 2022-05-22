using core.Data.Models;
namespace core.Data.GraphQL
{
    public class UserPayload
    {
        public UserPayload(User user)
        {
            User = user;
        }

        public User User { get; }
    }
}
