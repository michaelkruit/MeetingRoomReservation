using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingRooms.Data.Entities
{
    [Table("Attendees")]
    public class Attendees
    {
        [Key]
        public int Id { get; set; }
        public string Names { get; set; }
        public int MeetingId { get; set; }
        [ForeignKey(nameof(MeetingId))]
        public Meeting Meeting { get; set; }
    }
}
