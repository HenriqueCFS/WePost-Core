using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core.Data.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }
        public string SenderId { get; set; }

        public string ReceiverId { get; set; } 
        public AppUser Sender { get; set; }
        public AppUser Receiver { get; set; }

        public string? Message { get; set; } = null;
        public string? Files { get; set; } = null;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public DateTime? SeenAt { get; set; } = null;


    }
}
