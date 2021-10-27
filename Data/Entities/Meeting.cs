using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingRooms.Data.Entities
{
    [Table("Meetings")]
    public class Meeting
    {
        [Key]
        public int Id { get; set; }
        public DateTime StartDatetime { get; set; }
        public DateTime EndDatetime { get; set; }
        public string AttendingCompany { get; set; }
        public int MeetingRoomId { get; set; }
        [ForeignKey(nameof(MeetingRoomId))]
        public MeetingRoom MeetingRoom { get; set; }

        public virtual Attendees Attendees { get; set; }
    }
}
