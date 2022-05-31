using core.Data.Models;
using core.Data.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace core.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        public class ExplorerUser
        {
            string UserName { get; set; }
            string Name { get; set; }
        }
        
        private readonly UserManager<User> userManager;

        public UsersController(UserManager<User> userManager)
        {
            this.userManager = userManager;  
        }


        [HttpGet]
        [Authorize(Policy = "IsUser")]
        [Route("explore")]
        public async Task<IActionResult> GetAllUsers()
        {
            IList<User> allUsers = await userManager.GetUsersInRoleAsync("User");
            var treatedUsers = allUsers.Select(User => new { User.Id, User.UserName,  User.Name });
            return Ok(treatedUsers);

        }



        [HttpGet]
        [Authorize(Policy = "IsUser")]
        [Route("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
           User currentUser = await userManager.FindByNameAsync(User.Identity.Name);
           return Ok(currentUser);
        }
         
    }
} 
