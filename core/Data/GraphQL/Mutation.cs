using core.Data.Models;
using GraphQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using core.Data.GraphQL;

namespace core.Data.GraphQL
{
    public class Mutation
    {
        public async Task<UserPayload> AddUserAsync(
            AddUserInput input,
            [Service] ProjectContext context)
        {
            var user = new User
            {
                UserName = input.Username,
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return new UserPayload(user);
        }

        public async Task<User> UpdateUserAsync(
            UpdateUserInput input,
            [Service] ProjectContext context)
        {
            var selectedUser = context.Users?.Where(x => x.UserName == input.Username).FirstOrDefault();
            try
            {
                if (selectedUser == null)
                {
                    throw new Exception("User not found.");
                }
                selectedUser.UserName = input.Username;
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (selectedUser == null)
                {
                    throw new Exception("User not found.");
                }
                else
                {
                    throw;
                }
            }
            return selectedUser;
        }

        public interface IError<T>
        {
        }
    }
}
