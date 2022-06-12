using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core.Data.Models;



public class AppUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public AppUser()
    {
        SentFriendRequests = new List<Friend>();
        ReceievedFriendRequests = new List<Friend>();
    }

    public string Name { get; set; }

    public string ProfilePicture { get; set; }

    public ICollection<ChatMessage>? SentMessages { get; set; } = new Collection<ChatMessage>();
    public ICollection<ChatMessage> ReceivedMessages { get; set; } = new Collection<ChatMessage>();

    public ICollection<Friend> SentFriendRequests { get; set; } = new Collection<Friend>();

    public ICollection<Friend> ReceievedFriendRequests { get; set; } = new Collection<Friend>();

    [NotMapped]
    public ICollection<Friend> Friends
    {
        get
        {
            var friends = SentFriendRequests.Where(x => x.Approved).ToList();
            friends.AddRange(ReceievedFriendRequests.Where(x => x.Approved));
            return friends;
        }
    }
    [NotMapped]
    public virtual ICollection<ChatMessage> Messages
    {
        get
        {
            var messages = SentMessages.ToList();
            messages.AddRange(ReceivedMessages);
            return messages;
        }
    }
}

