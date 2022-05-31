using System.ComponentModel.DataAnnotations;

namespace core.Data.Models
{
    public class AddFriendModel
    {
        [Required(ErrorMessage = "Target user to add required!")]
        public string RequestedTo { get; set; }

    }
}