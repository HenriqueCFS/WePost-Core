using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core.Data.Models
{
    public class Friend
    {
        public string RequestedById { get; set; }
        public string RequestedToId { get; set; }
        public virtual User RequestedBy { get; set; }
        public virtual User RequestedTo { get; set; }

        public DateTime? RequestTime { get; set; }

        public DateTime? BecameFriendsTime { get; set; }

        public FriendRequestFlag FriendRequestFlag { get; set; }

        [NotMapped]
        public bool Approved => FriendRequestFlag == FriendRequestFlag.Approved;

        public void AddFriendRequest(User user, User friendUser)
        {
            var friendRequest = new Friend()
            {
                RequestedBy = user,
                RequestedTo = friendUser,
                RequestTime = DateTime.UtcNow,
                FriendRequestFlag = FriendRequestFlag.None
            };
            user.SentFriendRequests.Add(friendRequest);
        }
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
