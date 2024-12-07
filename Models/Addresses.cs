using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace grpc_hello_world.Models
{
    [Table("addresses")]
    public class Address
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("address_street")]
        public required string Street { get; set; }

        [Column("address_house_number")]
        public string? HouseNumber { get; set; }

        [Column("address_city")]
        public required string City { get; set; }

        [Column("address_post_code")]
        public required string PostCode { get; set; }

        [Column("owner_email")]
        public required string OwnerEmail { get; set; } // This references `users(email)`

        [ForeignKey("OwnerEmail")]
        public User? Owner { get; set; } // Navigation property to `User`

        [Column("is_primary")]
        public required bool IsPrimary { get; set; }
    }
}
