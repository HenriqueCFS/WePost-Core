using core.Data.Models;

namespace core.Data.GraphQL
{
    public class Query
    {
        [GraphQLNonNullType]
        public IQueryable<AppUser> getUsers([Service] ProjectContext context) =>
            context.Users;

        public AppUser GetUser([Service] ProjectContext dbContext, string username) => dbContext.Users.FirstOrDefault(x => x.UserName == username);
    }
}
