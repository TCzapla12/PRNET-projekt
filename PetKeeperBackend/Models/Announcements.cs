using System.ComponentModel.DataAnnotations.Schema;

namespace grpc_hello_world.Models
{
    [Table("announcements")]
    public class Announcement
    {
        [Column("id")]
        public Guid Id { get; set; }

        // Foreign Key: Author Email (linked to User)
        [Column("author_email")]
        public string AuthorEmail { get; set; }
        public User Author { get; set; }

        // Foreign Key: Keeper Email (linked to User)
        [Column("keeper_email")]
        public string KeeperEmail { get; set; }
        public User Keeper { get; set; }

        // Other properties
        [Column("keeper_profit")]
        public int KeeperProfit { get; set; }

        [Column("is_negotiable")]
        public bool IsNegotiable { get; set; }

        [Column("long_term")]
        public bool LongTerm { get; set; }

        [Column("description")]
        public string Description { get; set; }

        // Timestamp properties (Unix)
        [Column("start_term")]
        public long StartTerm { get; set; }

        [Column("end_term")]
        public long EndTerm { get; set; }

        [Column("created_date")]
        public long CreatedDate { get; set; }

        [Column("started_date")]
        public long? StartedDate { get; set; }

        [Column("finished_date")]
        public long? FinishedDate { get; set; }

        // Status field
        [Column("status")]
        public string Status { get; set; }

        // Address details
        [Column("address_street")]
        public string AddressStreet { get; set; }

        [Column("address_house_number")]
        public string AddressHouseNumber { get; set; }

        [Column("address_city")]
        public string AddressCity { get; set; }

        [Column("address_post_code")]
        public string AddressPostCode { get; set; }
    }
}

