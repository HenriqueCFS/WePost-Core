using System.Text;  
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using core.Data;
using core.Data.Roles;
using core.Data.Models;

namespace core.Controllers
{  
    [Route("api/[controller]")]  
    [ApiController]  
    public class AuthController : ControllerBase  
    {  
        private readonly UserManager<User> userManager;  
        private readonly RoleManager<IdentityRole> roleManager;  
        private readonly IConfiguration _configuration;  
  
        public AuthController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)  
        {  
            this.userManager = userManager;  
            this.roleManager = roleManager;  
            _configuration = configuration;  
        }  
  
        [HttpPost]  
        [Route("login")]  
        public async Task<IActionResult> Login([FromBody] LoginModel model)  
        {  
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))  
            {  
                var userRoles = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>();
  
                foreach (var userRole in userRoles)  
                {  
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));  
                }  
  
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Security:JWT:Secret"]));
                var token = new JwtSecurityToken(  
                    issuer: _configuration["Security:JWT:Issuer"],  
                    audience: _configuration["Security:JWT:Audience"],  
                    expires: DateTime.Now.AddHours(3),  
                    claims: authClaims,  
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)  
                    );  
  
                return Ok(new  
                {  
                    token = new JwtSecurityTokenHandler().WriteToken(token),  
                    expiration = token.ValidTo  
                });  
            }  
            return Unauthorized();  
        }  
  
        [HttpPost]  
        [Route("register")]  
        public async Task<IActionResult> Register([FromBody] RegisterModel model)  
        {  
            var userExists = await userManager.FindByNameAsync(model.UserName);
            var emailExists = await userManager.FindByEmailAsync(model.UserName);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            if (emailExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Email already registered!" }); 
            

            User user = new User()  
            {  
                Email = model.Email,  
                SecurityStamp = Guid.NewGuid().ToString(),  
                UserName = model.UserName,
                Role = "Admin"
            };  
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again.", Errors = result.Errors });

            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await roleManager.RoleExistsAsync(UserRoles.User))
            {
                await userManager.AddToRoleAsync(user, UserRoles.User);
            }
            return Ok(new Response { Status = "Success", Message = "User created successfully!", Errors = result.Errors });  
        }  
  
        [HttpPost]  
        [Route("register-admin")]  
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)  
        {
            var userExists = await userManager.FindByNameAsync(model.UserName);
            var emailExists = await userManager.FindByEmailAsync(model.UserName);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            if (emailExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Email already registered!" });

            User user = new User()  
            {  
                Email = model.Email,  
                SecurityStamp = Guid.NewGuid().ToString(),  
                UserName = model.UserName,
            };  
            var result = await userManager.CreateAsync(user, model.Password);  
            if (!result.Succeeded)  
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again.", Errors = result.Errors });  
  
            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))  
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));  
            if (!await roleManager.RoleExistsAsync(UserRoles.User))  
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));  
  
            if (await roleManager.RoleExistsAsync(UserRoles.Admin))  
            {  
                await userManager.AddToRoleAsync(user, UserRoles.Admin);  
            }  
  
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });  
        }  
    }  
}  