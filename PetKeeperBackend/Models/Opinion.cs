using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace grpc_hello_world.Models
{
    [Table("opinions")]
    public class Opinion
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

        // Foreign Key: Announcement Id (linked to Announcement)
        [Column("announcement_id")]
        public Guid? AnnouncementId { get; set; }
        [ForeignKey("KeeperId")]
        public Announcement Announcement { get; set; }

        [Column("rating")]
        public uint Rating { get; set; } // CHECK (rating >= 0 AND rating <= 10)

        [Column("description")]
        public string Description { get; set; }

        [Column("created_date")]
        public ulong CreatedDate { get; }
    }
}
