using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core.Data.Models
{
    [Table("Bookmarks")]
    public class Bookmark
    {
        public int BlogId { get; set; }

        [Required]
        public string senderId { get; set; }
    }
}
