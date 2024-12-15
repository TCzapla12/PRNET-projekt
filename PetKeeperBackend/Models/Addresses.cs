using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace grpc_hello_world.Models
{
    [Table("addresses")]
    public class Address
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("street")]
        public required string Street { get; set; }

        [Column("house_number")]
        public required string HouseNumber { get; set; }

        [Column("apartment_number")]
        public string? ApartmentNumber { get; set; }

        [Column("city")]
        public required string City { get; set; }

        [Column("post_code")]
        public required string PostCode { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("is_primary")]
        public required bool IsPrimary { get; set; }

        [Column("owner_id")]
        public required Guid OwnerId { get; set; } // This references `users(email)`

        [ForeignKey("OwnerId")]
        public User? Owner { get; set; } // Navigation property to `User`

    }
}
