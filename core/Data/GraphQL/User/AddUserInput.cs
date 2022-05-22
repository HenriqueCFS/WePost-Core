namespace core.Data.GraphQL
{
    public record AddUserInput(
       string Username,
       string Password,
       string Role
     );
}
