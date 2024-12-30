using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace grpc_hello_world.Models
{
    [Table("announcements")]
    public class Announcement
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        // Foreign Key: Author Id (linked to User)
        [Column("author_id")]
        public Guid AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public User Author { get; set; }

        // Foreign Key: Keeper Id (linked to User)
        [Column("keeper_id")]
        public Guid? KeeperId { get; set; }
        [ForeignKey("KeeperId")]
        public User Keeper { get; set; }

        // Foreign Key: Animal Id (linked to User)
        [Column("animal_id")]
        public Guid AnimalId { get; set; }
        [ForeignKey("AnimalId")]
        public Animal Animal { get; set; }

        [Column("keeper_profit")]
        public uint KeeperProfit { get; set; }

        [Column("is_negotiable")]
        public bool IsNegotiable { get; set; }

        [Column("description")]
        public string Description { get; set; }

        // Timestamp properties (Unix)
        [Column("start_term")]
        public ulong StartTerm { get; set; }

        [Column("end_term")]
        public ulong EndTerm { get; set; }

        [Column("created_date")]
        public ulong CreatedDate { get; }

        [Column("started_date")]
        public ulong? StartedDate { get; set; }

        [Column("finished_date")]
        public ulong? FinishedDate { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("address_id")]
        public Guid AddressId { get; set; }
        [ForeignKey("AddressId")]  // Navigation property
        public Address? Address { get; set; }
    }
}

