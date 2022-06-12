using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core.Data.Models
{
    public class Friend
    {
        public string RequestedById { get; set; }
        public string RequestedToId { get; set; }
        public  AppUser RequestedBy { get; set; }
        public  AppUser RequestedTo { get; set; }

        public DateTime? RequestTime { get; set; }

        public DateTime? BecameFriendsTime { get; set; }

        public FriendRequestFlag FriendRequestFlag { get; set; }

        [NotMapped]
        public bool Approved => FriendRequestFlag == FriendRequestFlag.Approved;

    }

    public enum FriendRequestFlag
    {
        None,
        Approved,
        Rejected,
        Blocked,
        Spam
    };
}
