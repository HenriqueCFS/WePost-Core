using Microsoft.AspNetCore.Identity;
using core.Data.Models;

namespace core.Extensions
{
    public interface IPasswordValidator<TUser> where TUser : class
    {
        Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password);
    }
    public class UsernameAsPasswordValidator<TUser> : IPasswordValidator<TUser>
    where TUser : AppUser
    {
        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            if (string.Equals(user.UserName, password, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Code = "UsernameAsPassword",
                    Description = "You cannot use your username as your password"
                }));
            }
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
