using GraphQL.Types;

namespace core.Models
{
    public class UserType : ObjectGraphType<User>
    {
        public UserType()
        {
            Name = "User";

            Field(x=> x.Username).Description("User Name");

            Field(x => x.Id).Description("User Id");

            Field(x => x.Password).Description("Password");

            Field(x => x.Role).Description("Role");


        }
    }
}
