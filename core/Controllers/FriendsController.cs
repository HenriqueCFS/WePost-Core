using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using core.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace core.Controllers
{




    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {

        public class UserContactInfo
        {
            public string UserName { get; set; }
            public string Email { get; set; }   
            public string Id { get; set; }
        }
        private readonly ProjectContext _context;

        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;

        public FriendsController(ProjectContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
        }

        // GET: api/Friends
        [HttpGet]
        [Authorize(Policy = "IsUser")]
        public async Task<ActionResult<IEnumerable<Friend>>> GetFriends()
        {
            if (_context.Friends == null)
            {
                return NotFound();
            }
            User currentUser = await userManager.FindByNameAsync(User.Identity.Name);
            var friendList  = await _context.Friends.Where(friend => (friend.RequestedById == currentUser.Id || friend.RequestedToId == currentUser.Id) && (friend.FriendRequestFlag == FriendRequestFlag.Approved || friend.FriendRequestFlag == FriendRequestFlag.None))
                .Select(friend => new { friend.RequestedById, friend.RequestedToId, friend.FriendRequestFlag, RequestedBy = new UserContactInfo() { Id = friend.RequestedBy.Id, UserName = friend.RequestedBy.UserName, Email = friend.RequestedBy.Email}, RequestedTo = new UserContactInfo() { Id = friend.RequestedTo.Id, UserName = friend.RequestedTo.UserName, Email = friend.RequestedTo.Email}})
                .ToListAsync();
            return Ok(friendList);
        }

        // GET: api/Friends/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Friend>> GetFriend(string id)
        {
            if (_context.Friends == null)
            {
                return NotFound();
            }
            var friend = await _context.Friends.FindAsync(id);

            if (friend == null)
            {
                return NotFound();
            }

            return friend;
        }

        // PUT: api/Friends/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFriend(string id, Friend friend)
        {
            if (id != friend.RequestedById)
            {
                return BadRequest();
            }

            _context.Entry(friend).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FriendExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Friends
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Policy = "IsUser")]
        public async Task<ActionResult<Friend>> PostFriend([FromBody] AddFriendModel model)
        {
            User reqByUser = await userManager.FindByNameAsync(User.Identity.Name);
            User reqToUser = await userManager.FindByNameAsync(model.RequestedTo);

            Friend hasRequest = _context.Friends.Find(reqToUser.Id, reqByUser.Id);

            if (hasRequest != null && hasRequest.FriendRequestFlag == FriendRequestFlag.Approved)
            {
                return Ok("You are already friends.");
            }

            if (hasRequest != null && hasRequest.RequestedToId == reqByUser.Id && hasRequest.FriendRequestFlag == FriendRequestFlag.None)
            {
                hasRequest.FriendRequestFlag = FriendRequestFlag.Approved;
                hasRequest.BecameFriendsTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                Response res = new()
                {
                    Status = "created",
                    Message = "Friend Added."
                };
                return Ok(res);

            }
            if (hasRequest != null)
            {
                return Conflict("Friend request already sent.");
            }

            if (reqByUser == null || reqToUser == null)
            {
                return NotFound("User not found.");
            }
            Friend friend = new();
            friend.AddFriendRequest(reqByUser, reqToUser);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FriendExists(friend.RequestedById))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Friend request sent.");
        }

        // DELETE: api/Friends/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFriend(string id)
        {
            if (_context.Friends == null)
            {
                return NotFound();
            }
            var friend = await _context.Friends.FindAsync(id);
            if (friend == null)
            {
                return NotFound();
            }

            _context.Friends.Remove(friend);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FriendExists(string id)
        {
            return (_context.Friends?.Any(e => e.RequestedById == id)).GetValueOrDefault();
        }
    }
}
