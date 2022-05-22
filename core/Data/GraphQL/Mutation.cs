using core.Data.Models;
using GraphQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

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
                Username = input.Username,
                Password = input.Password,
                Role = input.Role
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return new UserPayload(user);
        }

        public async Task<User> UpdateUserAsync(
            UpdateUserInput input,
            [Service] ProjectContext context)
        {
            var selectedUser = context.Users?.Where(x => x.Id == input.Id).SingleOrDefault();
            try
            {
                if (selectedUser == null)
                {
                    throw new Exception("User not found.");
                }
                selectedUser.Username = input.Username;
                selectedUser.Password = input.Password;
                selectedUser.Role = input.Role;
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
