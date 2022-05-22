namespace core.Data.GraphQL
{
    public record UpdateUserInput(
       int Id,
       string Username,
       string Password,
       string Role
     );
}
