using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingRooms.Data.Entities
{
    [Table("Meetings")]
    public class MeetingRoom
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }
    }
}
