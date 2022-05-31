using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core.Data.Models;



public class User : IdentityUser
{
    public User()
    {
        SentFriendRequests = new List<Friend>();
        ReceievedFriendRequests = new List<Friend>();
    }

    public string Name { get; set; }

    public string ProfilePicture { get; set; }

    public virtual ICollection<Friend> SentFriendRequests { get; set; }

    public virtual ICollection<Friend> ReceievedFriendRequests { get; set; }

    [NotMapped]
    public virtual ICollection<Friend> Friends
    {
        get
        {
            var friends = SentFriendRequests.Where(x => x.Approved).ToList();
            friends.AddRange(ReceievedFriendRequests.Where(x => x.Approved));
            return friends;
        }
    }
}

