using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core.Data.Models
{
    [Table("Users")]
    public class User
    {
        [GraphQLType(typeof(NonNullType<IdType>))]
        [Key]
        public int Id { get; set; }
        [GraphQLNonNullType]
        [Required]
        public string Username { get; set; }
        [GraphQLNonNullType]
        [Required]
        public string Password { get; set; }
        [GraphQLNonNullType]
        [Required]
        public string Role { get; set; }
    }
}
