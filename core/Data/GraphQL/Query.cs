using core.Data.Models;

namespace core.Data.GraphQL
{
    public class Query
    {
        [GraphQLNonNullType]
        public IQueryable<User> getUsers([Service] ProjectContext context) =>
            context.Users;

        public User GetUser([Service] ProjectContext dbContext, int id) => dbContext.Users.FirstOrDefault(x => x.Id == id);
    }
}
