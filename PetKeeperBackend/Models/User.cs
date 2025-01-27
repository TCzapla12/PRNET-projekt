using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace grpc_hello_world.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set;  }

        [Column("email")]
        public required string Email { get; set; }

        [Column("username")]
        [Index(IsUnique = true)]
        public required string Username { get; set; }

        [Column("password_hash")]
        public required string PasswordHash { get; set; }

        [Column("first_name")]
        public required string FirstName { get; set; }

        [Column("last_name")]
        public required string LastName { get; set; }

        // Optional data, that can be set later
        [Column("phone")]
        public string? Phone { get; set; }

        [Column("pesel")]
        public string? Pesel { get; set; }

        [Column("is_activated")]
        public bool IsActivated { get; set; } = false;

        [Column("is_verified")]
        public bool IsVerified { get; set; } = false;

        [Column("is_banned")]
        public bool IsBanned { get; set; } = false;

        [Column("is_admin")]
        public bool IsAdmin { get; set; } = false;

        [Column("avatar_url")]
        public string? AvatarUrl { get; set; }

        [Column("document_url")]
        public required string[] DocumentUrl { get; set; }

        [Column("created_date")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public long? CreatedDate { get; private set; }
    }
}