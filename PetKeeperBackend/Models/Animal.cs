using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace grpc_hello_world.Models
{
    [Table("animals")]
    public class Animal
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("name")]
        public required string Name { get; set; }

        [Column("owner_id")]
        public required Guid OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public User? Owner { get; set; } // Navigation property to `User`

        [Column("type")]
        public required string Type { get; set; }

        [Column("photo")]
        public required string Photo { get; set; }

        [Column("description")]
        public required string Description { get; set; }
    }
}
